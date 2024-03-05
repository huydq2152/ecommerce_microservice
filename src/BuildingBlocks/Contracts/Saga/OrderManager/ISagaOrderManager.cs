namespace Contracts.Saga.OrderManager;

public interface ISagaOrderManager<in TInput, out TOutput>
    where TInput : class
    where TOutput : class
{
    public TOutput CreateOrder(TInput input);

    public TOutput RollBackOrder(string userName, string documentNo, long orderId);
}