namespace AutodartsLocalTraining.Modes;

public sealed class ThreehundredDartsHighscore : ITrainingMode
{
    private readonly List<DartThrowResult> _turnThrows = new();
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue => "Scoring";
    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => 180;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        Score += dart.Value;

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

        if (_turnCount == 100 + 1)
            IsComplete = true; 
        if (IsComplete)
            Score = Score / 100;
    }
}