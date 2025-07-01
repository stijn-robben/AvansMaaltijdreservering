using AvansMaaltijdreservering.Core.Domain.Entities;
using AvansMaaltijdreservering.Core.Domain.Enums;

namespace AvansMaaltijdreservering.Core.Domain.Interfaces;

public interface ICanteenRepository
{
    Task<IEnumerable<Canteen>> GetAllAsync();
    Task<Canteen?> GetByIdAsync(int id);
    Task<IEnumerable<Canteen>> GetCanteensByCityAsync(City city);
    Task<Canteen?> GetByLocationAsync(CanteenLocation location);
    Task<Canteen> AddAsync(Canteen canteen);
    Task<Canteen> UpdateAsync(Canteen canteen);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}