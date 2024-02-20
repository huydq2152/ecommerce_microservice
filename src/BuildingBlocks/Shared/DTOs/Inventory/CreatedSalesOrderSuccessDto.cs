namespace Shared.DTOs.Inventory;

public class CreatedSalesOrderSuccessDto
{
    public string DocumentNo { get; set; }

    public CreatedSalesOrderSuccessDto(string documentNo)
    {
        DocumentNo = documentNo;
    }
}