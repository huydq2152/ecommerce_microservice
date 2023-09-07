using Customer.API.Repositories.Interface;
using Customer.API.Services.Interface;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IResult> GetCustomerByUserNameAsync(string userName) =>
        Results.Ok(await _customerRepository.GetCustomerByUserNameAsync(userName));

    public async Task<IResult> GetCustomersAsync() => Results.Ok(await _customerRepository.GetCustomersAsync());
}