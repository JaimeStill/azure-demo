using Arma.Demo.Core.Processing;
using Processor;

await new ProcessingEngine(
    PackageGenerator.ApprovalPackage()
).Execute();