using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrainingMode
{
    [JsonStringEnumMemberName("Round the Board - Single fields")]
    RoundTheBoardSingleFields
}