namespace Shared.DTOs.Cart;

public class CartItemDto
{
    public int Quantity { get; set; }

    public decimal ItemPrice { get; set; }

    public string ItemNo { get; set; }
    public string ItemName { get; set; }

    public int AvailableQuantity { get; set; }
    public void SetAvalableQuantity(int quantity) => AvailableQuantity = quantity;
}