namespace JsonToClassConverter.ClassDefinitions.Models
{
    public class CSharpField
    {
        public CSharpField(string name, string type, bool isArray)
        {
            Name = name;
            Type = type;
            IsArray = isArray;
        }

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsArray { get; set; }
    }
}