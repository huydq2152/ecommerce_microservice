using AutoMapper;
using MediatR;
using Ordering.Application.Common.Exception;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResult<OrderDto>>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderCommandHandler(ILogger logger, IMapper mapper, IOrderRepository orderRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    private const string MethodName = "UpdateOrderCommandHandler";

    public async Task<ApiResult<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderEntity = await _orderRepository.GetByIdAsync(request.Id);
        if (orderEntity == null) throw new NotFoundException(nameof(Domain.Entities.Order), request.Id);

        _logger.Information($"BEGIN: {MethodName} - Order: {request.Id}");

        orderEntity = _mapper.Map(request, orderEntity);
        await _orderRepository.UpdateOrderAsync(orderEntity);
        await _orderRepository.SaveChangeAsync();

        _logger.Information($"Order {request.Id} was successfully updated.");

        var result = _mapper.Map<OrderDto>(orderEntity);
        return new ApiSuccessResult<OrderDto>(result);
    }
}