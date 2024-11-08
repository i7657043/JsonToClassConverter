namespace JsonToClassConverter.JsonParser.Extensions
{
    internal static class JsonExtensions
    {
        public static string SanitiseJson(this string json) =>
            json.Replace("\r\n", string.Empty).Replace(" ", string.Empty);
    }
}
