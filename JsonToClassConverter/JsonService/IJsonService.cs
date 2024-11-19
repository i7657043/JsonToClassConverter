public interface IJsonService
{
    Task<string> GetJsonFromSource(CommandLineOptions commandLineOptions);
}

