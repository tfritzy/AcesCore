

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AcesCore
{
    [JsonConverter(typeof(EventConverter))]
    public abstract class Event
    {
        public abstract EventType Type { get; }
    }

    public class EventConverter : JsonConverter
    {
        private static readonly Dictionary<EventType, Type> TypeMap = new Dictionary<EventType, Type>
        {
            { EventType.AdvanceRound, typeof(AdvanceRoundEvent) },
            { EventType.AdvanceTurn, typeof(AdvanceTurnEvent) },
            { EventType.Discard, typeof(DiscardEvent) },
            { EventType.DrawFromDeck, typeof(DrawFromDeckEvent) },
            { EventType.DrawFromPile, typeof(DrawFromPileEvent) },
            { EventType.GameEndEvent, typeof(GameEndEvent) },
            { EventType.JoinGame, typeof(JoinGameEvent) },
            { EventType.PlayerWentOut, typeof(PlayerWentOutEvent) },
            { EventType.StartGame, typeof(StartGameEvent)},
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(Event).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var typeString = jsonObject.GetValue("type", StringComparison.OrdinalIgnoreCase)?.Value<string>();
            if (!Enum.TryParse<EventType>(typeString, true, out EventType EventType))
            {
                throw new JsonSerializationException($"Invalid component type: {typeString}");
            }

            if (!TypeMap.TryGetValue(EventType, out var targetType))
            {
                throw new InvalidOperationException($"Didn't add '{EventType}' type to dictionary");
            }

            object? target = Activator.CreateInstance(targetType);

            if (target == null)
            {
                throw new InvalidOperationException($"Failed to create instance of type '{targetType}'");
            }

            serializer.Populate(jsonObject.CreateReader(), target);

            if (!(target is Event))
            {
                throw new InvalidOperationException($"Created instance of type '{targetType}' is not a schema.Character");
            }

            return (Event)target;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}