using JsonToClassConverter.ClassDefinitions.Models;

public interface IConverterController
{
    Task Run();
    List<CSharpClass> Convert(string json);
    Task WriteAsync(List<CSharpClass> classDefinitions, string outputPath);
}

