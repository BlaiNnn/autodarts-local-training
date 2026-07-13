using System.Text.Json.Serialization;

namespace AutodartsApiSimulator.Models;

public class SimState
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Waiting";

    [JsonPropertyName("numThrows")]
    public int NumThrows { get; set; }

    [JsonPropertyName("throws")]
    public List<SimThrowInfo> Throws { get; set; } = new();
}