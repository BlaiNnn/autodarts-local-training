using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

public class TrainingProgram
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("sequence")]
    public List<TrainingStep> Sequence { get; set; } = new();
}
