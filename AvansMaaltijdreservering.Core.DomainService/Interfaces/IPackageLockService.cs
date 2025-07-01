namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IPackageLockService
{
    Task<T> ExecuteWithPackageLockAsync<T>(int packageId, Func<Task<T>> operation);
    Task ExecuteWithPackageLockAsync(int packageId, Func<Task> operation);
}