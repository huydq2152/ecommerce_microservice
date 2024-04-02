using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.V1.Order;
using Ordering.Application.Features.V1.Order.Commands.CreateOrder;
using Ordering.Application.Features.V1.Order.Commands.DeleteOrder;
using Ordering.Application.Features.V1.Order.Commands.DeleteOrderByDocumentNo;
using Ordering.Application.Features.V1.Order.Commands.UpdateOrder;
using Ordering.Application.Features.V1.Order.Queries.GetOrders.GetOrderById;
using Shared.DTOs.Order;
using Shared.SeedWork;
using Shared.Services.Email;
using OrderDto = Ordering.Application.Common.Models.OrderDto;

namespace Ordering.API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _emailService;
    private readonly IMapper _mapper;

    public OrdersController(IMediator mediator, ISmtpEmailService emailService, IMapper mapper)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailService = emailService;
        _mapper = mapper;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
        public const string GetOrder = nameof(GetOrder);
        public const string CreateOrder = nameof(CreateOrder);
        public const string UpdateOrder = nameof(UpdateOrder);
        public const string DeleteOrder = nameof(DeleteOrder);
        public const string DeleteOrderByDocumentNo = nameof(DeleteOrderByDocumentNo);
    }

    #region test email

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        var request = new MailRequest()
        {
            Body = "hello",
            Subject = "microservice test email",
            ToAddress = "quanghuyitbkhn@gmail.com"
        };
        await _emailService.SendEmailAsync(request);

        return Ok();
    }

    #endregion

    #region CRUD

    [HttpGet("userName/{userName}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string userName)
    {
        var query = new GetOrderByUserNameQuery(userName);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:long}", Name = RouteNames.GetOrder)]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDto>> GetOrderById([Required] long id)
    {
        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost(Name = RouteNames.CreateOrder)]
    [ProducesResponseType(typeof(ApiResult<long>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiResult<long>>> CreateOrder([FromBody] CreateOrderDto model)
    {
        var command = _mapper.Map<CreateOrderCommand>(model);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:long}", Name = RouteNames.UpdateOrder)]
    [ProducesResponseType(typeof(ApiResult<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<OrderDto>> UpdateOrder([Required] long id, [FromBody] UpdateOrderCommand command)
    {
        command.SetId(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:long}", Name = RouteNames.DeleteOrder)]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteOrder([Required] long id)
    {
        var command = new DeleteOrderCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }
    
    [HttpDelete("document-no/{documentNo}", Name = RouteNames.DeleteOrderByDocumentNo)]
    [ProducesResponseType(typeof(ApiResult<bool>), (int)HttpStatusCode.NoContent)]
    public async Task<ActionResult> DeleteOrderByDocumentNo(string documentNo)
    {
        var command = new DeleteOrderByDocumentNoCommand(documentNo);
        await _mediator.Send(command);
        return NoContent();
    }

    #endregion
}