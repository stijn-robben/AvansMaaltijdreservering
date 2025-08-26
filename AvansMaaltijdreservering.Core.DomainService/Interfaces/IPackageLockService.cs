namespace AvansMaaltijdreservering.Core.DomainService.Interfaces;

public interface IPackageLockService
{
    Task<bool> TryLockPackageAsync(int packageId, TimeSpan timeout = default);
    Task ReleasePackageLockAsync(int packageId);
}