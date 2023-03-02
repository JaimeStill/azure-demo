using Arma.Demo.Core.Processing;

namespace Processor;
public class ProcessingEngine
{
    public Package Package { get; private set; }
    public Process Process { get; private set; }

    public ProcessingEngine(Package package)
    {
        Package = package;
        Process = GenerateProcess(package);
    }

    public async Task Execute()
    {
        Console.WriteLine($"Submitting package {Package.Name} for {Package.Intent.ToActionString()}");
        Console.WriteLine("Package Items:");

        foreach (Resource resource in Package.Resources)
            Console.WriteLine(resource.Name);

        await Task.Delay(800);

        Console.WriteLine($"Package {Package.Name} assigned process {Process.Name}");

        foreach (ProcessTask task in Process.Tasks)
        {
            Console.WriteLine($"Current step: {task.Name}");
            await Task.Delay(task.Duration);
            Console.WriteLine($"Package {Package.Name} was approved by {task.Section}");
        }

        Console.WriteLine($"Package {Package.Name} was successfully approved");
    }

    static Process GenerateProcess(Package package) => package.Intent switch
    {
        Intent.Approve  => ProcessGenerator.ApprovalProcess(),
        Intent.Acquire  => ProcessGenerator.AcquisitionProcess(),
        Intent.Transfer => ProcessGenerator.TransferProcess(),
        Intent.Destroy  => ProcessGenerator.DestructionProcess(),
        _ => throw new ArgumentOutOfRangeException(
            "An unexpected intent was provided and no associated process could be found"
        )
    };
}