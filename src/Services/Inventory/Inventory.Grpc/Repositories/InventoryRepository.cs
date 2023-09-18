using Infrastructure.Common.Repositories;
using Inventory.Grpc.Entities;
using Inventory.Grpc.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Configuration;

namespace Inventory.Grpc.Repositories;

public class InventoryRepository : MongoDbRepository<InventoryEntry>, IInventoryRepository
{
    public InventoryRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
    {
    }

    public Task<int> GetStockQuantity(string itemNo)
        => Task.FromResult(Collection.AsQueryable()
            .Where(o => o.ItemNo.Equals(itemNo))
            .Sum(o => o.Quantity));
}