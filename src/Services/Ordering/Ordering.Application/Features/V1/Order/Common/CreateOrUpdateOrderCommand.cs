using AutoMapper;
using Ordering.Application.Common.Mappings;

namespace Ordering.Application.Features.V1.Order.Common;

public class CreateOrUpdateOrderCommand: IMapFrom<Domain.Entities.Order>
{
    public decimal TotalPrice { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }

    public string ShippingAddress { get; set; }

    private string _invoiceAddress;

    public string? InvoiceAddress
    {
        get => _invoiceAddress;
        set => _invoiceAddress = value ?? ShippingAddress;
    }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateOrUpdateOrderCommand, Domain.Entities.Order>();
    }
}