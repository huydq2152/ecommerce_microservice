﻿using System.Linq.Expressions;
using Inventory.API.Entities.Abstraction;
using Inventory.API.Extensions;
using MongoDB.Driver;

namespace Inventory.API.Repositories.Abstraction;

public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
{
    private IMongoDatabase Database { get; }

    public MongoDbRepository(IMongoClient client, DatabaseSettings settings)
    {
        Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
        => Database.WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName());

    protected virtual IMongoCollection<T> Collection => Database.GetCollection<T>(GetCollectionName());

    public Task CreateAsync(T entity) => Collection.InsertOneAsync(entity);

    public Task UpdateAsync(T entity)
    {
        Expression<Func<T, string>> func = mongoEntity => mongoEntity.Id;
        var value = (string)entity.GetType().GetProperty(func.Body.ToString().Split(".")[1])?.GetValue(entity, null)!;
        var filter = Builders<T>.Filter.Eq(func, value);
        return Collection.ReplaceOneAsync(filter, entity);
    }

    public Task DeleteAsync(string id) => Collection.DeleteOneAsync(o => o.Id.Equals(id));

    private static string GetCollectionName()
    {
        var collectionName = (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as
            BsonCollectionAttribute)?.CollectionName;
        return collectionName ?? "";
    }
}