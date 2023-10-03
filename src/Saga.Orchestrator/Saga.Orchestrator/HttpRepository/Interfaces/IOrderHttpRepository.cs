using Shared.DTOs.Order;

namespace Saga.Orchestrator.HttpRepository.Interfaces;

public interface IOrderHttpRepository
{
    Task<long> CreateOrder(CreateOrderDto order);
    Task<OrderDto> GetOrder(long id);
    Task<bool> DeleteOrder(long id);
    Task<bool> DeleteOrderByDocumentNo(string documentNo);
}