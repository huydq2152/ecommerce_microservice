using Basket.API.Entities;
using Basket.API.Repositories.Interface;
using Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;

    public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger)
    {
        _redisCacheService = redisCacheService;
        _serializeService = serializeService;
        _logger = logger;
    }

    public async Task<Cart?> GetBasketByUserName(string userName)
    {
        _logger.Information($"BEGIN: GetBasketByUserName {userName}");
        var basket = await _redisCacheService.GetStringAsync(userName);
        _logger.Information($"END: GetBasketByUserName {userName}");
        if (string.IsNullOrEmpty(basket)) return null;
        var result = _serializeService.Deserialize<Cart>(basket);
        return result;
    }

    public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions? options = null)
    {
        _logger.Information($"BEGIN: UpdateBasket for {cart.UserName}");
        if (options != null)
        {
            await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart), options);
        }
        else
        {
            await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
        }
        _logger.Information($"END: UpdateBasket for {cart.UserName}");

        var result = await GetBasketByUserName(cart.UserName);
        return result;
    }

    public async Task<bool> DeleteBasketByUserName(string userName)
    {
        try
        {
            _logger.Information($"BEGIN: DeleteBasketFromUserName {userName}");
            await _redisCacheService.RemoveAsync(userName);
            _logger.Information($"END: DeleteBasketFromUserName {userName}");
            return true;
        }
        catch (Exception e)
        {
            _logger.Error($"DeleteBasketByUserName: {e.Message}");
            throw;
        }
    }
}