using AutoMapper;
using Shared.DTOs.Basket;
using Shared.DTOs.Cart;
using Shared.DTOs.Inventory;
using Shared.DTOs.Order;

namespace Saga.Orchestrator;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
        CreateMap<CartItemDto, SaleItemDto>();
    }
}