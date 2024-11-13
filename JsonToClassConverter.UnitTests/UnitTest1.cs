using FluentAssertions;
using JsonToClassConverter.ClassDefinitions;
using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.ClassDefinitions.Extensions;
using JsonToClassConverter.JsonParsing;
using JsonToClassConverter.JsonParsing.Extensions;
using JsonToClassConverter.JsonParsing.Models;
using System.Text.Json;
using System.Collections.Generic;

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

            List<CSharpClass> finalisedClasses = new List<CSharpClass>();

            foreach (CSharpClass classDefinition in classDefinitions)
            {
                CSharpClass? existing = GetDuplicates(finalisedClasses, classDefinition);
                if (existing == null)
                {
                    finalisedClasses.Add(classDefinition);
                }
                else
                {
                    foreach (CSharpField existingField in existing.Fields)
                    {
                        if (existingField.Type == "Nullable`1")
                        {
                            CSharpField matchingField = classDefinition.Fields.First(field => field.Name == existingField.Name);
                            if (matchingField.Type != "Nullable`1")
                            {
                                existingField.Type = matchingField.Type;
                            }
                        }
                    }

                    foreach (CSharpField field in classDefinition.Fields)
                    {
                        if (field.Type == "Nullable`1")
                        {
                            CSharpField matchingField = existing.Fields.First(existingField => existingField.Name == field.Name);
                            if (matchingField.Type != "Nullable`1")
                            {
                                field.Type = matchingField.Type;
                            }
                        }
                    }
                }
            }

            //Assert
            //classDefinitions.Should().BeEquivalentTo(expectedRes);

            finalisedClasses.PrintOutput();
        }

        private static CSharpClass? GetDuplicates(List<CSharpClass> finalisedClasses, CSharpClass classDefinition)
        {
            return finalisedClasses.FirstOrDefault(finalisedClass => String.Join(string.Empty, finalisedClass.Fields.Select(field => field.Name)) == String.Join(string.Empty, classDefinition.Fields.Select(field => field.Name)));
        }
    }
}