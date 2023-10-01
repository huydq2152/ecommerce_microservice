namespace Shared.DTOs.Cart;

public class CartDto
{
    public string UserName { get; set; }
    
    public string EmailAddress { get; set; }

    public List<CartItemDto> Items { get; set; } = new();

    public CartDto()
    {
    }

    public CartDto(string userName)
    {
        UserName = userName;
    }

    public decimal TotalPrice => Items.Sum(item => item.Quantity * item.ItemPrice);
}