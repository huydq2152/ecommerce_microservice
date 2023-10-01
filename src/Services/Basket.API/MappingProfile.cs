using AutoMapper;
using Basket.API.Entities;
using EventBus.Messages.IntegrationEvents.Events;
using Shared.DTOs.Cart;

namespace Basket.API;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckout, BasketCheckoutEvent>();
        CreateMap<CartDto, Cart>().ReverseMap();
        CreateMap<CartItemDto, CartItem>().ReverseMap();
    }
}