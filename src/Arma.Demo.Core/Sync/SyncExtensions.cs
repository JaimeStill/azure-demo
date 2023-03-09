using Microsoft.Extensions.DependencyInjection;

namespace Arma.Demo.Core.Sync;
public static class SyncExtensions
{
    public static void AddSyncGroupProvider(this IServiceCollection services) =>
        services.AddSingleton<SyncGroupProvider>();

    public static void AddSyncConnection<Connection,T>(this IServiceCollection services)
        where Connection : SyncConnection<T> =>
            services.AddSingleton<Connection>();
}