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
        TrainingMode.FiftyoneDartsOnEighteen => new FiftyoneDartsOnEighteen(),
        TrainingMode.Shanghai => new Shanghai(),
        TrainingMode.RoundTheBoardDoubleFields => new RoundTheBoardDoubleFields(),
        TrainingMode.BobsTwentyseven => new BobsTwentyseven(),
        TrainingMode.DoubleRoutine => new DoubleRoutine(),
        TrainingMode.ThreeDartCheckoutTwenty => new ThreeDartCheckoutTwenty(),
        TrainingMode.ThreeDartCheckoutNineteen => new ThreeDartCheckoutNineteen(),
        TrainingMode.ThreeDartCheckoutEighteen => new ThreeDartCheckoutEighteen(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown training mode.")
    };
}
