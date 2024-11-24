using JsonToClassConverter.ClassDefinitions.Models;
using JsonToClassConverter.JsonParsing.Models;

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
                        GetValueType(field.Type), //ToDo: Get the value type for string, bool + double
                        field.IsArray));

            foreach (JsonClass child in model.Children)
                classDefinition.Fields.Add(new CSharpField(child.Name, child.Name, child.IsArray));

            classDefinitions.Add(classDefinition);

            foreach (JsonClass child in model.Children)
                GenerateClassDefinitions(child, classDefinitions);

            return classDefinitions;
        }

        private string GetValueType(Type type)
        {
            if (type == typeof(String))
                return "string";
            else if (type == typeof(DateTime))
                return "DateTime";
            else if (type == typeof(Double))
                return "double";
            else if (type == typeof(Boolean))
                return "bool";
            else if (type.Name == "Nullable`1")
                return "null";
            else
                throw new Exception($"Unrecognised type: {type.Name}");
        }
    }
}