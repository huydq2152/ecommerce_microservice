using Shared.DTOs.Cart;
using Shared.DTOs.Inventory;

namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IInventoryHttpRepository
{
    Task<string> CreateSalesOrder(SalesProductDto model);
    Task<string> CreateOrderSale(string orderNo, SalesOrderDto model);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}