using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Queries.GetOrders;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, ApiResult<List<OrderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public GetOrderQueryHandler(IMapper mapper, IOrderRepository orderRepository)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var orderEntities = await _orderRepository.GetOrdersByUserNameAsync(request.UserName);
        var orders = _mapper.Map<List<OrderDto>>(orderEntities);

        return new ApiSuccessResult<List<OrderDto>>(orders);
    }
}