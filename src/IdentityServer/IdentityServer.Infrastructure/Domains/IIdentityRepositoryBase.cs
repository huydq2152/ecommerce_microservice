using System.Data;
using Contracts.Common.Interfaces;
using Contracts.Domains;

namespace IdentityServer.Infrastructure.Domains;

public interface IIdentityRepositoryBase<T, K> : IRepositoryBase<T, K> where T : EntityBase<K>
{
    Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeout) where TModel : EntityBase<K>;

    Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeout) where TModel : EntityBase<K>;

    Task<TModel> QuerySingleAsync<TModel>(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeout) where TModel : EntityBase<K>;

    Task<int> ExecuteAsync(string sql, object? param,
        CommandType? commandType, IDbTransaction? transaction, int? commandTimeout);
}