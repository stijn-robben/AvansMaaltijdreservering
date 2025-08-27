using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class CanteenEmployee
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Employee name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Employee name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Employee number is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Employee number must be between 1 and 20 characters")]
    public string EmployeeNumber { get; set; } = string.Empty;
    
    public int CanteenId { get; set; }
    [JsonIgnore]
    public virtual Canteen Canteen { get; set; } = null!;
    
    public bool WorksAtCanteen(int canteenId)
    {
        return CanteenId == canteenId;
    }
}
