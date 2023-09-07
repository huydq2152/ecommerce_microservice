using Customer.API.Services.Interface;

namespace Customer.API.Controller;

public static class CustomerController
{
    public static void MapCustomerAPI(this WebApplication app)
    {
        app.MapGet("/", () => "Welcome to customer API");
        app.MapGet("/api/customers", async (ICustomerService customerService) => await customerService.GetCustomersAsync());
        app.MapGet("/api/customers/{userName}",
            async (ICustomerService customerService, string userName) =>
            {
                var customer = await customerService.GetCustomerByUserNameAsync(userName);
                return customer != null ? Results.Ok(customer) : Results.NotFound();
            }
        );
    }
}