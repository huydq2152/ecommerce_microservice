using Contracts.Configurations;
using Infrastructure.Configurations;

namespace Ordering.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        var emailSetting = configuration.GetSection(nameof(SmtpEmailSetting)).Get<SmtpEmailSetting>();
        if (emailSetting == null) throw new ArgumentNullException("smtp email setting is not configured");
        services.AddSingleton(emailSetting);

        return services;
    }
}