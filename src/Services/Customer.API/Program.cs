using Common.Logging;
using Contracts.Common.Interfaces;
using Customer.API;
using Customer.API.Controller;
using Customer.API.Persistence;
using Customer.API.Repositories;
using Customer.API.Repositories.Interface;
using Customer.API.Services;
using Customer.API.Services.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(Serilogger.Configure);

Log.Information("Starting customer API up");

try
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped<ICustomerService, CustomerService>();
    
    var app = builder.Build();

    //Minimal API
    app.MapCustomerAPI();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    //app.UseHttpsRedirection(); //production only
    app.UseAuthorization();
    app.MapControllers();
    app.SeedCustomerData()
        .Run();
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
    Log.Information("Shutdown customer api success");
    Log.CloseAndFlush();
}