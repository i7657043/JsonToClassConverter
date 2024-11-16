using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.ClassDefinitions;
using JsonToClassConverter.JsonParsing.Models;
using JsonToClassConverter.JsonParsing;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using JsonToClassConverter.ClassDefinitions.Extensions;
using JsonToClassConverter.JsonParsing.Extensions;

public class ConverterController : IConverterController
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _commandLineOptions;
    
    public ConverterController(ILogger<ConverterController> logger, CommandLineOptions commandLineOptions)
    {
        _logger = logger;
        _commandLineOptions = commandLineOptions;
    }

    public async Task Run()
    {
        string json = !string.IsNullOrEmpty(_commandLineOptions.InputPath)
            ? File.ReadAllText(_commandLineOptions.InputPath).SanitiseJson()
            : _commandLineOptions.JsonText;

        List<CSharpClass> classDefinitions = Convert(json);

        _logger.LogInformation("Created the following {@ClassCount} classes:\n", classDefinitions.Count);

        classDefinitions.PrintOutput(_logger);

        await WriteAsync(classDefinitions, _commandLineOptions.OutputPath);
    }

    public List<CSharpClass> Convert(string json)
    {
        _logger.LogInformation("Processing JSON models");

        JsonElement.ObjectEnumerator jsonProps = new JsonElement.ObjectEnumerator();

        try
        {
            jsonProps = JsonDocument.Parse(json).RootElement.EnumerateObject();
        }
        catch (Exception ex)
        {
            throw new JsonParsingException($"Could not parse invalid JSON: {ex.Message}");
        }        

        JsonClass jsonModel = new JsonParser().ProcessJsonProps(new JsonClass("Outer"), jsonProps);

        _logger.LogInformation("Generating class definitions");

        List<CSharpClass> classDefinitions = new ClassDefinitionGenerator().GenerateClassDefinitions(jsonModel, new List<CSharpClass>());

        _logger.LogInformation("Finalising class definitions");

        List<CSharpClass> finalisedClassDefinitions = new List<CSharpClass>();

        foreach (CSharpClass classDefinition in classDefinitions)
        {
            CSharpClass? existing = finalisedClassDefinitions.GetIfExists(classDefinition);
            if (existing == null)
            {
                finalisedClassDefinitions.Add(classDefinition);
                continue;
            }

            ReplaceNullableValuesIfCurrentIsBetter(classDefinition, existing);

            //because we have a dupe class we aren't storing we need to replace references using the existing type
            UpdateTypesInFinalClassDefinitions(finalisedClassDefinitions, classDefinition, existing);
        }

        return finalisedClassDefinitions;
    }

    public async Task WriteAsync(List<CSharpClass> classDefinitions, string outputPath) =>
        await File.WriteAllTextAsync(outputPath, classDefinitions.GetClassDefinitionsAsOutput());

    private static void UpdateTypesInFinalClassDefinitions(List<CSharpClass> finalisedClasses, CSharpClass current, CSharpClass existing)
    {
        foreach (CSharpClass classDef in finalisedClasses)
            foreach (CSharpField field in classDef.Fields)
                field.Type = field.Type == current.Name //This works because we use json key to name the type
                    ? existing.Name
                    : field.Type;
    }

    private static void ReplaceNullableValuesIfCurrentIsBetter(CSharpClass current, CSharpClass existing)
    {
        foreach (CSharpField existingField in existing.Fields)
            if (existingField.Type == "null")
            {
                CSharpField currentField = current.Fields.First(field => field.Name == existingField.Name);
                if (currentField.Type != "null")
                    existingField.Type = currentField.Type;
            }
    }
}

