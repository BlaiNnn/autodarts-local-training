using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

public class ThrowInfo
{
    [JsonPropertyName("segment")]
    public Segment Segment { get; set; } = new();
}