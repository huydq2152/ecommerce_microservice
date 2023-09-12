using AutoMapper;
using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Application.Features.V1.Order.Common;
using Shared.DTOs.Order;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.CreateOrder;

public class CreateOrderCommand: CreateOrUpdateOrderCommand, IRequest<ApiResult<long>>, IMapFrom<Domain.Entities.Order>
{
    public string UserName { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrderDto, CreateOrderCommand>();
        profile.CreateMap<CreateOrderCommand, Domain.Entities.Order>();
    }
}