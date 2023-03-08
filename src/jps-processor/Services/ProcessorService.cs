using Arma.Demo.Core.Processing;
using Arma.Demo.Core.Sync;
using Microsoft.AspNetCore.SignalR.Client;

namespace Processor.Services;
public class ProcessorStatus
{
    public string? ConnectionId { get; private set; }
    public string State { get; private set; }

    public ProcessorStatus(HubConnection client)
    {
        ConnectionId = client.ConnectionId;
        State = client.State.ToString();
    }
}

public class ProcessorService : SyncService<Package>
{
    Guid? Key { get; set; }

    public ProcessorService(IConfiguration config) : base(
        config.GetValue<string>("SyncServer:ProcessorUrl") ?? "https://jps-sync.azurewebsites.net/processor"
    )
    {
        OnRegistered.Set((Guid key) =>
        {
            Console.WriteLine($"Service successfully registered at {key}");
            Key = key;
        });

        OnPush.Set(ProcessPackage);
    }

    public ProcessorStatus GetStatus() => new(connection);

    public static async Task Initialize(IServiceProvider services)
    {
        ProcessorService? processor = services.GetService<ProcessorService>();

        if (processor is not null)
            await processor.Register();
    }

    public async Task Register()
    {
        if (!Key.HasValue)
        {
            Key = Guid.NewGuid();
            await Connect();
            Console.WriteLine($"Registering service with key {Key}");
            await connection.InvokeAsync("RegisterService", Key);
        }
    }

    public async Task ProcessPackage(SyncMessage<Package> message)
    {
        Console.WriteLine($"Processing package {message.Data.Name}");

        Process process = GenerateProcess(message.Data);

        Console.WriteLine($"Notifying group {message.Data.Key}");

        message.Message = $"Submitting package {message.Data.Name} for {message.Data.Intent.ToActionString()}";

        Console.WriteLine(message.Message);
        await Notify(message);

        await Task.Delay(1200);

        message.Message = $"Package {message.Data.Name} assigned process {process.Name}";

        Console.WriteLine(message.Message);
        await Notify(message);

        for (int i = 0; i < process.Tasks.Count; i++)
        {
            ProcessTask task = process.Tasks[i];

            message.Message = $"Current step: {task.Name}";
            Console.WriteLine(message.Message);
            await Notify(message);

            await Task.Delay (task.Duration);

            if (i == process.Tasks.Count - 1 && message.Data.Intent == Intent.Destroy)
                message.Message = $"Package {message.Data.Name} was rejected by {task.Section}";
            else
                message.Message = $"Package {message.Data.Name} was successfully approved by {task.Section}";

            Console.WriteLine(message.Message);
            await Notify(message);
        }

        await Task.Delay(300);

        if (message.Data.Intent == Intent.Destroy)
        {
            message.Message = $"Items in package {message.Data.Name} cannot be destroyed";
            Console.WriteLine(message.Message);
            await Reject(message);
        }
        else
        {
            message.Message = $"Package {message.Data.Name} was successfully approved";
            Console.WriteLine(message.Message);
            await Complete(message);
        }
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