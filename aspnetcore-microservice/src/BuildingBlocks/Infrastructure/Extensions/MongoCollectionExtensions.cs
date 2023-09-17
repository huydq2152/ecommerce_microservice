using Infrastructure.Common.Models;
using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Extensions;

public static class MongoCollectionExtensions
{
    public static Task<PagedList<TDestination>> PaginatedListAsync<TDestination>(
        this IMongoCollection<TDestination> collection,
        FilterDefinition<TDestination> filter,
        int currentPage, int pageSize)
        where TDestination : class
    {
        return PagedList<TDestination>.ToPagedList(collection, filter, currentPage, pageSize);
    }
}