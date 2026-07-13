using System.Text.Json.Serialization;

namespace AutodartsApiSimulator.Models;

public class SimThrowInfo
{
    [JsonPropertyName("segment")]
    public SimSegment Segment { get; set; } = new();
}