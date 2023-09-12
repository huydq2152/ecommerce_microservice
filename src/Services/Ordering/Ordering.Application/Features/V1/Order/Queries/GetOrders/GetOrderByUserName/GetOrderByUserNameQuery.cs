using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order;

public class GetOrderByUserNameQuery : IRequest<ApiResult<List<OrderDto>>>
{
    public string UserName { get; set; }

    public GetOrderByUserNameQuery(string userName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
}