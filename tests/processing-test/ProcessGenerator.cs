using Arma.Demo.Core.Processing;

namespace Processor;
public static class ProcessGenerator
{
    public static Process ApprovalProcess() =>
        new()
        {
            Name = "Demo Approval Process",
            Intent = Intent.Approve,
            Tasks = new()
            {
                new()
                {
                    Name = "Security Review",
                    Section = "Cybersecurity",
                    Step = 1,
                    Duration = 800
                },
                new()
                {
                    Name = "Legal Review",
                    Section = "Legal",
                    Step = 2,
                    Duration = 1100
                },
                new()
                {
                    Name = "Informal Review",
                    Section = "Operational Review Board",
                    Step = 3,
                    Duration = 500
                },
                new()
                {
                    Name = "Formal Review",
                    Section = "Command Review Board",
                    Step = 4,
                    Duration = 1800
                },
                new()
                {
                    Name = "Final Approval",
                    Section = "Headquarters Commander",
                    Step = 5,
                    Duration = 1000
                }
            }
        };
}