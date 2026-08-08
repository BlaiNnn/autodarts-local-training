namespace AutodartsLocalTraining.Modes;

public sealed class OneHundredTwentyCheckout : ITrainingMode
{
    private const int StartTarget = 120;
    private const int RoundCount = 25;
    private const int SuccessBonus = 10;
    private const int FailurePenalty = 1;

    private readonly List<DartThrowResult> _turnThrows = new();
    private int _leftover = StartTarget;
    private bool _turnJustCompleted;
    private bool _turnSucceeded;
    private int _attemptNumber = 1;
    private int _roundIndex;

    public string PrimaryDisplayValue => _leftover.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; } = StartTarget;

    public int MaxScore => StartTarget + RoundCount * SuccessBonus;

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

        if (_turnSucceeded || _attemptNumber == 3)
        {
            Score += _turnSucceeded ? SuccessBonus : -FailurePenalty;
            _roundIndex++;
            _attemptNumber = 1;

            if (_roundIndex >= RoundCount)
                IsComplete = true;
            else
                _leftover = Score;
        }
        else
        {
            _attemptNumber++;
            _leftover = Score;
        }
    }
}