using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Serilog;

namespace Ordering.Infrastructure.Persistence;

public class OrderContextSeed
{
    private readonly ILogger _logger;
    private readonly OrderContext _context;

    public OrderContextSeed(ILogger logger, OrderContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception e)
        {
            _logger.Error("An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
           _logger.Error(e,"An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        if (!_context.Orders.Any())
        {
            await _context.Orders.AddRangeAsync(
                new Order()
                {
                    UserName = "customer1", FirstName = "customer1", LastName = "customer2",
                    EmailAddress = "customer1@local.com",
                    ShippingAddress = "Wollongong", InvoiceAddress = "Australia", TotalPrice = 250
                },
                new Order()
                {
                    UserName = "customer2", FirstName = "customer2", LastName = "customer2",
                    EmailAddress = "customer2@local.com",
                    ShippingAddress = "Alex", InvoiceAddress = "Australia", TotalPrice = 100
                });
        }
    }
}