using Contracts.Domains.Interface;
using Infrastructure.Common.Models;
using Inventory.API.Entities;
using Shared.DTOs.Inventory;

namespace Inventory.API.Services.Interfaces;

public interface IInventoryService : IMongoDbRepositoryBase<InventoryEntry>
{
    Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo);
    Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query);
    Task<InventoryEntryDto> GetByIdAsync(string id);
    Task<InventoryEntryDto> PurchaseItemAsync(PurchaseProductDto model);
    Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model);
    Task DeleteByDocumentNoAsync(string documentNo);
    Task<string> SalesOrderAsync(SalesOrderDto model);
}