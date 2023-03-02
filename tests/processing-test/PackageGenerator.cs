using Arma.Demo.Core.Processing;

namespace Processor;
public static class PackageGenerator
{
    public static Package ApprovalPackage() =>
        new()
        {
            Name = "Demo Approval",
            Intent = Intent.Approve
        };

    public static Package AcquisitionPackage() =>
        new()
        {
            Name = "Demo Acquisition",
            Intent = Intent.Acquire
        };

    public static Package TransferPackage() =>
        new()
        {
            Name = "Demo Transfer",
            Intent = Intent.Transfer
        };

    public static Package DestructionPackage() =>
        new()
        {
            Name = "Demo Destruction",
            Intent = Intent.Destroy
        };
}