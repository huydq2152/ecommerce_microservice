using Shared.Configuration;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        if (hangfireSettings == null) throw new ArgumentNullException(nameof(hangfireSettings));
        services.AddSingleton(hangfireSettings);
        
        return services;
    }
    
    
}