using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing.Models;
using Microsoft.Extensions.Logging;

namespace JsonToClassConverter.ClassDefinitions
{
    public class ClassDefinitionGenerator : IClassDefinitionGenerator
    {
        public List<CSharpClass> GenerateClassDefinitions(JsonClass model, List<CSharpClass> classDefinitions)
        {
            CSharpClass classDefinition = new CSharpClass(model.Name);

            foreach (JsonField field in model.Fields)
                classDefinition.Fields.Add(
                    new CSharpField(
                        field.Name,
                        field.Type.Name,
                        field.IsArray));

            foreach (JsonClass child in model.Children)
                classDefinition.Fields.Add(new CSharpField(child.Name, child.Name, child.IsArray));

            classDefinitions.Add(classDefinition);

            foreach (JsonClass child in model.Children)
                GenerateClassDefinitions(child, classDefinitions);

            return classDefinitions;
        }
    }
}