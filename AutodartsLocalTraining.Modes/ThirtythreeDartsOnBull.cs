namespace AutodartsLocalTraining.Modes;

public sealed class ThirtythreeDartsOnBull : ITrainingMode
{
    private const int TargetNumber = 25;

    private readonly List<DartThrowResult> _turnThrows = new();
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue => TargetNumber.ToString();
    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;
    
    public int Score { get; private set; }

    public int MaxScore => 66;

    public bool IsComplete { get;  private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var isHitSingle = dart.Number == TargetNumber && dart.Multiplier == 1;
        var isHitDouble = dart.Number == TargetNumber && dart.Multiplier == 2;
        if (isHitSingle) Score++;
        if (isHitDouble) Score += 2;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(),
            (isHitSingle || isHitDouble) ? ThrowOutcome.Good : ThrowOutcome.Bad));

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

        if (_turnCount == 11 + 1)
            IsComplete = true;
    }
}