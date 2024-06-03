using Infrastructure.Identity;
using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    builder.Services.ConfigureAuthenticationHandler();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        // app.UseSwagger();
        // app.UseSwaggerUI(
        //     c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{builder.Environment.ApplicationName} v1"));
    }

    app.UseCors("CorsPolicy");
    app.UseMiddleware<ErrorWrappingMiddleware>();
    // Ocelot just get token and give it to api endpoints authentication and authorization
    // app.UseAuthentication();
    app.UseRouting();
    // app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/",  context =>
        {
            // await context.Response.WriteAsync($"Hello, this is {builder.Environment.ApplicationName}");
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });
    
    app.UseSwaggerForOcelotUI(
        opt =>
        {
            opt.PathToSwaggerGenerator = "/swagger/docs";
            opt.OAuthClientId("microservices_swagger");
            opt.DisplayRequestDuration();
        });

    await app.UseOcelot();

    app.Run();
}
catch (Exception ex)
{
    var type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shutdown basket api success");
    Log.CloseAndFlush();
}