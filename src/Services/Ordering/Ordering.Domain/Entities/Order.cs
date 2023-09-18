using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Common.Events;
using Ordering.Domain.OrderAggregate.Events;
using Shared.Enums;

namespace Ordering.Domain.Entities;

public class Order : AuditableEventEntity<long>
{
    [Required] public string UserName { get; set; }

    public Guid DocumentNo { get; set; } = Guid.NewGuid();

    [Column(TypeName = "decimal(10,2)")] public decimal TotalPrice { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(250)")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Column(TypeName = "nvarchar(250)")]
    public string EmailAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string ShippingAddress { get; set; }

    [Column(TypeName = "nvarchar(max)")] public string InvoiceAddress { get; set; }

    public EOrderStatus Status { get; set; }

    [NotMapped] public string FullName => $"{FirstName} {LastName}";

    public Order AddedOrder()
    {
        AddDomainEvents(new OrderCreatedEvent(Id, UserName, DocumentNo.ToString(), TotalPrice, FirstName, LastName,
            EmailAddress,
            ShippingAddress, InvoiceAddress, Status, FullName));
        return this;
    }

    public Order DeletedOrder()
    {
        RemoveDomainEvents(new OrderDeletedEvent(Id));
        return this;
    }
}