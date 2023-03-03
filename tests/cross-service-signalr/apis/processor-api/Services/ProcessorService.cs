using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;
using Microsoft.AspNetCore.SignalR.Client;

namespace Processor.Services;
public class ProcessorService : SyncService<Package>
{
    Guid Key { get; set; }

    public ProcessorService(IConfiguration config) : base(
        config.GetValue<string>("SyncServer:ProcessorUrl") ?? "http://localhost:5100/processor"
    )
    {
        Key = Guid.NewGuid();

        OnRegistered = (Guid key) =>
        {
            Console.WriteLine($"Service successfully registered at {key}");
            Key = key;
        };

        OnInitialize = ProcessPackage;
    }

    public async Task Register()
    {
        await EnsureConnection();
        Console.WriteLine($"Registering service with key {Key}");
        await connection.InvokeAsync("RegisterService", Key);
    }

    public async Task ProcessPackage(SyncMessage<Package> message)
    {
        Console.WriteLine($"Processing package {message.Data.Name}");

        Process process = GenerateProcess(message.Data);

        Console.WriteLine($"Notifying group {message.Data.Key}");

        message.Message = $"Submitting package {message.Data.Name} for {message.Data.Intent.ToActionString()}";

        await Notify(message);

        await Task.Delay(1200);

        message.Message = $"Package {message.Data.Name} assigned process {process.Name}";

        await Notify(message);

        foreach (ProcessTask task in process.Tasks)
        {
            message.Message = $"Current step: {task.Name}";
            await Notify(message);

            await Task.Delay(task.Duration);

            message.Message = $"Package {message.Data.Name} was successfully appoved by {task.Section}";
            await Notify(message);
        }

        await Task.Delay(300);

        message.Message = $"Package {message.Data.Name} was successfully approved";
        await Complete(message);

        Console.WriteLine($"Processing package {message.Data.Name} with process {process.Name} was successful");
    }

    static Process GenerateProcess(Package package) => package.Intent switch
    {
        Intent.Approve => Approval(),
        Intent.Acquire => Acquisition(),
        Intent.Transfer => Transfer(),
        Intent.Destroy => Destruction(),
        _ => throw new ArgumentOutOfRangeException(
            nameof(package.Intent),
            package.Intent,
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