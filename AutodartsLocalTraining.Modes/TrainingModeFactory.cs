namespace AutodartsLocalTraining.Modes;

public static class TrainingModeFactory
{
    public static ITrainingMode Create(TrainingMode mode) => mode switch
    {
        TrainingMode.RoundTheBoardSingleFields => new RoundTheBoardSingleFields(),
        TrainingMode.ThreeDartCheckout => new ThreeDartCheckout(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown training mode.")
    };
}
