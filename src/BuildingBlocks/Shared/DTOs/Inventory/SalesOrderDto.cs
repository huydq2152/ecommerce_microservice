namespace Shared.DTOs.Inventory;

public class SalesOrderDto
{
    public string OrderNo { get; set; }
    public List<SaleItemDto> SaleItems { get; set; }
}