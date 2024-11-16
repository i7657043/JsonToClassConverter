public class JsonParsingException : Exception
{
    public JsonParsingException(string jsonParsingErrorMessage) =>
        JsonParsingErrorMessage = jsonParsingErrorMessage;

    public string JsonParsingErrorMessage { get; } = string.Empty;
}