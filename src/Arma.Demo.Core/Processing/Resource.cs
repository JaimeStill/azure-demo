namespace Arma.Demo.Core.Processing;
public class Resource
{
    public Guid Key { get; private set; }
    public string Name { get; set; }

    public Resource()
    {
        Key = Guid.NewGuid();
    }
}