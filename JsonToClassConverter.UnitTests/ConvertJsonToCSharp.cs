using FluentAssertions;
using JsonToClassConverter.ClassDefinitions.Extensions;
using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Extensions.Logging;

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
                        new CSharpField("FirstName", "string", false),
                        new CSharpField("Jobs", "string", true),
                        new CSharpField("Qualified", "bool", false),
                        new CSharpField("PeopleList", "PeopleList", true),
                        new CSharpField("CarProp", "CarProp", false),
                        new CSharpField("CarProps", "CarProp", true)
                    }
                },
                new CSharpClass("PeopleList")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Name", "string", false),
                        new CSharpField("Age", "double", false),
                        new CSharpField("Locations", "double", true)
                    }
                },
                new CSharpClass("CarProp")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Brand", "string", false),
                        new CSharpField("Age", "double", false),
                        new CSharpField("VehicleInfo", "VehicleInfo", false),
                        new CSharpField("Owners", "Owners", true)
                    }
                },
                new CSharpClass("VehicleInfo")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Seats", "double", false)
                    }
                },
                new CSharpClass("Owners")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("Details", "string", false),
                        new CSharpField("Credentials", "Credentials", true)
                    }
                },
                new CSharpClass("Credentials")
                {
                    Fields = new List<CSharpField>
                    {
                        new CSharpField("TheText", "string", false)
                    }
                }
            };

            ILogger<ConverterController> logger = GetLogger();

            Mock<IJsonService> jsonService = new Mock<IJsonService>();

            //Act
            List<CSharpClass> classDefinitions = new ConverterController(logger, new CommandLineOptions
            {
                FilePath = "SampleData.json",
                OutputPath = "OutputPath.cs"
            }, jsonService.Object)
            .Convert(json);

            //Assert
            classDefinitions.Should().BeEquivalentTo(expectedRes);

            classDefinitions.PrintOutput(logger);
        }

        private static ILogger<ConverterController> GetLogger() =>
            new SerilogLoggerFactory(new LoggerConfiguration()
                .MinimumLevel.Is(Serilog.Events.LogEventLevel.Information)
                .WriteTo.Console(outputTemplate: "{Message}{NewLine}{Exception}")
                .CreateLogger())
            .CreateLogger<ConverterController>();
    }
}