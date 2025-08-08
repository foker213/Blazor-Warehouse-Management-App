namespace WarehouseManagement.Contracts;

public class FilterDto
{
    public List<string>? Resources { get; set; }
    public List<string>? UnitsOfMeasure { get; set; }
    public DateOnly? DateStart { get; set; }
    public DateOnly? DateEnd { get; set; }
    public List<string>? Numbers { get; set; }
    public List<string>? Clients { get; set; }
}