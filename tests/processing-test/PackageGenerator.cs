using Arma.Demo.Core.Processing;

namespace Processor;
public static class PackageGenerator
{
    public static Package ApprovalPackage() =>
        new() { Name = "Demo Approval" };
}