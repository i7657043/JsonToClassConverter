namespace JsonToClassConverter.ClassDefinitions.Models
{
    public class CSharpClass
    {
        public CSharpClass(string name)
        {
            Name = name;
        }

        public string Name { get; set; } = string.Empty;
        public List<CSharpField> Fields { get; set; } = new List<CSharpField>();
    }
}