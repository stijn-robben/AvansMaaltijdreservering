using AvansMaaltijdreservering.Core.DomainService.Interfaces;
using System.Collections.Concurrent;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class PackageLockService : IPackageLockService
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _packageLocks = new();
    
    public async Task<bool> TryLockPackageAsync(int packageId, TimeSpan timeout = default)
    {
        var semaphore = _packageLocks.GetOrAdd(packageId, _ => new SemaphoreSlim(1, 1));
        
        if (timeout == default)
            timeout = TimeSpan.FromSeconds(30);
            
        return await semaphore.WaitAsync(timeout);
    }
    
    public async Task ReleasePackageLockAsync(int packageId)
    {
        if (_packageLocks.TryGetValue(packageId, out var semaphore))
        {
            semaphore.Release();
        }
        
        await Task.CompletedTask;
    }
}