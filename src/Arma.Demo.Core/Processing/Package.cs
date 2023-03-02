namespace Arma.Demo.Core.Processing;
public class Package
{
    public Guid Key { get; private set; }
    public string Name { get; set; }
    public Intent Intent { get; set; }

    public List<Resource> Resources { get; set; }

    public Package()
    {
        Key = Guid.NewGuid();
    }
}