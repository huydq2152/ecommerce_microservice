using MediatR;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest<ApiResult<bool>>
{
    public long Id { get; set; }

    public DeleteOrderCommand(long id)
    {
        Id = id;
    }
}