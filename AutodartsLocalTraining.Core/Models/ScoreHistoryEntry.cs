namespace AutodartsLocalTraining.Models;

public sealed record ScoreHistoryEntry
{
    public required DateTime Date { get; init; }
    public required int Score { get; init; }
}
