using System.ComponentModel.DataAnnotations;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    public bool ContainsAlcohol { get; set; }
    
    [Url(ErrorMessage = "Photo URL must be a valid URL")]
    [StringLength(500, ErrorMessage = "Photo URL must not exceed 500 characters")]
    public string? PhotoUrl { get; set; }
    
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
