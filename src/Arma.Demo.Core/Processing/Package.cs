namespace Arma.Demo.Core.Processing;
public class Package
{
    public static Guid Key => Guid.NewGuid();
    public string Name { get; set; }
    public Intent Intent { get; set; }

    public List<Resource> Resources { get; set; }
}