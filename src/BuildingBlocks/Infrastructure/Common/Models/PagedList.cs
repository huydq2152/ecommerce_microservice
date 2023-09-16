using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Common.Models;

public class PagedList<T>: List<T>
{
    public PagedList(IEnumerable<T> items, long totalItems, int currentPage, int pageSize)
    {
        _metaData = new MetaData()
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
        AddRange(items);
    }

    private MetaData _metaData { get; }

    public MetaData GetMetaData()
    {
        return _metaData;
    }
    
    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int currentPage, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, currentPage, pageSize);
    }
    
    public static async Task<PagedList<T>> ToPagedList(IMongoCollection<T> source, FilterDefinition<T> filter, int currentPage, int pageSize)
    {
        var count = await source.Find(filter).CountDocumentsAsync();
        var items = await source.Find(filter)
            .Skip((currentPage - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, currentPage, pageSize);
    }
}