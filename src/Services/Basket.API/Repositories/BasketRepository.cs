using Basket.API.Entities;
using Basket.API.Repositories.Interface;
using Basket.API.Services;
using Basket.API.Services.Interface;
using Contracts.Common.Interfaces;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.ScheduleJob;
using ILogger = Serilog.ILogger;

namespace Basket.API.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _redisCacheService;
    private readonly ISerializeService _serializeService;
    private readonly ILogger _logger;
    private readonly BackgroundJobHttpService _backgroundJobHttpService;
    private readonly IEmailTemplateService _emailTemplateService;

    public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger,
        BackgroundJobHttpService backgroundJobHttpService, IEmailTemplateService emailTemplateService)
    {
        _redisCacheService = redisCacheService;
        _serializeService = serializeService;
        _logger = logger;
        _backgroundJobHttpService = backgroundJobHttpService;
        _emailTemplateService = emailTemplateService;
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

        try
        {
            await TriggerSendEmailReminderCheckout(cart);
        }
        catch (Exception e)
        {
            _logger.Error(e.Message);
        }

        var result = await GetBasketByUserName(cart.UserName);
        return result;
    }

    private async Task TriggerSendEmailReminderCheckout(Cart cart)
    {
        var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail(cart.UserName);
        var model = new ReminderCheckoutOrderDto("quanghuyitbkhn@gmail.com", "Reminder Checkout", emailTemplate,
            DateTimeOffset.UtcNow.AddSeconds(30));

        const string uri = "/api/scheduled-jobs/send-email-reminder-checkout-order";
        var response = await _backgroundJobHttpService.Client.PostAsJson(uri, model);

        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            var jobId = await response.ReadContentAs<string>();
            if (!string.IsNullOrEmpty(jobId))
            {
                cart.JobId = jobId;
                await _redisCacheService.SetStringAsync(cart.UserName, _serializeService.Serialize(cart));
            }
        }
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