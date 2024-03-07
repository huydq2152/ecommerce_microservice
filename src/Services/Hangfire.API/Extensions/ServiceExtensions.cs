using Contracts.Configurations;
using Contracts.ScheduleJobs;
using Contracts.Services;
using Hangfire.API.Services;
using Hangfire.API.Services.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.Extensions;
using Infrastructure.ScheduleJobs;
using Infrastructure.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configuration;

namespace Hangfire.API.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        if (hangfireSettings == null) throw new ArgumentNullException(nameof(hangfireSettings));
        services.AddSingleton(hangfireSettings);
        
        var emailSettings = configuration.GetSection(nameof(SmtpEmailSetting)).Get<SmtpEmailSetting>();
        if (emailSettings == null) throw new ArgumentNullException(nameof(emailSettings));
        services.AddSingleton(emailSettings);
        
        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IScheduleJobService,HangfireService>()
            .AddScoped<ISmtpEmailService, SmtpEmailService>()
            .AddScoped<IBackgroundJobService, BackgroundJobService>();

        return services;
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var hangfireSettings = services.GetOption<HangfireSettings>(nameof(HangfireSettings));
        services.AddHealthChecks()
            .AddMongoDb(hangfireSettings.Storage.ConnectionString,
                name: "MongoDB Health",
                failureStatus: HealthStatus.Degraded);
    }
}