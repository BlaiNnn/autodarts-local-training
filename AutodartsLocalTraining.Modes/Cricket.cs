namespace AutodartsLocalTraining.Modes;

public sealed class CricketScoring : ITrainingMode
{
    private const int FirstNumber = 15;
    private const int LastNumber = 20;
    private const int BullNumber = 25;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _currentTargetNumber = FirstNumber;
    private bool _turnJustCompleted;

    public string PrimaryDisplayValue => _currentTargetNumber.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => (LastNumber - FirstNumber + 1) * 9 + 6;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var isHitSingle = dart.Number == _currentTargetNumber && dart.Multiplier == 1;
        var isHitDouble = dart.Number == _currentTargetNumber && dart.Multiplier == 2;
        var isHitTripple = dart.Number == _currentTargetNumber && dart.Multiplier == 3;
        if (isHitSingle) Score++;
        if (isHitDouble) Score += 2;
        if (isHitTripple) Score += 3;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(), 
            (isHitSingle || isHitDouble || isHitTripple) ? ThrowOutcome.Good : ThrowOutcome.Bad));

        if (_turnThrows.Count >= 3)
            _turnJustCompleted = true;

        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _turnJustCompleted = false;

        if (_currentTargetNumber >= BullNumber)
            IsComplete = true;
        else if (_currentTargetNumber == LastNumber)
            _currentTargetNumber = BullNumber;
        else
            _currentTargetNumber++;
    }
}
