using System.ComponentModel.DataAnnotations;

namespace Basket.API.Entities;

public class CartItem
{
    [Required]
    [Range(1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.1, double.PositiveInfinity, ErrorMessage = "The field {0} must be >= {1}.")]
    public decimal ItemPrice { get; set; }
    
    [Required]
    public string ItemNo { get; set; }
    [Required]
    public string ItemName { get; set; }
    
    public int AvailableQuantity { get; set; }
    public void SetAvalableQuantity(int quantity) => AvailableQuantity = quantity;
}