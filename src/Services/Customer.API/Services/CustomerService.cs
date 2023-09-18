using AutoMapper;
using Customer.API.Repositories.Interface;
using Customer.API.Services.Interface;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<IResult> GetCustomerByUserNameAsync(string userName)
    {
        var customer = await _customerRepository.GetCustomerByUserNameAsync(userName);
        var result = _mapper.Map<CustomerDto>(customer);
        return Results.Ok(result);
    }
    
    public async Task<IResult> GetCustomersAsync()
    {
        var customers = await _customerRepository.GetCustomersAsync();
        var result = _mapper.Map<List<CustomerDto>>(customers);
        return Results.Ok(result);
    }
}