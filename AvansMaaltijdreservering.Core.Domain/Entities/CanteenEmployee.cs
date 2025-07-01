namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class CanteenEmployee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    
    public int CanteenId { get; set; }
    public virtual Canteen Canteen { get; set; } = null!;
    
    public bool WorksAtCanteen(int canteenId)
    {
        return CanteenId == canteenId;
    }
}
