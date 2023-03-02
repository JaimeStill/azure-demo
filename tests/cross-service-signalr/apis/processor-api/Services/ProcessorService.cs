using Arma.Demo.Core.Processing;
using Microsoft.AspNetCore.SignalR;
using Processor.Hubs;

namespace Processor.Services;
public class ProcessorService
{
    readonly IHubContext<ProcessorHub> socket;
    public ProcessorService(IHubContext<ProcessorHub> socket)
    {
        this.socket = socket;
    }

    dynamic Group(Guid key) =>
        socket.Clients
            .Group(key.ToString());

    async Task Broadcast(Guid key, string message) =>
        await Group(key)
            .SendAsync("broadcast", message);

    async Task Complete(Guid key, string message) =>
        await Group(key)
            .SendAsync("complete", message);

    public async Task ProcessPackage(Package package)
    {
        Process process = GenerateProcess(package);

        await Broadcast(
            package.Key,
            $"Submitting package {package.Name} for {package.Intent.ToActionString()}"
        );

        await Task.Delay(1200);

        await Complete(
            package.Key,
            $"Package {package.Name} was successfully approved"
        );
    }

    static Process GenerateProcess(Package package) => package.Intent switch
    {
        Intent.Approve  => Approval(),
        Intent.Acquire  => Acquisition(),
        Intent.Transfer => Transfer(),
        Intent.Destroy  => Destruction(),
        _ => throw new ArgumentOutOfRangeException(
            "An unexpected intent was provided and no associated process could be found"
        )
    };

    static Process Approval() =>
        new()
        {
            Name = "Approval Processor",
            Tasks = GenerateTasks()
        };

    static Process Acquisition() =>
        new()
        {
            Name = "Acquisition Processor",
            Tasks = GenerateTasks()
        };

    static Process Transfer() =>
        new()
        {
            Name = "Transfer Processor",
            Tasks = GenerateTasks()
        };

    static Process Destruction() =>
        new()
        {
            Name = "Destruction Processor",
            Tasks = GenerateTasks()
        };

    static List<ProcessTask> GenerateTasks() => new()
    {
        new()
        {
            Name = "Security Review",
            Section = "Cybersecurity",
            Step = 1,
            Duration = 800
        },
        new()
        {
            Name = "Legal Review",
            Section = "Legal",
            Step = 2,
            Duration = 1100
        },
        new()
        {
            Name = "Informal Review",
            Section = "Operational Review Board",
            Step = 3,
            Duration = 500
        },
        new()
        {
            Name = "Formal Review",
            Section = "Command Review Board",
            Step = 4,
            Duration = 1800
        },
        new()
        {
            Name = "Final Approval",
            Section = "Headquarters Commander",
            Step = 5,
            Duration = 1000
        }
    };
}