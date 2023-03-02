using Microsoft.AspNetCore.SignalR.Client;

namespace CoreCli;
public class ProcessorClient : IAsyncDisposable
{
    public Guid Key { get; private set; }
    HubConnection? connection;
    Action<string> output;
    string hub;

    public ProcessorClient(string hub, Action<string> output)
    {
        Key = Guid.NewGuid();

        connection = new HubConnectionBuilder()
            .WithUrl(hub)
            .WithAutomaticReconnect()
            .Build();

        this.hub = hub;
        this.output = output;
    }

    public async Task Initialize()
    {
        if (connection is not null)
        {
            connection.On<string>("message", output);
            connection.On<string>("complete", output);

            Console.WriteLine($"Connecting to hub: {hub}");
            await connection.StartAsync();
            Console.WriteLine($"Connection status: {connection.State}");

            await connection
                .InvokeAsync("TriggerJoin", Key);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnection()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    async ValueTask DisposeConnection()
    {
        if (connection is not null)
        {
            await connection
                .InvokeAsync("TriggerLeave", Key);

            await connection
                .DisposeAsync()
                .ConfigureAwait(false);

            connection = null;
        }
    }
}