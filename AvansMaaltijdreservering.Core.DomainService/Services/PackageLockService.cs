using System.Collections.Concurrent;
using AvansMaaltijdreservering.Core.DomainService.Interfaces;

namespace AvansMaaltijdreservering.Core.DomainService.Services;

public class PackageLockService : IPackageLockService
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _packageLocks = new();

    public async Task<T> ExecuteWithPackageLockAsync<T>(int packageId, Func<Task<T>> operation)
    {
        var semaphore = _packageLocks.GetOrAdd(packageId, _ => new SemaphoreSlim(1, 1));
        
        await semaphore.WaitAsync();
        try
        {
            return await operation();
        }
        finally
        {
            semaphore.Release();
            
            // Clean up unused semaphores to prevent memory leaks
            if (semaphore.CurrentCount == 1)
            {
                _packageLocks.TryRemove(packageId, out _);
                semaphore.Dispose();
            }
        }
    }

    public async Task ExecuteWithPackageLockAsync(int packageId, Func<Task> operation)
    {
        await ExecuteWithPackageLockAsync(packageId, async () =>
        {
            await operation();
            return Task.CompletedTask;
        });
    }
}