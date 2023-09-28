namespace Basket.API.Services.Interface;

public interface IEmailTemplateService
{
    string GenerateReminderCheckoutOrderEmail(string userName, string checkoutUrl = "baskets/checkout");
}