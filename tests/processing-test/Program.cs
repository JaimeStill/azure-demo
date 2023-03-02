using Arma.Demo.Core.Processing;
using Processor;

Package package = PackageGenerator.ApprovalPackage();

Console.WriteLine($"Submitting package {package.Name} for approval");

await Task.Delay(1200);

Process process = ProcessGenerator.ApprovalProcess();

Console.WriteLine($"Package {package.Name} assigned process {process.Name}");

foreach (ProcessTask task in process.Tasks)
{
    Console.WriteLine($"Current step: {task.Name}");
    await Task.Delay(task.Duration);
    Console.WriteLine($"Package {package.Name} was approved by {task.Section}");
}

Console.WriteLine($"Package {package.Name} was successfully approved");