using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext,
        unitOfWork)
    {
    }

    public async Task<Entities.Customer> GetCustomerByUserNameAsync(string userName) =>
        await FindByCondition(o => o.UserName.Equals(userName)).SingleOrDefaultAsync();

    public async Task<IEnumerable<Entities.Customer>> GetCustomersAsync() => await FindAll().ToListAsync();
}