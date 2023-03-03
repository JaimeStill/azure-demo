using Microsoft.AspNetCore.SignalR.Client;

namespace Arma.Demo.Core.Sync;
public abstract class SyncService<T> : ISyncService<T>
{
    protected readonly HubConnection connection;
    protected readonly string endpoint;

    protected List<Guid> Groups { get; set; }
    public virtual Action<Guid> OnRegistered { get; set; }
    public virtual Func<SyncMessage<T>, Task> OnPush { get; set; }
    public virtual Func<SyncMessage<T>, Task> OnNotify { get; set; }
    public virtual Func<SyncMessage<T>, Task> OnComplete { get; set; }
    public virtual Func<SyncMessage<T>, Task> OnReturn { get; set; }
    public virtual Func<SyncMessage<T>, Task> OnReject { get; set; }

    public SyncService(string endpoint)
    {
        Console.WriteLine($"Building Sync connection at {endpoint}");
        connection = BuildHubConnection(endpoint);

        this.endpoint = endpoint;
        Groups = new();
    }

    protected virtual HubConnection BuildHubConnection(string endpoint) =>
        new HubConnectionBuilder()
            .WithUrl(endpoint)
            .WithAutomaticReconnect()
            .Build();

    public async Task EnsureConnection()
    {
        if (connection.State != HubConnectionState.Connected)
            await Connect();
    }

    public async Task Connect()
    {
        if (connection is not null)
        {
            Console.WriteLine("Registering Sync events");
            RegisterEvents();
            Console.WriteLine($"Connecting to {endpoint}");
            await connection.StartAsync();
        }
    }

    public async Task Join(Guid key)
    {
        Groups.Add(key);
        Console.WriteLine($"Joining group {key}");
        await connection.InvokeAsync("Join", key);
    }

    public async Task Leave(Guid key)
    {
        Groups.Remove(key);
        Console.WriteLine($"Leaving group {key}");
        await connection.InvokeAsync("Leave", key);
    }

    public async Task Push(SyncMessage<T> message)
    {
        message.Action = SyncAction.Push;
        await connection.InvokeAsync("SendPush", message);
    }

    public async Task Notify(SyncMessage<T> message)
    {
        message.Action = SyncAction.Notify;
        await connection.InvokeAsync("SendNotify", message);
    }

    public async Task Complete(SyncMessage<T> message)
    {
        message.Action = SyncAction.Complete;
        await connection.InvokeAsync("SendComplete", message);
    }

    public async Task Return(SyncMessage<T> message)
    {
        message.Action = SyncAction.Return;
        await connection.InvokeAsync("SendReturn", message);
    }

    public async Task Reject(SyncMessage<T> message)
    {
        message.Action = SyncAction.Reject;
        await connection.InvokeAsync("SendReject", message);
    }

    void RegisterEvents()
    {
        if (OnRegistered is not null)
            connection.On("Registered", OnRegistered);

        if (OnPush is not null)
            connection.On("Push", OnPush);

        if (OnNotify is not null)
            connection.On("Notify", OnNotify);

        if (OnComplete is not null)
            connection.On("Complete", OnComplete);

        if (OnReturn is not null)
            connection.On("Return", OnReturn);

        if (OnReject is not null)
            connection.On("Reject", OnReject);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnection()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected async ValueTask DisposeConnection()
    {
        if (connection is not null)
        {
            foreach (Guid key in Groups)
                await Leave(key);

            await connection
                .DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}