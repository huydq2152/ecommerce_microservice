using Inventory.API.Entities;
using Inventory.API.Repositories.Abstraction;
using Shared.DTOs.Inventory;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetByIdAsync(string id);
    Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model);
}