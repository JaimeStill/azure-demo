using Microsoft.Extensions.Configuration;

namespace SyncServer;
public static class ConfigExtensions
{
    public static string[]? GetConfigArray(this IConfiguration config, string section) => 
        config.GetSection(section)
            .GetChildren()
            .Select(x => x.Value)
            .ToArray();
}