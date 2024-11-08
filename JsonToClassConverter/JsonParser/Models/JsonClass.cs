public class JsonClass
{
    public string Name { get; set; } = string.Empty;
    public List<JsonField> Fields { get; set; } = new List<JsonField>();
    public List<JsonClass> Children { get; set; } = new List<JsonClass>();
    public bool IsArray { get; set; }
}
