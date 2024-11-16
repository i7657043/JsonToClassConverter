using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing.Models;

namespace JsonToClassConverter.ClassDefinitions
{
    public interface IClassDefinitionGenerator
    {
        List<CSharpClass> GenerateClassDefinitions(JsonClass model, List<CSharpClass> classDefinitions);
    }
}