using Microsoft.Extensions.DependencyInjection;

namespace Arma.Demo.Core.Sync;
public class SyncServiceManager
{
    List<SyncService> Services { get; set; }

    public SyncServiceManager()
    {
        Services = new();
    }

    public IEnumerable<SyncService> GetServices() => Services.AsReadOnly();

    public void RegisterServices(IEnumerable<SyncService> services) =>
        Services.AddRange(services);

    public SyncService RegisterService(string name, string endpoint, Type hub)
    {
        SyncService service = new(name, endpoint, hub);
        Services.Add(service);
        return service;
    }

    public SyncService GetService(Type hub) =>
        Services.FirstOrDefault(service => service.Hub == hub);

    public void RemoveService(SyncService service)
    {
        Services.Remove(service);
    }
}
