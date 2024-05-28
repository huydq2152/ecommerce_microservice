using Contracts.Common.Interfaces;
using IdentityServer.Infrastructure.Repositories;
using IdentityServer.Presentation;
using IdentityServer.Services.EmailService;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace IdentityServer.Extensions;

internal static class HostingExtensions
{
    internal static void AddAppConfigurations(this ConfigureHostBuilder host)
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        });
    }

    public static void ConfigureSerilog(this ConfigureHostBuilder host)
    {
        host.UseSerilog((context, configuration) =>
        {
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var username = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
            var password = context.Configuration.GetValue<string>("ElasticConfiguration:Password");
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");

            if (string.IsNullOrEmpty(elasticUri))
                throw new Exception("ElasticConfiguration Uri is not configured.");

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console().ReadFrom.Configuration(context.Configuration)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat =
                            $"{applicationName}-logs-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1,
                        ModifyConnectionSettings = x => x.BasicAuthentication(username, password)
                    })
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("Application", applicationName)
                .ReadFrom.Configuration(context.Configuration);
        });
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        // uncomment if you want to add a UI
        builder.Services.AddRazorPages();

        builder.Services.AddConfigurationSettings(builder.Configuration);
        builder.Services.AddScoped<IEmailSender, SmtpEmailService>();

        // fix can't login with identity server when Username and Password is correct
        builder.Services.ConfigureCookiePolicy();

        // configure cors
        builder.Services.ConfigureCors();

        // need config for identity before identity server
        builder.Services.ConfigureIdentity(builder.Configuration);
        builder.Services.ConfigureIdentityServer(builder.Configuration);

        builder.Services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        builder.Services.AddTransient(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
        builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
        builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
        
        builder.Services.AddControllers(config =>
        {
            config.RespectBrowserAcceptHeader = true;
            config.ReturnHttpNotAcceptable = true;
        }).AddApplicationPart(typeof(AssemblyReference).Assembly);

        builder.Services.ConfigureSwagger(builder.Configuration);
        
        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // uncomment if you want to add a UI
        app.UseStaticFiles();

        app.UseCors("CorsPolicy");
        app.UseSwagger();
        app.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Server API"));
        app.UseRouting();

        app.UseCookiePolicy();
        app.UseIdentityServer();

        // uncomment if you want to add a UI
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute().RequireAuthorization();
            endpoints.MapRazorPages().RequireAuthorization();
        });

        return app;
    }
}