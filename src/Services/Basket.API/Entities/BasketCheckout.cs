using System.ComponentModel.DataAnnotations;

namespace Basket.API.Entities;

public class BasketCheckout
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string EmailAddress { get; set; }
    [Required]
    public string ShippingAddress { get; set; }
    [Required]
    public string InvoiceAddress { get; set; }
}