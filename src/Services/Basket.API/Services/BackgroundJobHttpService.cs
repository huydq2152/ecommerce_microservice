using Infrastructure.Extensions;
using Shared.Configuration;
using Shared.DTOs.ScheduleJob;

namespace Basket.API.Services;

public class BackgroundJobHttpService
{
    private readonly HttpClient _client;

    private readonly string _scheduleJobUrl;

    public BackgroundJobHttpService(HttpClient client, BackgroundJobSettings backgroundJobSettings)
    {
        client.BaseAddress = new Uri(backgroundJobSettings.HangfireUrl);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Accept", "application/json");

        _client = client;
        _scheduleJobUrl = backgroundJobSettings.ScheduleJobUrl;
    }
    
    public async Task<string?> SendEmailReminderCheckout(ReminderCheckoutOrderDto model )
    {
        var uri = $"{_scheduleJobUrl}/send-email-reminder-checkout-order";
        var response = await _client.PostAsJson(uri, model);
        string jobId = null;
        if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
        {
            jobId = await response.ReadContentAs<string>();
        }
        return jobId;
    }
    
    public async Task DeleteReminderCheckoutOrder(string jobId)
    {
        var uri = $"{_scheduleJobUrl}/delete/jobId/{jobId}";
        await _client.DeleteAsync(uri);
    }
}