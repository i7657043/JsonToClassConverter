using JsonToClassConverter.JsonParsing.Extensions;
using Microsoft.Extensions.Logging;

public class JsonService : IJsonService
{
    private readonly ILogger<JsonService> _logger;
    private readonly HttpClient _httpClient;

    public JsonService(ILogger<JsonService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<string> GetJsonFromSource(CommandLineOptions commandLineOptions)
    {
        string json = string.Empty;

        if (!string.IsNullOrEmpty(commandLineOptions.FilePath))
        {
            _logger.LogInformation($"Getting JSON from path: {commandLineOptions.FilePath}");

            json = File.ReadAllText(commandLineOptions.FilePath);
        }
        else if (!string.IsNullOrEmpty(commandLineOptions.JsonText))
        {
            _logger.LogInformation($"Getting JSON from input text");

            json = commandLineOptions.JsonText;
        }
        else if (!string.IsNullOrEmpty(commandLineOptions.Url))
        {
            _logger.LogInformation($"Getting JSON from URL: {commandLineOptions.Url}");

            json = await GetAsync(commandLineOptions.Url);
        }

        return json.SanitiseJson();
    }

    public async Task<string> GetAsync(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}