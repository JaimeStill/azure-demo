using Microsoft.AspNetCore.SignalR.Client;

namespace CoreCli.Clients;
public class PingClient : IAsyncDisposable
{
    public Guid Key { get; private set; }
    HubConnection? connection;
    readonly Action<string> output;
    readonly string hub;

    public PingClient(string hub, Action<string> output)
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
            connection.On("broadcast", output);
            connection.On("complete", output);

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