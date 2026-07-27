using System.Text.Json.Serialization;

namespace AutodartsLocalTraining.Modes;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrainingMode
{
    RoundTheBoardSingleFields,
    ThreeDartCheckout,
    ThirtythreeDartsOnTwenty,
    ThirtythreeDartsOnNineteen,
    ThirtythreeDartsOnBull,
    FiftyoneDartsOnTwenty,
    FiftyoneDartsOnNineteen,
    FiftyoneDartsOnEighteen,
    Shanghai,
    RoundTheBoardDoubleFields,
}
