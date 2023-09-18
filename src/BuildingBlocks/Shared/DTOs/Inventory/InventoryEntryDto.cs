using Shared.Enums;

namespace Shared.DTOs.Inventory;

public class InventoryEntryDto
{
    public string Id { get; set; }

    public EDocumentType DocumentType { get; set; }

    public string DocumentNo { get; set; }

    public string ItemNo { get; set; }

    public int Quantity { get; set; }

    public string ExternalDocumentNo { get; set; }
}