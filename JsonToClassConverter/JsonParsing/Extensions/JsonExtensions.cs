namespace JsonToClassConverter.JsonParsing.Extensions
{
    public static class JsonExtensions
    {
        public static string SanitiseJson(this string json) =>
            json.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);
    }
}
