using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

public class AutodartsState
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Unknown";

    [JsonPropertyName("event")]
    public JsonElement Event { get; set; }

    [JsonPropertyName("numThrows")]
    public int NumThrows { get; set; }

    [JsonPropertyName("throws")]
    public List<ThrowInfo> Throws { get; set; } = new();
}