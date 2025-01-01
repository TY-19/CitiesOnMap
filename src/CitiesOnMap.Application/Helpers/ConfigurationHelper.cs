using CitiesOnMap.Application.Interfaces.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CitiesOnMap.Application.Helpers;

public class ConfigurationHelper(
    IConfiguration configuration,
    ILogger<ConfigurationHelper> logger
) : IConfigurationHelper
{
    public string? GetConfigurationValue(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);
        value ??= configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            logger.LogError("Cannot get the value of the {Key} from the environment or " +
                            "the configuration. Some features may not work correctly.", key);
        }

        return value;
    }
}