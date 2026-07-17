using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.BoardSimulator.Models;

public class SimThrowInfo
{
    [JsonPropertyName("segment")]
    public SimSegment Segment { get; set; } = new();
}