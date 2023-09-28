using Basket.API.Services.Interface;

namespace Basket.API.Services;

public class BasketEmailTemplateService: EmailTemplateService, IEmailTemplateService
{
    public string GenerateReminderCheckoutOrderEmail(string email, string userName)
    {
        var checkoutUrl = "http://localhost:5001/baskets/checkout";
        var emailText = ReadEmailTemplateContent("reminder-checkout-order");
        var emailReplaceText = emailText.Replace("[userName]", userName).Replace("[checkoutUrl]", checkoutUrl);

        return emailReplaceText;
    }
}