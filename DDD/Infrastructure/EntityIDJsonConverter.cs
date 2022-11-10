using System.Text.Json;
using System.Text.Json.Serialization;
using DDD.Domain.Core;

namespace DDD.Infrastructure;

// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0#steps-to-follow-the-factory-pattern
public class EntityIDJsonConverter : JsonConverter<EntityID>
{
    public override bool CanConvert(Type typeToConvert)
        => typeof(EntityID).IsAssignableFrom(typeToConvert);

    public override EntityID? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotImplementedException();

    public override void Write(Utf8JsonWriter writer, EntityID value, JsonSerializerOptions options)
        => writer.WriteRawValue(value.Value.ToString());
}
