namespace AutodartsLocalTraining.Modes;

public sealed class ThreeDartCheckout : ITrainingMode
{
    private const int FirstTarget = 3;
    private const int LastTarget = 41;
    private const int Step = 2;
    private const int TargetCount = (LastTarget - FirstTarget) / Step + 1;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _apiThrowsSeenCount;
    private int _currentTarget = FirstTarget;
    private int _leftover = FirstTarget;
    private bool _turnJustCompleted;

    public string PrimaryDisplayValue => _leftover.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => TargetCount * 2;

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
            var remaining = _leftover - dart.Value;
            var isDouble = dart.Multiplier == 2;

            var isBust = remaining < 0 || remaining == 1 || (remaining == 0 && !isDouble);
            var isCheckout = remaining == 0 && isDouble;

            if (isBust)
            {
                _turnThrows.Add(new DartThrowResult(FormatName(dart), ThrowOutcome.Bad));
                _turnJustCompleted = true;
                _apiThrowsSeenCount = throws.Count;
                break;
            }

            if (isCheckout)
            {
                Score += _turnThrows.Count == 1 ? 2 : 1;
                _leftover = 0;
                _turnThrows.Add(new DartThrowResult(FormatName(dart), ThrowOutcome.Good));
                _turnJustCompleted = true;
                _apiThrowsSeenCount = throws.Count;
                break;
            }

            _leftover = remaining;
            _turnThrows.Add(new DartThrowResult(FormatName(dart), ThrowOutcome.Neutral));

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

        if (_currentTarget >= LastTarget)
        {
            IsComplete = true;
        }
        else
        {
            _currentTarget += Step;
            _leftover = _currentTarget;
        }
    }

    private static string FormatName(DartThrow dart) => dart.Multiplier switch
    {
        2 => $"D{dart.Number}",
        3 => $"T{dart.Number}",
        _ => dart.Number.ToString()
    };
}
