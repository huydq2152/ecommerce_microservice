using Shared.Enums;

namespace Shared.DTOs.Inventory;

public class SaleItemDto
{
    public string ItemNo { get; set; }
    public int Quantity { get; set; }
    public EDocumentType DocumentType => EDocumentType.Sale;
}