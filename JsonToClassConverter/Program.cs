using JsonToClassConverter.ClassDefinitions;
using JsonToClassConverter.ClassDefinitions.Extensions;
using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing;
using JsonToClassConverter.JsonParsing.Extensions;
using JsonToClassConverter.JsonParsing.Models;
using System.Text.Json;

namespace JsonToClassConverter
{
    internal partial class Program
    {

        private static async Task Main(string[] args)
        {
            string json = File.ReadAllText($"{Directory.GetCurrentDirectory()}/SampleData.json").SanitiseJson();

            JsonClass model = new JsonParser().ProcessJsonProps(new JsonClass(), JsonDocument.Parse(json).RootElement.EnumerateObject());
            model.Name = "Outer";

            List<CSharpClass> classDefinitions = new ClassDefinitionGenerator().GenerateClassDefinitions(model, new List<CSharpClass>());            

            classDefinitions.PrintOutput();
        }
    }
}