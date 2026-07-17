namespace AutodartsLocalTraining;

public enum ThrowOutcome
{
    Neutral,
    Good,
    Bad
}

public sealed record DartThrowResult(string DisplayText, ThrowOutcome Outcome);
