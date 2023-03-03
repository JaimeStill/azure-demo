using Arma.Demo.Core.Processing;

namespace Processor;
public static class PackageGenerator
{
    public static Package ApprovalPackage() =>
        new() {
            Key = Guid.NewGuid(),
            Name = "Demo Approval",
            Intent = Intent.Approve,
            Resources = new()
            {
                new() { Key = Guid.NewGuid(), Name = "Training Plan" },
                new() { Key = Guid.NewGuid(), Name = "Servers" }
            }
        };
}