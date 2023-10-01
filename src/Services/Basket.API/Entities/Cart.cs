namespace Basket.API.Entities;

public class Cart
{
    public string UserName { get; set; }
    
    public string EmailAddress { get; set; }

    public List<CartItem> Items { get; set; } = new();

    public Cart()
    {
    }

    public Cart(string userName)
    {
        UserName = userName;
    }

    public decimal TotalPrice => Items.Sum(item => item.Quantity * item.ItemPrice);

    public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.UtcNow;
    
    public string? JobId { get; set; }
}