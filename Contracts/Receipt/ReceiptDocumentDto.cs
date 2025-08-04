namespace WarehouseManagement.Contracts.Receipt;

public class ReceiptDocumentDto
{
    public int Id { get; set; }
    public required string Number { get; set; }
    public DateTime Date { get; set; }
    public List<ReceiptResourceDto>? ReceiptResources { get; set; }
}
