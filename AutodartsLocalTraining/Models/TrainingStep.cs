using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

public class TrainingStep
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("mode")]
    public TrainingMode Mode { get; set; }
}