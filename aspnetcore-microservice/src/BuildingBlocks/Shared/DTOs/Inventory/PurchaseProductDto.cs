using Shared.Enums;

namespace Shared.DTOs.Inventory;

public class PurchaseProductDto
{
    private string? _itemNo;
    public void SetItemNo(string itemNo) => _itemNo = itemNo;
    public string? ItemNo => _itemNo;
    
    public EDocumentType EDocumentType = EDocumentType.Purchase;
    public int Quantity { get; set; }
    public string? DocumentNo { get; set; }
    public string? ExternalDocumentNo { get; set; }
}