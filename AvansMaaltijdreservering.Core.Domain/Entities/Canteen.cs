using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Entities;

public class Canteen
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Canteen location is required")]
    public CanteenLocation Location { get; set; }
    
    [Required(ErrorMessage = "City is required")]
    public City City { get; set; }
    
    public bool ServesWarmMeals { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<CanteenEmployee> Employees { get; set; } = new List<CanteenEmployee>();
    [JsonIgnore]
    public virtual ICollection<Package> Packages { get; set; } = new List<Package>();
}
