namespace AutodartsLocalTraining.Modes;

public sealed class ThreeDartCheckout : ITrainingMode
{
    
    private readonly List<DartThrowResult> _turnThrows = new();
    private static readonly int[] Targets = { 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37, 39, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61 };
    private int _leftover = Targets[0];
    private bool _turnJustCompleted;
    private int _turnCount = 1;

    public string PrimaryDisplayValue => _leftover.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => Targets.Length * 2;

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
            // Three Dart Checkout: ThrowOutcome.Bad here always means the leg was
            // busted (went negative, hit exactly 1, or reached 0 without a double) -
            // show "Busted" instead of the actual segment, which would be confusing
            // (e.g. showing "S1" for a throw that busted a leftover of 1).
            _turnThrows.Add(new DartThrowResult("Busted", ThrowOutcome.Bad));
            _turnJustCompleted = true;
            return true;
        }

        if (isCheckout)
        {
            Score += _turnThrows.Count == 1 ? 2 : 1;
            _leftover = 0;
            _turnThrows.Add(new DartThrowResult(dart.FormatName(), ThrowOutcome.Good));
            _turnJustCompleted = true;
            return true;
        }

        _leftover = remaining;
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

        if (_turnCount == (Targets.Length + 1))
        {
            IsComplete = true;
        }
        else
        {
            _leftover = Targets[_turnCount - 1];
        }
    }
}
