using JsonToClassConverter.JsonParsing.Models;
using System.Text.Json;

namespace JsonToClassConverter.JsonParsing
{
    public interface IJsonParser
    {
        JsonClass ProcessJsonProps(JsonClass model, JsonElement.ObjectEnumerator jsonObject);
    }
}