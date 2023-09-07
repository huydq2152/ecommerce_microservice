using System.ComponentModel.DataAnnotations;
using System.Net;
using Basket.API.Entities;
using Basket.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;

    public BasketController(IBasketRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> GetBasketByUserName([Required] string userName)
    {
        var result = await _repository.GetBasketByUserName(userName);
        return Ok(result ?? new Cart());
    }

    [HttpPost(Name = "UpdateBasket")]
    [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<Cart>> UpdateBasket([FromBody] Cart cart)
    {
        var option = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(1))
            .SetSlidingExpiration(TimeSpan.FromMinutes(5));
        var result = await _repository.UpdateBasket(cart, option);
        return Ok(result);
    }

    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<bool>> DeleteBasket([Required] string userName)
    {
        var result = await _repository.DeleteBasketByUserName(userName);
        return Ok(result);
    }
}