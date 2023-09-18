using Contracts.Common.Events;
using Ordering.Domain.Entities;
using Shared.Enums;

namespace Ordering.Domain.OrderAggregate.Events;

public class OrderCreatedEvent: BaseEvent
{
    public long Id { get; set; }
    public string UserName { get; set; }

    public string DocumentNo { get; set; }

    public decimal TotalPrice { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailAddress { get; set; }

    public string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; }

    public EOrderStatus Status { get; set; }
    
    public string FullName { get; set; }

    public OrderCreatedEvent(long id, string userName, string documentNo, decimal totalPrice, string firstName, string lastName, string emailAddress,
        string shippingAddress, string invoiceAddress, EOrderStatus status, string fullName)
    {
        Id = id;
        UserName = userName;
        DocumentNo = documentNo;
        TotalPrice = totalPrice;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        ShippingAddress = shippingAddress;
        InvoiceAddress = invoiceAddress;
        Status = status;
        FullName = fullName;
    }
}