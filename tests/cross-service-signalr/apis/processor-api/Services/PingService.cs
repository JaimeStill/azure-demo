using Microsoft.AspNetCore.SignalR;
using Processor.Hubs;

namespace Processor.Services;
public class PingService
{
    readonly IHubContext<ProcessorHub> socket;
    public PingService(IHubContext<ProcessorHub> socket)
    {
        this.socket = socket;
    }

    public async Task<bool> Pong(Guid key)
    {
        try
        {
            Console.WriteLine($"Broadcasting PONG to group {key}");

            await socket.Clients
                .Group(key.ToString())
                .SendAsync("complete", "Pong");

            return true;
        }
        catch
        {
            return false;
        }
    }
}