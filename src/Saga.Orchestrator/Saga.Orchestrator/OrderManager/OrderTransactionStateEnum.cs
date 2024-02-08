namespace Saga.Orchestrator.OrderManager;

public enum OrderTransactionStateEnum
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    OrderCreated,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    InventoryRollBacked,
}