using JsonToClassConverter.ClassDefinitions.Models;

namespace JsonToClassConverter.ClassDefinitions.Extensions
{
    public static class ClassDefinitionExtensions
    {
        public static void PrintOutput(this List<CSharpClass> classDefinitions)
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