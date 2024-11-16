namespace JsonToClassConverter.ClassDefinitions.Extensions
{
    public static class StringExtensions
    {
        public static string GetUppercaseFirstLetter(this string word) =>
           char.ToUpper(word[0]) + word.Substring(1);
    }
}