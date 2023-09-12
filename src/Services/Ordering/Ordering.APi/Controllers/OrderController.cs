using System.ComponentModel.DataAnnotations;
using System.Net;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Order;
using Shared.Services.Email;

namespace Ordering.API.Controllers;

[Route("api/vi/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _emailService;

    public OrderController(IMediator mediator, ISmtpEmailService emailService)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _emailService = emailService;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{userName}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string userName)
    {
        var query = new GetOrderQuery(userName);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("test-email")]
    public async Task<IActionResult> TestEmail()
    {
        var request = new MailRequest()
        {
            Body = "hello",
            Subject = "tedu-microservice test email",
            ToAddress = "quanghuyitbkhn@gmail.com"
        };
        await _emailService.SendEmailAsync(request);

        return Ok();
    }
}