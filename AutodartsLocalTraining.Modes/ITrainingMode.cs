namespace AutodartsLocalTraining.Modes;

public interface ITrainingMode
{
    /// <summary>The big central display value: target number or leftover-to-checkout.</summary>
    string PrimaryDisplayValue { get; }

    IReadOnlyList<DartThrowResult> CurrentTurnThrows { get; }

    int Score { get; }

    int MaxScore { get; }

    bool IsComplete { get; }

    /// <summary>Feeds newly observed darts from the board. No-ops while a turn is pending advance or <see cref="IsComplete"/>.
    /// Returns true if this call resolved the turn (bust/checkout/completed 3 darts), awaiting <see cref="AdvanceToNextTurn"/>.</summary>
    bool ProcessThrows(IReadOnlyList<DartThrow> throws);

    /// <summary>Commits the pending turn result and moves to the next target, or completes the training.</summary>
    void AdvanceToNextTurn();
}
