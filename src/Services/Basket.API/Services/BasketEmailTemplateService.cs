﻿using Basket.API.Services.Interface;
using Shared.Configuration;

namespace Basket.API.Services;

public class BasketEmailTemplateService: EmailTemplateService, IEmailTemplateService
{
    public BasketEmailTemplateService(BackgroundJobSettings backgroundJobSettings) : base(backgroundJobSettings)
    {
    }
    
    public string GenerateReminderCheckoutOrderEmail(string userName, string checkoutUrl)
    {
        var url = $"{_backgroundJobSettings.ApiGwUrl}/{checkoutUrl}/{userName}";
        var emailText = ReadEmailTemplateContent("reminder-checkout-order");
        var emailReplaceText = emailText.Replace("[userName]", userName).Replace("[checkoutUrl]", url);

        return emailReplaceText;
    }
}