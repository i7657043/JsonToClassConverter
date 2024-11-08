using JsonToClassConverter.JsonParser.Extensions;
using System.Text.Json;

internal partial class Program
{

    private static async Task Main(string[] args)
    {
        string json = File.ReadAllText($"{Directory.GetCurrentDirectory()}/SampleData.json").SanitiseJson();

        JsonClass model = new JsonParser().ProcessJsonProps(new JsonClass(), JsonDocument.Parse(json).RootElement.EnumerateObject());
        model.Name = "Outer";

        List<CSharpClass> classDefinitions = new ClassDefinitionGenerator().GenerateClassDefinitions(model, new List<CSharpClass>());

        PrintOutput(classDefinitions);
    }


    private static void PrintOutput(List<CSharpClass> classDefinitions)
    {
        foreach (CSharpClass classDefinition in classDefinitions)
        {
            Console.WriteLine($"public class {classDefinition.Name}");
            Console.WriteLine("{");

            classDefinition.Fields.ForEach(field => 
                Console.WriteLine($"  public {field.Name} {field.Type}{(field.IsArray ? "[]" : string.Empty)}"));

            Console.WriteLine("}\n");
        }
    }
}
