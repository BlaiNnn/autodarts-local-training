using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Modes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrainingMode
{
    RoundTheBoardSingleFields,
    ThreeDartCheckout
}
