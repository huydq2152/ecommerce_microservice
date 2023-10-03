using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
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

    public async Task<Order> GetOrderByDocumentNo(string documentNo)
    {
        var result = await FindByCondition(o => o.DocumentNo.ToString().Equals(documentNo)).FirstOrDefaultAsync();
        return result ?? new Order();
    }

    public void CreateOrder(Order order)
    {
        Create(order);
    }

    public async Task<Order> UpdateOrderAsync(Order order)
    {
        await UpdateAsync(order);
        return order;
    }

    public void DeleteOrder(Order order)
    {
        Delete(order);
    }
}