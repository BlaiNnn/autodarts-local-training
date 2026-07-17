using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

public class Segment
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("multiplier")]
    public int Multiplier { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "-";
}