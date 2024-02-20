using AutoMapper;
using Contracts.Saga.OrderManager;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Cart;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.OrderManager;

public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public SagaOrderManager(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public OrderResponse CreateOrder(BasketCheckoutDto input)
    {
        var orderStateMachine =
            new Stateless.StateMachine<OrderTransactionStateEnum, OrderActionEnum>(OrderTransactionStateEnum
                .NotStarted);
        long orderId = -1;
        CartDto cart = null;
        OrderDto addedOrder = null;
        string? inventoryDocumentNo;

        orderStateMachine.Configure(OrderTransactionStateEnum.NotStarted)
            .PermitDynamic(OrderActionEnum.GetBasket, () =>
            {
                cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                return cart != null ? OrderTransactionStateEnum.BasketGot : OrderTransactionStateEnum.BasketGetFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.GetBasket));
        ;

        orderStateMachine.Configure(OrderTransactionStateEnum.BasketGot)
            .PermitDynamic(OrderActionEnum.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0
                    ? OrderTransactionStateEnum.OrderCreated
                    : OrderTransactionStateEnum.OrderCreatedFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.CreateOrder));

        orderStateMachine.Configure(OrderTransactionStateEnum.OrderCreated)
            .PermitDynamic(OrderActionEnum.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null
                    ? OrderTransactionStateEnum.OrderGot
                    : OrderTransactionStateEnum.OrderGetFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.GetOrder));

        orderStateMachine.Configure(OrderTransactionStateEnum.OrderGot)
            .PermitDynamic(OrderActionEnum.UpdateInvetory, () =>
            {
                var salesOrder = new SalesOrderDto()
                {
                    OrderNo = addedOrder.DocumentNo,
                    SaleItems = _mapper.Map<List<SaleItemDto>>(cart.Items)
                };

                inventoryDocumentNo =
                    _inventoryHttpRepository.CreateOrderSale(addedOrder.DocumentNo, salesOrder).Result;
                return inventoryDocumentNo != null
                    ? OrderTransactionStateEnum.InventoryUpdated
                    : OrderTransactionStateEnum.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.UpdateInvetory));


        return new OrderResponse(orderStateMachine.State == OrderTransactionStateEnum.InventoryUpdated);
    }

    public OrderResponse RollBackOrder(BasketCheckoutDto input)
    {
        return new OrderResponse(false);
    }
}