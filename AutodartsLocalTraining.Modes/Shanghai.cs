namespace AutodartsLocalTraining.Modes;

public sealed class Shanghai : ITrainingMode
{
    private const int FirstNumber = 1;
    private const int LastNumber = 20;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _currentTargetNumber = FirstNumber;
    private bool _turnJustCompleted;
    private bool _hitSingle;
    private bool _hitDouble;
    private bool _hitTripple;

    public string PrimaryDisplayValue => _currentTargetNumber.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => 240;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var isHitSingle = dart.Number == _currentTargetNumber && dart.Multiplier == 1;
        var isHitDouble = dart.Number == _currentTargetNumber && dart.Multiplier == 2;
        var isHitTripple = dart.Number == _currentTargetNumber && dart.Multiplier == 3;
        if (isHitSingle)
        {
            Score++;
            _hitSingle = true;
        }
        if (isHitDouble)
        {
            Score += 2;
            _hitDouble = true;
        }
        if (isHitTripple)
        {
            Score += 3;
            _hitTripple = true;
        }

        _turnThrows.Add(new DartThrowResult(dart.FormatName(), (isHitSingle || isHitDouble || isHitTripple) ? ThrowOutcome.Good : ThrowOutcome.Bad));

        if (_turnThrows.Count >= 3)
            _turnJustCompleted = true;
        
        // Official Shanghai rule: hitting single, double AND triple of the current
        // number within the same turn (in any order/dart) gives a bonus of 6 -
        // e.g. 1+2+3=6 normal points, doubled to 12 total for a "Shanghai".
        if (_turnJustCompleted && _hitSingle && _hitDouble && _hitTripple)
            Score += 6;
        
        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _turnJustCompleted = false;
        _hitSingle = false;
        _hitDouble = false;
        _hitTripple = false;

        if (_currentTargetNumber >= LastNumber)
            IsComplete = true;
        else
            _currentTargetNumber++;
    }
}
