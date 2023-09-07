namespace Customer.API.Services.Interface;

public interface ICustomerService
{
    Task<IResult> GetCustomerByUserNameAsync(string userName);
    Task<IResult> GetCustomersAsync();
}