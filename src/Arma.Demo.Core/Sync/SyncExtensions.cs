using Microsoft.Extensions.DependencyInjection;

namespace Arma.Demo.Core.Sync;
public static class SyncExtensions
{
    public static void AddSyncGroupProvider(this IServiceCollection services) =>
        services.AddSingleton<SyncGroupProvider>();

    public static void AddSyncService<S,T>(this IServiceCollection services)
        where S : SyncService<T> =>
            services.AddSingleton<S>();
}