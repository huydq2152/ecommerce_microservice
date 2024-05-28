using System.Data;
using Contracts.Common.Interfaces;
using Contracts.Domains;
using Dapper;
using IdentityServer.Infrastructure.Exceptions;
using IdentityServer.Persistence;
using Infrastructure.Common.Repositories;

namespace IdentityServer.Infrastructure.Domains;

public class IdentityRepositoryBase<T, K> : RepositoryBase<T, K, IdentityContext>, IIdentityRepositoryBase<T, K>
    where T : EntityBase<K>
{
    private readonly IdentityContext _dbContext;
    private readonly IUnitOfWork<IdentityContext> _unitOfWork;

    public IdentityRepositoryBase(IdentityContext dbContext, IUnitOfWork<IdentityContext> unitOfWork) : base(dbContext,
        unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<TModel>> QueryAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<K>
    {
        return (await _dbContext.Connection.QueryAsync<TModel>(sql, param,
            transaction, 30, CommandType.StoredProcedure)).AsList();
    }

    public async Task<TModel> QueryFirstOrDefaultAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<K>
    {
        var entity =
            await _dbContext.Connection.QueryFirstOrDefaultAsync<TModel>(sql, param, transaction, commandTimeout,
                commandType);
        if (entity == null) throw new EntityNotFoundException();
        return entity;
    }

    public async Task<TModel> QuerySingleAsync<TModel>(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
        where TModel : EntityBase<K>
    {
        return await _dbContext.Connection.QuerySingleAsync<TModel>(sql, param, transaction, commandTimeout,
            commandType);
    }

    public async Task<int> ExecuteAsync(string sql, object? param,
        CommandType? commandType = CommandType.StoredProcedure, IDbTransaction? transaction = null,
        int? commandTimeout = 30)
    {
        return await _dbContext.Connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }
}