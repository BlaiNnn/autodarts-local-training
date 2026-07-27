namespace AutodartsLocalTraining.Modes;

public sealed class BobsTwentyseven : ITrainingMode
{
    private const int FirstNumber = 1;
    private const int LastNumber = 20;
    private const int BullNumber = 25;
    private const int StartScore = 27;
    private const int BullValue = 50;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _currentTargetNumber = FirstNumber;
    private bool _turnJustCompleted;
    private bool _turnHadHit;

    public string PrimaryDisplayValue => _currentTargetNumber.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; } = StartScore;

    public int MaxScore => StartScore + 3 * (LastNumber * (LastNumber + 1)) + 3 * BullValue;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var isHit = dart.Number == _currentTargetNumber && dart.Multiplier == 2;
        if (isHit) Score += _currentTargetNumber * 2;
        if (isHit) _turnHadHit = true;

        _turnThrows.Add(new DartThrowResult(dart.FormatName(), isHit ? ThrowOutcome.Good : ThrowOutcome.Bad));

        if (_turnThrows.Count >= 3)
            _turnJustCompleted = true;
            
        if (_turnThrows.Count >= 3 && !_turnHadHit)
            Score -= _currentTargetNumber * 2;
            

        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _turnJustCompleted = false;
        _turnHadHit = false;

        if (_currentTargetNumber >= BullNumber)
            IsComplete = true;
        else if (_currentTargetNumber == LastNumber)
            _currentTargetNumber = BullNumber;
        else
            _currentTargetNumber++;
    }
}