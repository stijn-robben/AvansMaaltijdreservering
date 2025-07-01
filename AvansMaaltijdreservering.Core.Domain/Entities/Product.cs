namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool ContainsAlcohol { get; set; }
    public string? PhotoUrl { get; set; }
    
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
