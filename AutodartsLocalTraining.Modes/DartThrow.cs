namespace AutodartsLocalTraining.Modes;

public readonly record struct DartThrow(int Number, int Multiplier)
{
    public int Value => Number * Multiplier;
}
