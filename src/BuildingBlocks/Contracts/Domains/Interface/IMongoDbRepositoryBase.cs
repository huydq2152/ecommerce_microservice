using MongoDB.Driver;

namespace Contracts.Domains.Interface;

public interface IMongoDbRepositoryBase<T> where T: MongoEntity
{
    IMongoCollection<T> FindAll(ReadPreference? readPreference);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}