using Microsoft.Extensions.Configuration;
using System.IO;

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public string Endpoint { get; set; }
    public string ContentType { get; set; }
}

public static class ConfigurationHelper
{
    public static ApiSettings GetApiSettings()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("apisettings.json")
            .Build();

        var apiSettings = config.GetSection("ApiSettings").Get<ApiSettings>();
        return apiSettings;
    }
}
