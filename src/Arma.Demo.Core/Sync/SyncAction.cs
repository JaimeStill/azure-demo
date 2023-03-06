using Microsoft.AspNetCore.SignalR.Client;

namespace Arma.Demo.Core.Sync;
public class SyncEvent<T> : IDisposable
{
    private readonly string method;
    private readonly HubConnection client;
    private IDisposable subscription;

    public SyncEvent(string method, HubConnection client)
    {
        this.method = method;
        this.client = client;
    }
    
    public void Set(Func<T, Task> action)
    {
        subscription?.Dispose();
        subscription = client.On(method, action);
    }

    public void Set(Action<T> action)
    {
        subscription?.Dispose();
        subscription = client.On(method, action);
    }

    public void Dispose()
    {
        subscription?.Dispose();
        GC.SuppressFinalize(this);
    }
}