using JsonToClassConverter.ClassDefinitions.Extensions;

namespace JsonToClassConverter.JsonParsing.Models
{
    public class JsonField
    {
        public JsonField(string name, Type type)
        {
            Name = name.GetUppercaseFirstLetter();
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsArray { get; set; }
    }
}