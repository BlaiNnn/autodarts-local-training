namespace AutodartsLocalTraining.Modes;

public sealed class FiftyoneDartsOnNineteen : ITrainingMode
{
    private const int TargetNumber = 19;

    private readonly List<DartThrowResult> _turnThrows = new();
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue => TargetNumber.ToString();
    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;
    
    public int Score { get; private set; }

    public int MaxScore => 153;

    public bool IsComplete { get;  private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var isHitSingleDouble = dart.Number == TargetNumber && (dart.Multiplier == 1 || dart.Multiplier == 2);
        var isHitTripple = dart.Number == TargetNumber && dart.Multiplier == 3;
        if (isHitSingleDouble) Score++;
        if (isHitTripple) Score += 3;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(),
            (isHitTripple || isHitSingleDouble) ? ThrowOutcome.Good : ThrowOutcome.Bad));

        if (_turnThrows.Count >= 3)
            _turnJustCompleted = true;
        
        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted)  return;
        
        _turnThrows.Clear();
        _turnJustCompleted = false;
        _turnCount++;

        if (_turnCount == 17 + 1)
            IsComplete = true;
    }
}