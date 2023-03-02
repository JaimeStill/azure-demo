namespace Arma.Demo.Core.Processing;
public class Process
{
    public string Name { get; set; }
    public Intent Intent { get; set; }

    public List<ProcessTask> Tasks { get; set; }
}