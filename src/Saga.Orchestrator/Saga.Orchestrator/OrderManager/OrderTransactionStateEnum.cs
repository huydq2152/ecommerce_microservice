namespace Saga.Orchestrator.OrderManager;

public enum OrderTransactionStateEnum
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    OrderCreated,
    OrderCreatedFailed,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    InventoryRollBacked,
}