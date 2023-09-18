using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryQueryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext) : base(dbContext)
    {
    }

    public async Task<Entities.Customer> GetCustomerByUserNameAsync(string userName)
    {
        var result = await FindByCondition(o => o.UserName.Equals(userName)).SingleOrDefaultAsync();
        return result;
    }
        

    public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync() => await FindAll().ToListAsync();
}