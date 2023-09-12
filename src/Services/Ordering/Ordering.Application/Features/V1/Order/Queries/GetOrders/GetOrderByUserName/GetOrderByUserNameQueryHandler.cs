using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order;

public class GetOrderByUserNameQueryHandler : IRequestHandler<GetOrderByUserNameQuery, ApiResult<List<OrderDto>>>
{
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger _logger;

    public GetOrderByUserNameQueryHandler(IMapper mapper, IOrderRepository orderRepository, ILogger logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger;
    }

    private const string MethodName = "GetOrdersQueryHandler";
    
    public async Task<ApiResult<List<OrderDto>>> Handle(GetOrderByUserNameQuery request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");

        var orderEntities = await _orderRepository.GetOrdersByUserNameAsync(request.UserName);
        var orders = _mapper.Map<List<OrderDto>>(orderEntities);

        _logger.Information($"END: {MethodName} - Username: {request.UserName}");

        return new ApiSuccessResult<List<OrderDto>>(orders);
    }
}