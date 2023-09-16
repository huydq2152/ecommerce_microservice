using Shared.Enums;

namespace Shared.DTOs.Inventory;

public class PurchaseProductDto
{
    public EDocumentType EDocumentType = EDocumentType.Purchase;
    public string ItemNo { get; set; }
    public int Quantity { get; set; }
    public string DocumentNo { get; set; }
    public string ExternalDocumentNo { get; set; }
}