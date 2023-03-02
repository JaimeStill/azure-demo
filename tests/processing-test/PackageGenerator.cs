using Arma.Demo.Core.Processing;

namespace Processor;
public static class PackageGenerator
{
    public static Package ApprovalPackage() =>
        new() {
            Name = "Demo Approval",
            Intent = Intent.Approve,
            Resources = new()
            {
                new() { Name = "Training Plan" },
                new() { Name = "Servers" }
            }
        };
}