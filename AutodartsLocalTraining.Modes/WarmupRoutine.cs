namespace AutodartsLocalTraining.Modes;

public sealed class WarmupRoutine : ITrainingMode
{
    private readonly List<DartThrowResult> _turnThrows = new();
    private static readonly int[] Targets = { 25, 20, 10, 5, 16, 8, 4, 12, 6, 3 };
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue =>
        _turnCount <= 3
            ? Targets[0].ToString()
            : Targets[1 + ((_turnCount - 4) / 3) * 3 + Math.Min(_turnThrows.Count, 2)].ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => 0;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(), ThrowOutcome.Neutral));

        if (_turnThrows.Count >= 3)
            _turnJustCompleted = true;

        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _turnJustCompleted = false;
        _turnCount++;

        if (_turnCount == 12 + 1)
            IsComplete = true;
    }
}