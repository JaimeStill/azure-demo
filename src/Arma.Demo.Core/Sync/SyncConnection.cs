using Microsoft.AspNetCore.SignalR.Client;

namespace Arma.Demo.Core.Sync;
public abstract class SyncConnection<T> : ISyncConnection<T>
{
    protected readonly HubConnection connection;
    protected readonly string endpoint;
    protected CancellationToken token;

    protected List<Guid> Groups { get; set; }

    public bool Available { get; private set; }
    public SyncEvent<Guid> OnRegistered { get; private set; }
    public SyncEvent<SyncMessage<T>> OnPush { get; private set; }
    public SyncEvent<SyncMessage<T>> OnNotify { get; private set; }
    public SyncEvent<SyncMessage<T>> OnComplete { get; private set; }
    public SyncEvent<SyncMessage<T>> OnReturn { get; private set; }
    public SyncEvent<SyncMessage<T>> OnReject { get; private set; }

    public SyncConnection(string endpoint)
    {
        Console.WriteLine($"Building Sync connection at {endpoint}");
        connection = BuildHubConnection(endpoint);

        InitializeEvents();

        connection.Closed += async (error) =>
        {
            await Task.Delay(5000);
            await Connect();
        };

        connection.On("Available", () => Available = true);
        connection.On("Disconnected", () => Available = false);

        this.endpoint = endpoint;
        Groups = new();
        token = new();
    }

    public async Task Connect()
    {
        if (connection.State != HubConnectionState.Connected)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine($"Connecting to {endpoint}");
                    await connection.StartAsync(token);
                    return;
                }
                catch when (token.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to {endpoint}");
                    Console.WriteLine(ex.Message);
                    await Task.Delay(5000);
                }
            }
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
        message.Action = SyncActionType.Push;
        await connection.InvokeAsync("SendPush", message);
    }

    public async Task Notify(SyncMessage<T> message)
    {
        message.Action = SyncActionType.Notify;
        await connection.InvokeAsync("SendNotify", message);
    }

    public async Task Complete(SyncMessage<T> message)
    {
        message.Action = SyncActionType.Complete;
        await connection.InvokeAsync("SendComplete", message);
    }

    public async Task Return(SyncMessage<T> message)
    {
        message.Action = SyncActionType.Return;
        await connection.InvokeAsync("SendReturn", message);
    }

    public async Task Reject(SyncMessage<T> message)
    {
        message.Action = SyncActionType.Reject;
        await connection.InvokeAsync("SendReject", message);
    }

    protected virtual HubConnection BuildHubConnection(string endpoint) =>
        new HubConnectionBuilder()
            .WithUrl(endpoint)
            .WithAutomaticReconnect()
            .Build();

    protected void InitializeEvents()
    {
        OnRegistered = new("Registered", connection);
        OnPush = new("Push", connection);
        OnNotify = new("Notify", connection);
        OnComplete = new("Complete", connection);
        OnReturn = new("Return", connection);
        OnReject = new("Reject", connection);
    }

    public async ValueTask DisposeAsync()
    {
        DisposeEvents();

        await DisposeConnection()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    protected void DisposeEvents()
    {
        OnRegistered?.Dispose();
        OnPush?.Dispose();
        OnNotify?.Dispose();
        OnComplete?.Dispose();
        OnReturn?.Dispose();
        OnReject?.Dispose();
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