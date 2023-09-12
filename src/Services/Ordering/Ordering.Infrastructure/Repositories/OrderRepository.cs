using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories;

public class OrderRepository : RepositoryBase<Order, long, OrderContext>, IOrderRepository
{
    public OrderRepository(OrderContext dbContext, IUnitOfWork<OrderContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName)
    {
        var result = await FindByCondition(o => o.UserName.Equals(userName)).ToListAsync();
        return result;
    }

    public async void CreateOrder(Order order)
    {
        await CreateAsync(order);
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        await UpdateAsync(order);
        return order;
    }

    public async void DeleteOrder(Order order)
    {
        await DeleteAsync(order);
    }
}