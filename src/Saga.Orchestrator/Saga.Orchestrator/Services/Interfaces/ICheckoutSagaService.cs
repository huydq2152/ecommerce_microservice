using Shared.DTOs.Basket;

namespace Saga.Orchestrator.Services.Interfaces;

public interface ICheckoutSagaService
{
    Task<bool> CheckoutOrder(string userName, BasketCheckoutDto basketCheckout);
}