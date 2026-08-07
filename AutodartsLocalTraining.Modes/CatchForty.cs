namespace AutodartsLocalTraining.Modes;

public sealed class CatchForty : ITrainingMode
{
    private const int FirstNumber = 61;
    private const int LastNumber = 100;
    private const int TargetCount = LastNumber - FirstNumber + 1;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _leftover = FirstNumber;
    private bool _turnJustCompleted;
    private bool _turnSucceeded;
    private int _targetIndex;
    private int _attemptNumber = 1;

    public string PrimaryDisplayValue => _leftover.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => TargetCount * 3;

    public bool IsComplete { get; private set; }

    public bool ProcessThrow(DartThrow dart)
    {
        if (IsComplete || _turnJustCompleted) return false;

        var remaining = _leftover - dart.Value;
        var isDouble = dart.Multiplier == 2;

        var isBust = remaining < 0 || remaining == 1 || (remaining == 0 && !isDouble);
        var isCheckout = remaining == 0 && isDouble;

        if (isBust)
        {
            _turnThrows.Add(new DartThrowResult("Busted", ThrowOutcome.Bad));
            _turnSucceeded = false;
            _turnJustCompleted = true;
            return true;
        }

        if (isCheckout)
        {
            if (_attemptNumber == 2)
                Score += 1;
            else if (_turnThrows.Count >= 2)
                Score += 2;
            else
                Score += 3;

            _leftover = 0;
            _turnThrows.Add(new DartThrowResult(dart.FormatName(), ThrowOutcome.Good));
            _turnSucceeded = true;
            _turnJustCompleted = true;
            return true;
        }

        _leftover = remaining;
        _turnThrows.Add(new DartThrowResult(dart.FormatName(), ThrowOutcome.Neutral));

        if (_turnThrows.Count >= 3)
        {
            _turnSucceeded = false;
            _turnJustCompleted = true;
        }

        return _turnJustCompleted;
    }

    public void AdvanceToNextTurn()
    {
        if (!_turnJustCompleted) return;

        _turnThrows.Clear();
        _turnJustCompleted = false;

        if (_turnSucceeded || _attemptNumber == 2)
        {
            _targetIndex++;
            _attemptNumber = 1;

            if (_targetIndex >= TargetCount)
                IsComplete = true;
            else
                _leftover = FirstNumber + _targetIndex;
        }
        else
        {
            _attemptNumber = 2;
            _leftover = FirstNumber + _targetIndex;
        }
    }
}