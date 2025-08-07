namespace WarehouseManagement.Contracts;

public class FilterDto
{
    public string? Resource { get; set; }
    public string? UnitOfMeasure { get; set; }
    public DateOnly? DateStart { get; set; }
    public DateOnly? DateEnd { get; set; }
    public string? Number { get; set; }
    public string? Client { get; set; }
}