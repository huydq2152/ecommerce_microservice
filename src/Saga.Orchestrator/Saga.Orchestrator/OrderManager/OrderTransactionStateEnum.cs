namespace Saga.Orchestrator.OrderManager;

public enum OrderTransactionStateEnum
{
    NotStarted,
    BasketGot,
    BasketGetFailed,
    BasketDeleted,
    OrderCreated,
    OrderCreatedFailed,
    OrderDeleted,
    OrderDeletedFailed,
    OrderGot,
    OrderGetFailed,
    InventoryUpdated,
    InventoryUpdateFailed,
    RollbackInventory,
    InventoryRollback,
}