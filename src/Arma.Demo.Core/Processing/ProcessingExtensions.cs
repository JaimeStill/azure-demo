namespace Arma.Demo.Core.Processing;
public static class ProcessingExtensions
{
    public static string ToActionString(this Intent intent) => intent switch
    {
        Intent.Acquire => "Acquisition",
        Intent.Approve => "Approval",
        Intent.Destroy => "Destruction",
        Intent.Transfer => "Transfer",
        _ => throw new ArgumentOutOfRangeException(
            nameof(intent),
            intent,
            "An unexpected intent was provided"
        )
    };
}