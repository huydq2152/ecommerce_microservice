using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interface;
using Basket.API.Services.Interface;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Shared.DTOs.Cart;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly StockItemGrpcService _stockItemGrpcService;
    private readonly IEmailTemplateService _emailTemplateService;

    public BasketsController(IBasketRepository basketRepository, IMapper mapper, IPublishEndpoint publishEndpoint,
        StockItemGrpcService stockItemGrpcService, IEmailTemplateService emailTemplateService)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _stockItemGrpcService = stockItemGrpcService ?? throw new ArgumentNullException(nameof(stockItemGrpcService));
        _emailTemplateService = emailTemplateService;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(CartDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CartDto>> GetBasketByUserName([Required] string userName)
    {
        var cart = await _basketRepository.GetBasketByUserName(userName);
        var result = _mapper.Map<CartDto>(cart);
        return Ok(result ?? new CartDto());
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(CartDto), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CartDto>> UpdateBasket([FromBody] CartDto model)
    {
        // communicate with inventory.grpc to check quantity available of products
        foreach (var item in model.Items)
        {
            var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
            item.SetAvalableQuantity(stock.Quantity);
        }

        var option = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));

        var cart = _mapper.Map<Cart>(model);
        var updatedCart = await _basketRepository.UpdateBasket(cart, option);
        var result = _mapper.Map<CartDto>(updatedCart);
        return Ok(result);
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string userName)
    {
        var result = await _basketRepository.DeleteBasketByUserName(userName);
        return Ok(result);
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var basket = await _basketRepository.GetBasketByUserName(basketCheckout.UserName);
        if (basket == null) return NotFound();

        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);

        await _basketRepository.DeleteBasketByUserName(basketCheckout.UserName);

        return Accepted();
    }

    [HttpPost("[action]", Name = "SendEmailReminder")]
    public ContentResult SendEmailReminder()
    {
        var emailTemplate = _emailTemplateService.GenerateReminderCheckoutOrderEmail("userName");
        var result = new ContentResult()
        {
            Content = emailTemplate,
            ContentType = "text/html"
        };
        return result;
    }
}