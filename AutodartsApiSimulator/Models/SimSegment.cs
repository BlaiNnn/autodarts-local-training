using System.Text.Json.Serialization;

namespace AutodartsApiSimulator.Models;

public class SimSegment
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("multiplier")]
    public int Multiplier { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "-";
}