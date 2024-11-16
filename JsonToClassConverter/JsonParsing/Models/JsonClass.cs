using JsonToClassConverter.ClassDefinitions.Extensions;

namespace JsonToClassConverter.JsonParsing.Models
{
    public class JsonClass
    {
        public JsonClass(string name) =>
            Name = name.GetUppercaseFirstLetter();

        public string Name { get; set; } = string.Empty;
        public List<JsonField> Fields { get; set; } = new List<JsonField>();
        public List<JsonClass> Children { get; set; } = new List<JsonClass>();
        public bool IsArray { get; set; }
    }
}