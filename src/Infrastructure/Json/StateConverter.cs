using System.Text.Json.Serialization;

using FFCmd.Dto.Github;

namespace FFCmd.Infrastructure.Json;

internal sealed class StateConverter : JsonConverter<State>
{
    public override bool CanConvert(Type t) => t == typeof(State);

    public override State Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        switch (value)
        {
            case "open":
                return State.Open;
            case "uploaded":
                return State.Uploaded;
        }
        throw new Exception("Cannot unmarshal type State");
    }

    public override void Write(Utf8JsonWriter writer, State value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case State.Open:
                JsonSerializer.Serialize(writer, "open", options);
                return;
            case State.Uploaded:
                JsonSerializer.Serialize(writer, "uploaded", options);
                return;
        }
        throw new Exception("Cannot marshal type State");
    }

    public static readonly StateConverter Singleton = new StateConverter();
}
