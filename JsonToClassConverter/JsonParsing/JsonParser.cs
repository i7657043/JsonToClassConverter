using JsonToClassConverter.JsonParsing.Models;
using System.Text.Json;

namespace JsonToClassConverter.JsonParsing
{
    public class JsonParser : IJsonParser
    {
        public JsonClass ProcessJsonProps(JsonClass model, JsonElement.ObjectEnumerator jsonObject)
        {
            foreach (JsonProperty property in jsonObject)
            {
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Object:
                        JsonClass childObj = ProcessJsonProps(new JsonClass(property.Name), property.Value.EnumerateObject());
                        model.Children.Add(childObj);
                        break;

                    case JsonValueKind.Array:
                        if (IsEmptyArray(property))
                            break;
                        else if (IsValueTypeArray(property))
                        {
                            model.Fields.Add(new JsonField(property.Name, GetValueType(property.Value.EnumerateArray().First().ValueKind)) { IsArray = true });
                            break;
                        }
                        //We only pass the first indice of the array in as we only need to map the values to a new class once. No hanlding of polymorphic array
                        JsonClass childArray = ProcessJsonProps(new JsonClass(property.Name), property.Value.EnumerateArray().First().EnumerateObject());
                        childArray.IsArray = true;
                        model.Children.Add(childArray);
                        break;

                    case JsonValueKind.String:
                        Type fieldType = GetValueType(property.Value.ValueKind, property.Value.GetString());
                        model.Fields.Add(new JsonField(property.Name, fieldType));
                        break;

                    case JsonValueKind.Null:
                        model.Fields.Add(new JsonField(property.Name, typeof(Nullable<>)));
                        break;

                    case JsonValueKind.Number:
                        model.Fields.Add(new JsonField(property.Name, typeof(double)));
                        break;

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        model.Fields.Add(new JsonField(property.Name, typeof(bool)));
                        break;
                }
            }

            return model;
        }

        private bool IsValueTypeArray(JsonProperty property) => !property.Value.ToString().Contains("[{");

        private bool IsEmptyArray(JsonProperty property) => property.Value.ToString() == "[]";

        private Type GetValueType(JsonValueKind valueKind, string? stringValue = null) =>
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
    }
}