using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        string json = JsonSerializer.Serialize(new Person()
        {
            FirstName = "fname",
            Jobs = new string[] { "job1", "job2" },
            //Dob = DateTime.Now,
            CarProps = new List<Car>()
            {
                new Car()
                {
                    Brand = "child vw",
                    Owners = new List<Owner>
                    {
                        new Owner()
                        {
                            Details = "detailshere",
                            Credentials = new List<Creds>()
                            {
                                new Creds(){ TheText = "texthere" }
                            }
                        }
                    },
                    VehicleInfo = new Info { Seats = 2 }
                }
            }
            //});
        });

        JsonClass model = GetDocumentModel(json);
        model.Name = "Outer";

    }
    private static JsonClass GetDocumentModel(string json)
        => ParseJson(JsonDocument.Parse(json).RootElement);

    private static JsonClass ParseJson(JsonElement jsonElement)
    {
        JsonElement.ObjectEnumerator obj = jsonElement.EnumerateObject();

        JsonClass model = ProcessJsonProps(new JsonClass(), obj);

        return model;
    }

    private static JsonClass ParseArray(JsonElement arrayElement)
    {
        JsonElement.ObjectEnumerator obj = arrayElement.EnumerateArray().First().EnumerateObject();

        JsonClass model = ProcessJsonProps(new JsonClass(), obj);

        return model;
    }

    private static JsonClass ProcessJsonProps(JsonClass model, JsonElement.ObjectEnumerator obj)
    {
        foreach (JsonProperty property in obj)
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    JsonClass childObj = ParseJson(property.Value);
                    childObj.Name = property.Name;
                    model.Children.Add(childObj);
                    break;

                case JsonValueKind.Array:
                    if (IsEmptyArray(property))
                        break;
                    else if (IsValueTypeArray(property))
                    {
                        Field field = new Field(property.Name, GetValueType(property.Value.EnumerateArray().First().ValueKind)) { IsArray = true };
                        model.Fields.Add(field);
                        break;
                    }
                    JsonClass childArray = ParseArray(property.Value);
                    childArray.Name = property.Name;
                    childArray.IsArray = true;
                    model.Children.Add(childArray);
                    break;

                case JsonValueKind.String:
                    Type fieldType = GetValueType(property.Value.ValueKind, property.Value.GetString());
                    model.Fields.Add(new Field(property.Name, fieldType));
                    break;

                case JsonValueKind.Null:
                    model.Fields.Add(new Field(property.Name, typeof(Nullable<>)));
                    break;

                case JsonValueKind.Number:
                    model.Fields.Add(new Field(property.Name, GetValueType(property.Value.ValueKind)));
                    break;
            }
        }

        return model;
    }

    private static bool IsValueTypeArray(JsonProperty property)
    {
        return !property.Value.ToString().Contains("{");
    }

    private static bool IsEmptyArray(JsonProperty property)
    {
        return property.Value.ToString() == "[]";
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





    public class Person
    {
        public string FirstName { get; set; }
        public List<Car> CarProps { get; set; } = new List<Car>();
        public string[] Jobs { get; set; }

    }

    public class Car
    {
        public string Brand { get; set; }
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
