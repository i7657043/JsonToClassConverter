using FluentAssertions;
using JsonToClassConverter.JsonParsing.Extensions;
using System.Text.Json;

namespace JsonToClassConverter.UnitTests
{
    public class Tests
    {
        string json = string.Empty;

        [SetUp]
        public void Setup()
        {
            json = File.ReadAllText($"{Directory.GetCurrentDirectory()}/SampleData.json").SanitiseJson();
        }

        [Test]
        public void ProcessClassDefinitions_ValidJson_ReturnsSuccess()
        {
            //Arrange
            List<CSharpClass> expectedRes = new List<CSharpClass>
            {
                new CSharpClass("Outer")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("FirstName", "String", false),
                        new CSharpField("Jobs", "String", true),
                        new CSharpField("Qualified", "Boolean", false),
                        new CSharpField("CarProps", "CarProps", true),
                        new CSharpField("CarProp", "CarProps", false)
                    }
                },
                new CSharpClass("CarProps")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Brand", "String", false),
                        new CSharpField("Age", "Double", false),
                        new CSharpField("VehicleInfo", "VehicleInfo", false),
                        new CSharpField("Owners", "Owners", false)
                    }
                },
                new CSharpClass("VehicleInfo")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Seats", "Double", false)
                    }
                },
                new CSharpClass("Owners")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Details", "String", false),
                        new CSharpField("Credentials", "Credentials", true)
                    }
                },
                new CSharpClass("Credentials")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("TheText", "String", false)
                    }
                }
            };

            //Act
            JsonClass model = new JsonParser().ProcessJsonProps(new JsonClass(), JsonDocument.Parse(json).RootElement.EnumerateObject());
            model.Name = "Outer";

            List<CSharpClass> classDefinitions = new ClassDefinitionGenerator().GenerateClassDefinitions(model, new List<CSharpClass>());

            //Assert
            classDefinitions.Should().BeEquivalentTo(expectedRes);

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
}