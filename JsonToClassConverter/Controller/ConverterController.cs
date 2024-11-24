using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.ClassDefinitions;
using JsonToClassConverter.JsonParsing.Models;
using JsonToClassConverter.JsonParsing;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using JsonToClassConverter.ClassDefinitions.Extensions;

public class ConverterController : IConverterController
{
    private readonly ILogger _logger;
    private readonly CommandLineOptions _commandLineOptions;
    private readonly IJsonService _jsonService;

    public ConverterController(ILogger<ConverterController> logger, CommandLineOptions commandLineOptions, IJsonService jsonService)
    {
        _logger = logger;
        _commandLineOptions = commandLineOptions;
        _jsonService = jsonService;
    }

    public async Task RunAsync()
    {
        string json = await _jsonService.GetJsonFromSource(_commandLineOptions);

        List<CSharpClass> classDefinitions = Convert(json);

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
            throw new JsonParsingException($"Could not parse array or invalid JSON: {ex.Message}");
        }

        JsonClass jsonModel = new JsonParser().ProcessJsonProps(new JsonClass("Outer"), jsonProps);

        _logger.LogInformation("Generating class definitions");

        List<CSharpClass> classDefinitions = new ClassDefinitionGenerator().GenerateClassDefinitions(jsonModel, new List<CSharpClass>());

        _logger.LogInformation("Finalising class definitions");

        List<CSharpClass> finalisedClassDefinitions = new List<CSharpClass>();

        foreach (CSharpClass classDefinition in classDefinitions)
        {
            CSharpClass? existing = GetIfExists(finalisedClassDefinitions, classDefinition);
            if (existing == null)
            {
                finalisedClassDefinitions.Add(classDefinition);
                continue;
            }

            ReplaceNullableValuesIfCurrentIsBetter(classDefinition, existing);

            //because we have a dupe class we aren't storing we need to replace references using the existing type
            UpdateTypesInFinalClassDefinitions(finalisedClassDefinitions, classDefinition, existing);
        }

        RemoveNullFields(finalisedClassDefinitions);

        _logger.LogInformation("Created {@ClassCount} CSharp Class/es:", finalisedClassDefinitions.Count);

        _logger.LogInformation($"\nWrote output to: {_commandLineOptions.OutputPath}\n");

        return finalisedClassDefinitions;
    }

    public async Task WriteAsync(List<CSharpClass> classDefinitions, string outputPath) =>
       await File.WriteAllTextAsync(outputPath, classDefinitions.GetClassDefinitionsAsOutput());

    private void RemoveNullFields(List<CSharpClass> finalisedClassDefinitions)
    {
        finalisedClassDefinitions.ForEach(classDefinition =>
            classDefinition.Fields = classDefinition.Fields.Where(field =>
            {
                bool fieldNotNull = field.Type != "null";
                if (!fieldNotNull)
                    _logger.LogInformation("Could not generate class from Field: {@FieldName} as it has a null value", $"{classDefinition.Name}.{field.Name}");
                
                return fieldNotNull;
            }).ToList());        
    }

    private static void UpdateTypesInFinalClassDefinitions(List<CSharpClass> finalisedClasses, CSharpClass current, CSharpClass existing)
    {
        foreach (CSharpClass classDef in finalisedClasses)
            foreach (CSharpField field in classDef.Fields)
                field.Type = field.Type == current.Name //This works because we use the json key to name the type
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

    public CSharpClass? GetIfExists(List<CSharpClass> existingClasses, CSharpClass classToSearchFor) =>
        existingClasses.FirstOrDefault(finalisedClass =>
            String.Join(string.Empty, finalisedClass.Fields.Select(field => field.Name)) ==
            String.Join(string.Empty, classToSearchFor.Fields.Select(field => field.Name))
        );        
}

