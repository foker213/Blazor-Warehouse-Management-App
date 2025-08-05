namespace WarehouseManagement.Contracts;

public class FilterDto
{
    public string? Resource { get; set; }
    public string? UnitOfMeasure { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public string? Number { get; set; }
    public string? Client { get; set; }
}