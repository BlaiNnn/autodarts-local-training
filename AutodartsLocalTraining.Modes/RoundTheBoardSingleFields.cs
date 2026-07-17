namespace AutodartsLocalTraining.Modes;

public sealed class RoundTheBoardSingleFields : ITrainingMode
{
    private const int FirstNumber = 1;
    private const int LastNumber = 20;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _apiThrowsSeenCount;
    private int _currentTargetNumber = FirstNumber;
    private bool _turnJustCompleted;

    public string PrimaryDisplayValue => _currentTargetNumber.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => (LastNumber - FirstNumber + 1) * 3;

    public bool IsComplete { get; private set; }

    public bool ProcessThrows(IReadOnlyList<DartThrow> throws)
    {
        if (IsComplete || _turnJustCompleted) return false;

        if (throws.Count < _apiThrowsSeenCount)
        {
            _turnThrows.Clear();
            _apiThrowsSeenCount = 0;
        }

        for (var i = _apiThrowsSeenCount; i < throws.Count; i++)
        {
            var dart = throws[i];
            var isHit = dart.Number == _currentTargetNumber && dart.Multiplier == 1;
            if (isHit) Score++;

            _turnThrows.Add(new DartThrowResult(FormatName(dart), isHit ? ThrowOutcome.Good : ThrowOutcome.Bad));

            if (_turnThrows.Count >= 3)
            {
                _turnJustCompleted = true;
                break;
            }
        }

        _apiThrowsSeenCount = throws.Count;
        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _apiThrowsSeenCount = 0;
        _turnJustCompleted = false;

        if (_currentTargetNumber >= LastNumber)
            IsComplete = true;
        else
            _currentTargetNumber++;
    }

    private static string FormatName(DartThrow dart) => dart.Multiplier switch
    {
        2 => $"D{dart.Number}",
        3 => $"T{dart.Number}",
        _ => dart.Number.ToString()
    };
}
