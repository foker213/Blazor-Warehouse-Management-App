namespace WarehouseManagement.Contracts;

public class FilterDto
{
    public string? Resource { get; set; }
    public string? UnitOfMeasure { get; set; }
    public DateTime Date { get; set; }
    public string? Number { get; set; }
    public string? Client { get; set; }
}