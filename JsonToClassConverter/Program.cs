using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        string json = JsonSerializer.Serialize(new Person
        {
            FirstName = "outer",
            SecondName = "outer",
            Age = 21,
            AChild = new Child
            {
                ChildName = "nest 1",
                Children = new List<Child>
                {
                    new Child
                    {
                        ChildName = "nest 2",
                    }
                }
            },
            //Children = new List<Child>
            //{
            //    new Child
            //    {
            //        ChildName = "nest 1",
            //        Children = new List<Child>
            //        {
            //            new Child
            //            {
            //                ChildName = "nest 2",
            //            }
            //        }
            //    },
            //}
        });

        DocumentModel model = GetDocumentModel(json);

        //PrintModel(model);
    }

    private static DocumentModel GetDocumentModel(string json)
        => ParseJson(JsonDocument.Parse(json).RootElement);

    private static DocumentModel ParseJson(JsonElement jsonElement)
    {
        DocumentModel model = new DocumentModel();

        foreach (JsonProperty property in jsonElement.EnumerateObject())
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                case JsonValueKind.Array:
                    var childObject = new ChildObject(property.Name, GetValueType(property.Value.ValueKind, property.Value.GetString()))
                    {
                        Fields = ParseJson(property.Value).Fields,
                        ChildObjects = ParseJson(property.Value).ChildObjects,
                        Arrays = ParseJson(property.Value).Arrays
                    };
                    model.ChildObjects.Add(childObject);
                    break;

                case JsonValueKind.String:
                    var fieldType = GetValueType(property.Value.ValueKind, property.Value.GetString());
                    model.Fields.Add(new Field(property.Name, fieldType));
                    break;

                //case JsonValueKind.Null:
                //    // Add a nullable field type for null values
                //    model.Fields.Add(new Field(property.Name, typeof(Nullable<>)));
                //    break;

                default:
                    if (IsValueType(property.Value.ValueKind))
                    {
                        model.Fields.Add(new Field(property.Name, GetValueType(property.Value.ValueKind)));
                    }
                    break;
            }
        }

        return model;
    }

    private static bool IsValueType(JsonValueKind valueKind) =>
     valueKind == JsonValueKind.False ||
     valueKind == JsonValueKind.True ||
     valueKind == JsonValueKind.String ||
     valueKind == JsonValueKind.Number ||
     valueKind == JsonValueKind.Null;

    private static Type GetValueType(JsonValueKind valueKind, string? stringValue = null) =>
        valueKind switch
        {
            JsonValueKind.True or JsonValueKind.False => typeof(bool),
            JsonValueKind.String => DetectStringType(stringValue),
            JsonValueKind.Number => typeof(double),
            JsonValueKind.Null => typeof(Nullable<>), // Nullable type for null values
            _ => throw new Exception($"Unsupported JSON value type: {valueKind}")
        };

    private static Type DetectStringType(string? value) => DateTime.TryParse(value, out _)
        ? typeof(DateTime)
        : typeof(string);

    //private static void PrintModel(DocumentModel model, int indent = 0)
    //{
    //    string indentStr = new string(' ', indent);
    //    Console.WriteLine($"{indentStr}DocumentModel:");

    //    foreach (var field in model.Fields)
    //    {
    //        Console.WriteLine($"{indentStr}  Field: {field.Name} ({field.Type.Name})");
    //    }

    //    foreach (var array in model.Arrays)
    //    {
    //        Console.WriteLine($"{indentStr}  ArrayField: {array.Name}");
    //        foreach (var primitiveValue in array.PrimitiveValues)
    //        {
    //            Console.WriteLine($"{indentStr}    PrimitiveValue: {primitiveValue}");
    //        }
    //        foreach (var obj in array.ObjectValues)
    //        {
    //            PrintModel(obj, indent + 4);
    //        }
    //    }

    //    foreach (var child in model.ChildObjects)
    //    {
    //        Console.WriteLine($"{indentStr}  ChildObject: {child.Name}");
    //        PrintModel(new DocumentModel { Fields = child.Fields, ChildObjects = child.ChildObjects, Arrays = child.Arrays }, indent + 4);
    //    }
    //}

    // Model classes
    public class DocumentModel
    {
        public List<Field> Fields { get; set; } = new List<Field>();
        public List<ChildObject> ChildObjects { get; set; } = new List<ChildObject>();
        public List<ArrayField> Arrays { get; set; } = new List<ArrayField>();
    }

    public class ChildObject : DocumentModel
    {
        public ChildObject(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class Field
    {
        public Field(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; set; }
        public Type Type { get; set; }
    }

    public class ArrayField
    {
        public ArrayField(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<string> PrimitiveValues { get; set; } = new List<string>();
        public List<DocumentModel> ObjectValues { get; set; } = new List<DocumentModel>();
    }

    // Sample data classes
    public class Person
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int Age { get; set; }
        public Child AChild { get; set; }
        //public List<Child> Children { get; set; }
    }

    public class Child
    {
        public string ChildName { get; set; }
        public List<Child> Children { get; set; }
    }
}
