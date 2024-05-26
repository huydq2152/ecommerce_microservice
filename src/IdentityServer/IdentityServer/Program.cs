using IdentityServer.Extensions;
using IdentityServer.Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    builder.Host.ConfigureSerilog();

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();
    
    var identityConnectionString = builder.Configuration.GetConnectionString("IdentitySqlConnection");
    if(identityConnectionString == null)
    {
        throw new ArgumentNullException("IdentitySqlConnection is not configured.");
    }
    SeedUserData.EnsureSeedData(identityConnectionString);
    
    app.MigrateDatabase().Run();
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
    Log.Information("Shutdown identity server success");
    Log.CloseAndFlush();
}