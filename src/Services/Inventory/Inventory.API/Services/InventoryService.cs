using AutoMapper;
using Infrastructure.Common.Models;
using Infrastructure.Common.Repositories;
using Infrastructure.Extensions;
using Inventory.API.Entities;
using Inventory.API.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configuration;
using Shared.DTOs.Inventory;

namespace Inventory.API.Services;

public class InventoryService : MongoDbRepository<InventoryEntry>, IInventoryService
{
    private readonly IMapper _mapper;

    public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNoAsync(string itemNo)
    {
        var entities = await FindAll().Find(o => o.ItemNo.Equals(itemNo)).ToListAsync();
        var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
        return result;
    }

    public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPagingAsync(GetInventoryPagingQuery query)
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

    public async Task<InventoryEntryDto> PurchaseItemAsync(PurchaseProductDto model)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            ItemNo = model.ItemNo,
            Quantity = model.Quantity,
            DocumentType = model.EDocumentType
        };
        await CreateAsync(itemToAdd);
        var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
        return result;
    }

    public async Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model)
    {
        var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
        {
            DocumentType = model.DocumentType,
            ItemNo = model.ItemNo,
            ExternalDocumentNo = model.ExternalDocumentNo,
            Quantity = model.Quantity * -1
        };
        await CreateAsync(itemToAdd);
        var result = _mapper.Map<InventoryEntryDto>(itemToAdd);
        return result;
    }

    public async Task DeleteByDocumentNoAsync(string documentNo)
    {
        var filter = Builders<InventoryEntry>.Filter.Eq(s => s.DocumentNo, documentNo);
        await Collection.DeleteOneAsync(filter);
    }

    public async Task<string> SalesOrderAsync(SalesOrderDto model)
    {
        var documentNo = Guid.NewGuid().ToString();
        foreach (var saleItem in model.SaleItems)
        {
            var itemToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                DocumentNo = documentNo,
                DocumentType = saleItem.DocumentType,
                ItemNo = saleItem.ItemNo,
                ExternalDocumentNo = model.OrderNo,
                Quantity = saleItem.Quantity * -1
            };
            await CreateAsync(itemToAdd);
        }
        return documentNo;
    }
}