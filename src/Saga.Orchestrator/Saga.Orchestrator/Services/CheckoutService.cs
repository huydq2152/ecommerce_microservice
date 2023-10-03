using AutoMapper;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IOrderHttpRepository _orderHttpRepository;
    private readonly IBasketHttpRepository _basketHttpRepository;
    private readonly IInventoryHttpRepository _inventoryHttpRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CheckoutService(IOrderHttpRepository orderHttpRepository, IBasketHttpRepository basketHttpRepository,
        IInventoryHttpRepository inventoryHttpRepository, IMapper mapper, ILogger logger)
    {
        _orderHttpRepository = orderHttpRepository;
        _basketHttpRepository = basketHttpRepository;
        _inventoryHttpRepository = inventoryHttpRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> CheckoutOrder(string userName, BasketCheckoutDto basketCheckout)
    {
        // Get card 
        _logger.Information($"Start: Get Cart {userName}");
        var card = await _basketHttpRepository.GetBasket(userName);
        if (card == null) return false;
        _logger.Information($"End: Get Cart {userName} success");

        // Create order
        _logger.Information($"Start: Create Order");

        var order = _mapper.Map<CreateOrderDto>(basketCheckout);
        order.TotalPrice = card.TotalPrice;
        var orderId = await _orderHttpRepository.CreateOrder(order);
        if (orderId < 0) return false;
        var addedOrder = await _orderHttpRepository.GetOrder(orderId);

        _logger.Information($"End: Created Order success, Order Id: {orderId} - Document No - {addedOrder.DocumentNo}");

        var inventoryDocumentNos = new List<string>();
        try
        {
            // Create sale item
            foreach (var cartItem in card.Items)
            {
                _logger.Information($"Start: Sale Item No: {cartItem.ItemNo} - Quantity: {cartItem.Quantity}");

                var saleOrder = new SalesProductDto(addedOrder.DocumentNo, cartItem.Quantity);
                saleOrder.SetItemNo(cartItem.ItemNo);
                var documentNo = await _inventoryHttpRepository.CreateSalesOrder(saleOrder);
                inventoryDocumentNos.Add(documentNo);

                _logger.Information($"End: Sale Item No: {cartItem.ItemNo} " +
                                    $"- Quantity: {cartItem.Quantity} - Document No: {documentNo}");
            }

            return await _basketHttpRepository.DeleteBasket(userName);
        }
        catch (Exception e)
        {
            // Rollback checkout order
            _logger.Error(e.Message);
            await RollBackCheckoutOrder(userName, addedOrder.Id, inventoryDocumentNos);
            return false;
        }
    }

    public async Task RollBackCheckoutOrder(string userName, long orderId, List<string> inventoryDocumentNo)
    {
    }
}