using System.Text.Json;

internal class Program2
{
    private static void Main2(string[] args)
    {
        string json = JsonSerializer.Serialize(new Person
        {
            FirstName = "hi",
            SecondName = "hello",
            NonName = null,
            Age = 21,
            Male = false,
            Children = new List<Child>
            {
                new Child
                {
                    NonName = null,
                    ChildName = "yoyo",
                    ChildOfChild = new ChildOfChild
                    {
                        ChildOfChildName = "nest",
                        Age = 21,
                        Dob = DateTime.Now,
                        Male = true
                    },
                    Children = new List<Child>
                    {
                        new Child
                        {
                            NonName = null,
                            ChildName = "nest 1",
                            ChildOfChild = new ChildOfChild
                            {
                                ChildOfChildName = "nest 1",
                                Age = 21,
                                Dob = DateTime.Now,
                                Male = true
                            }
                        },
                        new Child
                        {
                            NonName = null,
                            ChildName = "anotherChild",
                            Age = 21,
                            Dob = DateTime.Now,
                            Children = new List<Child>
                            {
                                new Child
                                {
                                    NonName = null,
                                    ChildName = "nest 2",
                                    ChildOfChild = new ChildOfChild
                                    {
                                        ChildOfChildName = "nest 2",
                                        Age = 21,
                                        Dob = DateTime.Now,
                                        Male = true
                                    }
                                },
                                new Child
                                {
                                    NonName = null,
                                    ChildName = "anotherChild",
                                    Age = 21,
                                    Dob = DateTime.Now
                                }
                            }
                        }
                    }
                },
                new Child
                {
                    NonName = null,
                    ChildName = "nest",
                    Age = 21,
                    Dob = DateTime.Now
                }
            },
            Child = new Child
            {
                NonName = null,
                ChildName = "extra",
                ChildOfChild = new ChildOfChild
                {
                    ChildOfChildName = "extra child of child"
                },
                Age = 21,
                Dob = DateTime.Now,
                Male = true
            }
        });

        DocumentModel model = GetDocumentModel(json);

        PrintModel(model);
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
                    var childObject = new ChildObject(property.Name)
                    {
                        Fields = ParseJson(property.Value).Fields,
                        ChildObjects = ParseJson(property.Value).ChildObjects,
                        Arrays = ParseJson(property.Value).Arrays
                    };
                    model.ChildObjects.Add(childObject);
                    break;

                case JsonValueKind.Array:
                    var arrayField = ParseArray(property.Name, property.Value);
                    model.Arrays.Add(arrayField);
                    break;

                case JsonValueKind.String:
                    var fieldType = GetValueType(property.Value.ValueKind, property.Value.GetString());
                    model.Fields.Add(new Field(property.Name, fieldType));
                    break;

                case JsonValueKind.Null:
                    // Add a nullable field type for null values
                    model.Fields.Add(new Field(property.Name, typeof(Nullable<>)));
                    break;

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

    private static ArrayField ParseArray(string name, JsonElement arrayElement)
    {
        var arrayField = new ArrayField(name);

        foreach (var item in arrayElement.EnumerateArray())
        {
            if (IsValueType(item.ValueKind))
            {
                arrayField.PrimitiveValues.Add(item.ToString());
            }
            else if (item.ValueKind == JsonValueKind.Object)
            {
                arrayField.ObjectValues.Add(ParseJson(item));
            }
        }

        return arrayField;
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

    private static void PrintModel(DocumentModel model, int indent = 0)
    {
        string indentStr = new string(' ', indent);
        Console.WriteLine($"{indentStr}DocumentModel:");

        foreach (var field in model.Fields)
        {
            Console.WriteLine($"{indentStr}  Field: {field.Name} ({field.Type.Name})");
        }

        foreach (var array in model.Arrays)
        {
            Console.WriteLine($"{indentStr}  ArrayField: {array.Name}");
            foreach (var primitiveValue in array.PrimitiveValues)
            {
                Console.WriteLine($"{indentStr}    PrimitiveValue: {primitiveValue}");
            }
            foreach (var obj in array.ObjectValues)
            {
                PrintModel(obj, indent + 4);
            }
        }

        foreach (var child in model.ChildObjects)
        {
            Console.WriteLine($"{indentStr}  ChildObject: {child.Name}");
            PrintModel(new DocumentModel { Fields = child.Fields, ChildObjects = child.ChildObjects, Arrays = child.Arrays }, indent + 4);
        }
    }

    // Model classes
    public class DocumentModel
    {
        public List<Field> Fields { get; set; } = new List<Field>();
        public List<ChildObject> ChildObjects { get; set; } = new List<ChildObject>();
        public List<ArrayField> Arrays { get; set; } = new List<ArrayField>();
    }

    public class ChildObject : DocumentModel
    {
        public ChildObject(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
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
        public string? NonName { get; set; } = null;
        public int Age { get; set; }
        public bool Male { get; set; }
        public DateTime Dob { get; set; }
        public List<Child> Children { get; set; }
        public Child Child { get; set; }
    }

    public class Child
    {
        public string ChildName { get; set; }
        public string? NonName { get; set; } = null;
        public int Age { get; set; }
        public bool Male { get; set; }
        public DateTime Dob { get; set; }
        public List<Child> Children { get; set; }
        public ChildOfChild ChildOfChild { get; set; }
    }

    public class ChildOfChild
    {
        public int Age { get; set; }
        public bool Male { get; set; }
        public DateTime Dob { get; set; }
        public string ChildOfChildName { get; set; }
    }
}
