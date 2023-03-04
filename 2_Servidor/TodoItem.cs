using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace servidorsincrono
{
    public class TodoItem
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("isComplete")]
        public bool IsComplete { get; set; }


        public override string ToString()
        {
            var s = IsComplete ? "+" : "-";
            return $"{s} {Id} {Name}";
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static TodoItem FromJson(string json)
        {
            return JsonSerializer.Deserialize<TodoItem>(json);
        }

        public static List<TodoItem> ListFromJson(string json)
        {
            return JsonSerializer.Deserialize<List<TodoItem>>(json);
        }

    }

}
