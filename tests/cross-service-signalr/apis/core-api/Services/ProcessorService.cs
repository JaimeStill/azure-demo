using Arma.Demo.Core.Processing;

namespace Core.Services;
public class ProcessorService
{
    readonly string processorEndpoint;
    public ProcessorService(IConfiguration config)
    {
        processorEndpoint = config.GetValue<string>("ProcessorEndpoint")
            ?? "http://localhost:5000/api/process/";
    }

    public async Task<bool> SendPackage(Package package)
    {
        Console.WriteLine($"Sending package {package.Name} to processing service at {processorEndpoint}");
        HttpClient client = new();
        HttpResponseMessage response = await client.PostAsJsonAsync(processorEndpoint, package);
        Console.WriteLine($"Package process status: {response.StatusCode}");
        return response.IsSuccessStatusCode;
    }
}
