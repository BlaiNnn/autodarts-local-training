namespace AutodartsLocalTraining.Modes;

public sealed class DoubleRoutine : ITrainingMode
{
    private readonly List<DartThrowResult> _turnThrows = new();
    private static readonly int[] Targets = { 20, 10, 5, 16, 8, 4, 12, 6, 3 };
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue =>
        Targets[((_turnCount - 1) / 10) * 3 + Math.Min(_turnThrows.Count, 2)].ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => 90;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var targetNumber = Targets[((_turnCount - 1) / 10) * 3 + _turnThrows.Count];

        var isHit = dart.Number == targetNumber && dart.Multiplier == 2;
        if (isHit) Score++;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(), isHit ? ThrowOutcome.Good : ThrowOutcome.Bad));

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

        if (_turnCount == 30 + 1)
            IsComplete = true;
    }
}