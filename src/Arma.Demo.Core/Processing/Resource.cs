namespace Arma.Demo.Core.Processing;
public class Resource
{
    public Guid Key => Guid.NewGuid();
    public string Name { get; set; }
}