using Common.Logging;
using Contracts.Common.Interfaces;
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

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    builder.Services.AddDbContext<CustomerContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString));
    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>()
        .AddScoped<ICustomerService, CustomerService>()
        .AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryBaseAsync<,,>))
        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

    var app = builder.Build();

    //Minimal API
    app.MapGet("/", () => "Welcome to customer API");
    app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomersAsync());
    app.MapGet("/api/customers/{username}",
        async (ICustomerService customerService, string userName) =>
        {
            var customer = await customerService.GetCustomerByUserNameAsync(userName);
            return customer != null ? Results.Ok(customer) : Results.NotFound();
        }
    );

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
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