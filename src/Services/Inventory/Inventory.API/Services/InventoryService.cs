using AutoMapper;
using Infrastructure.Common.Models;
using Infrastructure.Extensions;
using Inventory.API.Entities;
using Inventory.API.Extensions;
using Inventory.API.Repositories.Abstraction;
using Inventory.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.DTOs.Inventory;

namespace Inventory.API.Services;

public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
{
    private readonly IMapper _mapper;

    public InventoryService(IMongoClient client, DatabaseSettings settings, IMapper mapper) : base(client, settings)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
    {
        var entities = await FindAll().Find(o => o.ItemNo.Equals(itemNo)).ToListAsync();
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return result;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
    {
        var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
        var filterItemNo = Builders<InventoryEntry>.Filter.Eq(o => o.ItemNo, query.ItemNo());
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(o => o.DocumentNo, query.SearchTerm);
        }

        var andFilter = filterItemNo & filterSearchTerm;
        var pagedList = await Collection.PaginatedListAsync(andFilter, query.CurrentPage, query.PageSize);
        var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pagedList);
        var result = new PagedList<InventoryEntryDto>(items, pagedList.GetMetaData().TotalItems, query.CurrentPage,
            query.PageSize);
        return result;
    }

    public async Task<InventoryEntryDto> GetByIdAsync(string id)
    {
        var filter = Builders<InventoryEntry>.Filter.Eq(o => o.Id, id);
        var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return result;
    }

    public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
    {
        var entity = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = model.ItemNo,
            Quantity = model.Quantity,
            DocumentType = model.EDocumentType,
        };
        await CreateAsync(entity);
        var result = _mapper.Map<InventoryEntryDto>(entity);
        return result;
    }
}