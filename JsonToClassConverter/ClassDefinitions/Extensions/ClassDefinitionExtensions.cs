using JsonToClassConverter.ClassDefinitions.Models;
using Microsoft.Extensions.Logging;
using System.Text;

namespace JsonToClassConverter.ClassDefinitions.Extensions
{
    public static class ClassDefinitionExtensions
    {
        public static void PrintOutput(this List<CSharpClass> classDefinitions, ILogger logger)
        {
            foreach (CSharpClass classDefinition in classDefinitions)
            {
                logger.LogInformation($"public class {classDefinition.Name}");
                logger.LogInformation("{");

                classDefinition.Fields.ForEach(field =>
                    logger.LogInformation($"  public {field.Type}{(field.IsArray ? "[]" : string.Empty)} {field.Name}"));

                logger.LogInformation("}\n");
            }
        }

        public static string GetClassDefinitionsAsOutput(this List<CSharpClass> classDefinitions)
        {
            StringBuilder sb = new StringBuilder();

            foreach (CSharpClass classDefinition in classDefinitions)
            {
                sb.AppendLine($"public class {classDefinition.Name}");
                sb.AppendLine("{");

                classDefinition.Fields.ForEach(field =>
                    sb.AppendLine($"  public {field.Type}{(field.IsArray ? "[]" : string.Empty)} {field.Name} "));

                sb.AppendLine("}\n");
            }

            return sb.ToString();
        }

        public static string GetUppercaseFirstLetter(this string word) =>
            char.ToUpper(word[0]) + word.Substring(1);

        public static CSharpClass? GetIfExists(this List<CSharpClass> finalisedClasses, CSharpClass classDefinition) =>
            finalisedClasses.FirstOrDefault(finalisedClass =>
                String.Join(string.Empty, finalisedClass.Fields.Select(field => field.Name)) ==
                String.Join(string.Empty, classDefinition.Fields.Select(field => field.Name))
            );
    }
}