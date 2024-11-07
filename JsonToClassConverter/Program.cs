using System.Text.Json;

internal class Program
{    
    private static void Main(string[] args)
    {
        string json = JsonSerializer.Serialize(new Person()
        {
            CarProp = new Car()
            {
                Brand = "vw",
                Age = 21,
                Owners = new List<Owner>()
                {
                    new Owner()
                    {
                        Details = "new deets",
                        Credentials = new List<Creds>()
                        {
                            new Creds()
                            {
                                TheText = "credstext"
                            }
                        }
                    }
                },
                VehicleInfo = new Info()
                {
                    Seats = 2
                }
            },
            Jobs = new string[] { "job1" },
            FirstName = "fname",
            CarProps = new List<Car>
            {
                new Car()
                {
                    Brand = "child vw"
                }
            }
        });

        JsonClass model = ProcessJsonProps(new JsonClass(), JsonDocument.Parse(json).RootElement.EnumerateObject());
        model.Name = "Outer";

        List<CSharpClass> classDefinitions = GenerateClassDefinitions(model, new List<CSharpClass>());

        PrintOutput(classDefinitions);
    }    

    private static List<CSharpClass> GenerateClassDefinitions(JsonClass model, List<CSharpClass> classDefinitions)
    {
        CSharpClass classDefinition = new CSharpClass(model.Name);

        foreach (Field field in model.Fields)
            classDefinition.Fields.Add(
                new CSharpField(
                    field.Name, 
                    field.Type.Name == typeof(string).Name ? $"string" : field.Type.Name,
                    field.IsArray));

        foreach (JsonClass child in model.Children)
            classDefinition.Fields.Add(new CSharpField(child.Name, child.Name, child.IsArray));

        CSharpClass? duplicate = GetDuplicate(classDefinitions, classDefinition);

        if (duplicate == null)
            classDefinitions.Add(classDefinition);
        else
            //As we aren't writing this new class definition to output (because its a dupe), we need to modify any references to it in other classes
            UpdateTypeReferences(classDefinitions, classDefinition, duplicate);

        foreach (JsonClass child in model.Children)
            GenerateClassDefinitions(child, classDefinitions);

        return classDefinitions;
    }

    private static void UpdateTypeReferences(List<CSharpClass> classDefinitions, CSharpClass classDefinition, CSharpClass duplicate)
    {
        classDefinitions.ForEach(classDef =>
        {
            classDef.Fields.ForEach(field =>
            {
                if (field.Type == classDefinition.Name)
                    field.Type = duplicate.Name;
            });
        });
    }

    private static CSharpClass? GetDuplicate(List<CSharpClass> classDefinitions, CSharpClass classDefinition) =>
        classDefinitions.FirstOrDefault(
            classDef => String.Join(string.Empty,classDef.Fields.Select(x => x.Name)) == 
            String.Join(string.Empty, classDefinition.Fields.Select(x => x.Name)));

    public class CSharpClass
    {
        public CSharpClass(string name)
        {
            Name = name;
        }

        public string Name { get; set; } = string.Empty;
        public List<CSharpField> Fields { get; set; } = new List<CSharpField>();
    }

    public class CSharpField
    {
        public CSharpField(string name, string type, bool isArray)
        {
            Name = name;
            Type = type;
            IsArray = isArray;
        }

        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsArray { get; set; }
    }


    ////////////////////////////////////////////////////////////////////////////////////

    private static JsonClass ProcessJsonProps(JsonClass model, JsonElement.ObjectEnumerator obj)
    {
        foreach (JsonProperty property in obj)
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    JsonClass childObj = ProcessJsonProps(new JsonClass(), property.Value.EnumerateObject());
                    childObj.Name = property.Name;
                    model.Children.Add(childObj);
                    break;

                case JsonValueKind.Array:
                    if (IsEmptyArray(property))
                        break;
                    else if (IsValueTypeArray(property))
                    {
                        model.Fields.Add(new Field(property.Name, GetValueType(property.Value.EnumerateArray().First().ValueKind)) { IsArray = true });
                        break;
                    }
                    //We only pass the first indice of the array in as we only need to map the values to a new class once
                    //No hanlding of using multiple types in the same array. Who would want to do that anyway?
                    JsonClass childArray = ProcessJsonProps(new JsonClass(), property.Value.EnumerateArray().First().EnumerateObject());
                    childArray.Name = property.Name;
                    childArray.IsArray = true;
                    model.Children.Add(childArray);
                    break;

                case JsonValueKind.String:
                    Type fieldType = GetValueType(property.Value.ValueKind, property.Value.GetString());
                    model.Fields.Add(new Field(property.Name, fieldType));
                    break;

                case JsonValueKind.Null:
                    //model.Fields.Add(new Field(property.Name, typeof(Nullable<>)));
                    break;

                case JsonValueKind.Number:
                    model.Fields.Add(new Field(property.Name, GetValueType(property.Value.ValueKind)));
                    break;
            }
        }

        return model;
    }

    private static bool IsValueTypeArray(JsonProperty property) => !property.Value.ToString().Contains("[{");

    private static bool IsEmptyArray(JsonProperty property) => property.Value.ToString() == "[]";

    private static Type GetValueType(JsonValueKind valueKind, string? stringValue = null) =>
        valueKind switch
        {
            JsonValueKind.True or JsonValueKind.False => typeof(bool),
            JsonValueKind.String =>
                DateTime.TryParse(stringValue, out _)
                    ? typeof(DateTime)
                    : typeof(string),
            JsonValueKind.Number => typeof(double),
            JsonValueKind.Null => typeof(Nullable<>),
            _ => throw new Exception($"Unsupported JSON value type: {valueKind}")
        };

    public class JsonClass
    {
        public string Name { get; set; } = string.Empty;
        public List<Field> Fields { get; set; } = new List<Field>();
        public List<JsonClass> Children { get; set; } = new List<JsonClass>();
        public bool IsArray { get; set; }
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
        public bool IsArray { get; set; }
    }

    /////////////////////

    private static void PrintOutput(List<CSharpClass> classDefinitions)
    {
        foreach (CSharpClass classDefinition in classDefinitions)
        {
            Console.WriteLine($"public class {classDefinition.Name}");
            Console.WriteLine("{");
            classDefinition.Fields.ForEach(field => Console.WriteLine($"  public {field.Name} {field.Type}{(field.IsArray ? "[]" : string.Empty)}"));
            Console.WriteLine("}\n");
        }
    }



    public class Person
    {
        public string FirstName { get; set; }
        public List<Car> CarProps { get; set; } = new List<Car>();
        public Car CarProp { get; set; } = new Car();
        public string[] Jobs { get; set; }

    }

    public class Car
    {
        public string Brand { get; set; }
        public int Age { get; set; }
        public Info VehicleInfo { get; set; }
        public List<Owner> Owners { get; set; }
    }

    public class Info
    {
        public int Seats { get; set; }
    }

    public class Owner
    {
        public string Details { get; set; }
        public List<Creds> Credentials { get; set; }
    }

    public class Creds
    {
        public string TheText { get; set; }
    }
}
