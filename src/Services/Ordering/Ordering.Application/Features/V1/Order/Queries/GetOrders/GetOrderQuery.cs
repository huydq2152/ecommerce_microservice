using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order;

public class GetOrderQuery : IRequest<ApiResult<List<OrderDto>>>
{
    public string UserName { get; set; }

    public GetOrderQuery(string userName)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
    }
}