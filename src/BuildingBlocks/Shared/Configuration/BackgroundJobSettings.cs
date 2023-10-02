namespace Shared.Configuration;

public class BackgroundJobSettings
{
    public string HangfireUrl { get; set; }
    public string CheckoutUrl { get; set; }
    public string BasketUrl { get; set; }
    public string ScheduleJobUrl { get; set; }
}