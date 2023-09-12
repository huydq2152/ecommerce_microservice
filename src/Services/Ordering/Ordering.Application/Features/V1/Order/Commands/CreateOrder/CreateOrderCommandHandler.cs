using AutoMapper;
using MediatR;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Serilog;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Order.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IOrderRepository _orderRepository;

    public CreateOrderCommandHandler(ILogger logger, IMapper mapper, IOrderRepository orderRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    private const string MethodName = "CreateOrderCommandHandler";
    
    public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.Information($"BEGIN: {MethodName} - Username: {request.UserName}");
        var orderEntity = _mapper.Map<Domain.Entities.Order>(request);
        _orderRepository.CreateOrder(orderEntity);
        await _orderRepository.SaveChangeAsync();
        
        _logger.Information($"END: {MethodName} - Username: {request.UserName}");
        return new ApiSuccessResult<long>(orderEntity.Id);
    }
}