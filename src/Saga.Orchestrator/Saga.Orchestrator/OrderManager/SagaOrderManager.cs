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
        string? inventoryDocumentNo = null;

        //1. Get Basket by Username
        orderStateMachine.Configure(OrderTransactionStateEnum.NotStarted)
            .PermitDynamic(OrderActionEnum.GetBasket, () =>
            {
                cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                return cart.UserName != null
                    ? OrderTransactionStateEnum.BasketGot
                    : OrderTransactionStateEnum.BasketGetFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.GetBasket));

        //2. Create order (Place Order)
        orderStateMachine.Configure(OrderTransactionStateEnum.BasketGot)
            .PermitDynamic(OrderActionEnum.CreateOrder, () =>
            {
                var order = _mapper.Map<CreateOrderDto>(input);
                order.TotalPrice = cart.TotalPrice;
                orderId = _orderHttpRepository.CreateOrder(order).Result;
                return orderId > 0
                    ? OrderTransactionStateEnum.OrderCreated
                    : OrderTransactionStateEnum.OrderCreatedFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.CreateOrder));

        //3. Get Order detail by Order Id
        orderStateMachine.Configure(OrderTransactionStateEnum.OrderCreated)
            .PermitDynamic(OrderActionEnum.GetOrder, () =>
            {
                addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                return addedOrder != null
                    ? OrderTransactionStateEnum.OrderGot
                    : OrderTransactionStateEnum.OrderGetFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.GetOrder));

        //4. Inventory update
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

        //5. Delete Basket
        orderStateMachine.Configure(OrderTransactionStateEnum.InventoryUpdated)
            .PermitDynamic(OrderActionEnum.DeleteBasket, () =>
            {
                var result = _basketHttpRepository.DeleteBasket(input.UserName).Result;
                return result
                    ? OrderTransactionStateEnum.BasketDeleted
                    : OrderTransactionStateEnum.InventoryUpdateFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.DeleteBasket));

        //6. Rollback Order
        orderStateMachine.Configure(OrderTransactionStateEnum.InventoryUpdateFailed)
            .PermitDynamic(OrderActionEnum.DeleteInventory, () =>
            {
                RollBackOrder(input.UserName, inventoryDocumentNo, orderId);
                return OrderTransactionStateEnum.InventoryRollback;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.DeleteInventory));

        orderStateMachine.Fire(OrderActionEnum.GetBasket);

        return new OrderResponse(orderStateMachine.State == OrderTransactionStateEnum.BasketDeleted);
    }

    public OrderResponse RollBackOrder(string userName, string documentNo, long orderId)
    {
        var orderStateMachine =
            new Stateless.StateMachine<OrderTransactionStateEnum, OrderActionEnum>(OrderTransactionStateEnum
                .RollbackInventory);
        orderStateMachine.Configure(OrderTransactionStateEnum.RollbackInventory)
            .PermitDynamic(OrderActionEnum.DeleteInventory, () =>
            {
                _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
                return OrderTransactionStateEnum.InventoryRollback;
            });

        orderStateMachine.Configure(OrderTransactionStateEnum.InventoryRollback)
            .PermitDynamic(OrderActionEnum.DeleteOrder, () =>
            {
                var result = _orderHttpRepository.DeleteOrder(orderId).Result;
                return result
                    ? OrderTransactionStateEnum.OrderDeleted
                    : OrderTransactionStateEnum.OrderDeletedFailed;
            }).OnEntry(() => orderStateMachine.Fire(OrderActionEnum.DeleteOrder));

        orderStateMachine.Fire(OrderActionEnum.DeleteInventory);
        return new OrderResponse(orderStateMachine.State == OrderTransactionStateEnum.InventoryRollback);
    }
}