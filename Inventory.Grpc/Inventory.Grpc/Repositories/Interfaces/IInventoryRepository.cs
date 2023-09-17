using Contracts.Domains.Interface;
using Inventory.Grpc.Entities;

namespace Inventory.Grpc.Repositories.Interfaces;

public interface IInventoryRepository: IMongoDbRepositoryBase<InventoryEntry>
{
    Task<int> GetStockQuantity(string itemNo);
}