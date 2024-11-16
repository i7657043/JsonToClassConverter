using FluentAssertions;
using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing.Extensions;
using Microsoft.Extensions.Logging;
using Moq;

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
                        new CSharpField("PeopleList", "PeopleList", true),
                        new CSharpField("Qualified", "Boolean", false),
                        new CSharpField("CarProps", "CarProp", true),
                        new CSharpField("CarProp", "CarProp", false)
                    }
                },
                new CSharpClass("PeopleList")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Name", "String", false),
                        new CSharpField("Age", "Double", false),
                        new CSharpField("Locations", "Double", true)
                    }
                },
                new CSharpClass("CarProp")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Brand", "String", false),
                        new CSharpField("Age", "Double", false),
                        new CSharpField("VehicleInfo", "VehicleInfo", false),
                        new CSharpField("Owners", "Owners", true)
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
            List<CSharpClass> classDefinitions = new ConverterController(new Mock<ILogger<ConverterController>>().Object, new CommandLineOptions
            {
                InputPath = "SampleData.json",
                OutputPath = "OutputPath.cs"
            })
            .Convert(json);

            //Assert
            classDefinitions.Should().BeEquivalentTo(expectedRes);
        }
    }
}