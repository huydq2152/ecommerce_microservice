using AutoMapper;
using Infrastructure.Mapping;
using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Order.Common;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.UpdateOrder;

public class UpdateOrderCommand : CreateOrUpdateOrderCommand, IRequest<ApiResult<OrderDto>>,
    IMapFrom<Domain.Entities.Order>
{
    public long Id { get; set; }

    public void SetId(long id)
    {
        Id = id;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrUpdateOrderCommand, Domain.Entities.Order>()
            .ForMember(dest => dest.Status, opts => opts.Ignore())
            .IgnoreAllNonExisting();
    }
}