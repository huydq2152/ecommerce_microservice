using Common.Logging;
using HealthChecks.UI.Client;
using Inventory.Grpc.Extensions;
using Inventory.Grpc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.ConfigureMongoDbClient();
    builder.Services.AddInfrastructureServices();
    builder.Services.AddGrpc();
    builder.Services.ConfigureHealthChecks();

    var app = builder.Build();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        // health checks
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapGrpcHealthChecksService();
        endpoints.MapGrpcService<InventoryService>();
        endpoints.MapGrpcReflectionService();

        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Inventory.GRPC - Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
        });
    });
    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shutdown grpc success");
    Log.CloseAndFlush();
}