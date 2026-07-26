namespace AutodartsLocalTraining.Modes;

public static class TrainingModeFactory
{
    public static ITrainingMode Create(TrainingMode mode) => mode switch
    {
        TrainingMode.RoundTheBoardSingleFields => new RoundTheBoardSingleFields(),
        TrainingMode.ThreeDartCheckout => new ThreeDartCheckout(),
        TrainingMode.ThirtythreeDartsOnTwenty => new ThirtythreeDartsOnTwenty(),
        TrainingMode.ThirtythreeDartsOnNineteen => new ThirtythreeDartsOnNineteen(),
        TrainingMode.ThirtythreeDartsOnBull => new ThirtythreeDartsOnBull(),
        TrainingMode.FiftyoneDartsOnTwenty => new FiftyoneDartsOnTwenty(),
        TrainingMode.FiftyoneDartsOnNineteen => new FiftyoneDartsOnNineteen(),
        TrainingMode.FiftyoneDartsOnEightteen => new FiftyoneDartsOnEighteen(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown training mode.")
    };
}
