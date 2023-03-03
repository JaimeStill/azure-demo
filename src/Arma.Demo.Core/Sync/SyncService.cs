using Microsoft.AspNetCore.SignalR.Client;

namespace Arma.Demo.Core.Sync;
public abstract class SyncService<T> : ISyncService<T>
{
    protected readonly HubConnection connection;
    protected readonly string endpoint;

    protected List<Guid> Groups { get; set; }
    protected abstract Func<SyncMessage<T>, Task> OnInitialize { get; set; }
    protected abstract Func<SyncMessage<T>, Task> OnNotify { get; set; }
    protected abstract Func<SyncMessage<T>, Task> OnComplete { get; set; }
    protected abstract Func<SyncMessage<T>, Task> OnReturn { get; set; }
    protected abstract Func<SyncMessage<T>, Task> OnReject { get; set; }

    public SyncService(string endpoint)
    {
        connection = new HubConnectionBuilder()
            .WithUrl(endpoint)
            .WithAutomaticReconnect()
            .Build();

        this.endpoint = endpoint;
        Groups = new();
    }

    public async Task Connect(Guid key)
    {
        if (connection is not null)
        {
            RegisterEvents();
            await connection.StartAsync();
            await Join(key);
        }
    }

    public async Task Join(Guid key) =>
        await connection.InvokeAsync("Join", key);

    public async Task Leave(Guid key) =>
        await connection.InvokeAsync("Leave", key);

    public async Task Initialize(SyncMessage<T> message) =>
        await connection.InvokeAsync("SendInitialize", message);

    public async Task Notify(SyncMessage<T> message) =>
        await connection.InvokeAsync("SendNotify", message);

    public async Task Complete(SyncMessage<T> message) =>
        await connection.InvokeAsync("SendComplete", message);

    public async Task Return(SyncMessage<T> message) =>
        await connection.InvokeAsync("SendReturn", message);

    public async Task Reject(SyncMessage<T> message) =>
        await connection.InvokeAsync("SendReject", message);

    void RegisterEvents()
    {
        if (OnInitialize is not null)
            connection.On("Initialize", OnInitialize);

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