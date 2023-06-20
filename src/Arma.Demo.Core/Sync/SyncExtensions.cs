using Microsoft.Extensions.DependencyInjection;

namespace Arma.Demo.Core.Sync;
public static class SyncExtensions
{
    public static void AddSyncServiceManager(this IServiceCollection services) =>
        services.AddSingleton<SyncServiceManager>();

    public static void RegisterSyncServices(this IServiceProvider provider, IEnumerable<SyncService> services)
    {
        SyncServiceManager manager = provider.GetService<SyncServiceManager>();
        manager?.RegisterServices(services);
    }

    public static async Task InitializeSyncServiceManager(this IServiceProvider provider)
    {
        SyncServiceManager manager = provider.GetService<SyncServiceManager>();

        if (manager is not null)
        {
            foreach (SyncService service in manager.GetServices())
                await service.Ping();
        }
    }

    public static void AddSyncConnection<Connection,T>(this IServiceCollection services)
        where Connection : SyncConnection<T> =>
            services.AddSingleton<Connection>();
}