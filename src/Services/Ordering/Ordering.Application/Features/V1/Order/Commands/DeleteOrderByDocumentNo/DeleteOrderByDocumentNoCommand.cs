using MediatR;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.DeleteOrderByDocumentNo;

public class DeleteOrderByDocumentNoCommand: IRequest<ApiResult<bool>>
{
    public string DocumentNo { get; set; }

    public DeleteOrderByDocumentNoCommand(string documentNo)
    {
        DocumentNo = documentNo;
    }
}