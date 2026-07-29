namespace AutodartsLocalTraining.Modes;

public sealed class ThreeDartCheckoutEighteen : ITrainingMode
{
    
    private readonly List<DartThrowResult> _turnThrows = new();
    private static readonly int[] Targets = { 36, 58, 62, 66, 68, 70, 74, 78, 86, 90, 94, 102, 104, 108, 118, 122, 134, 144, 154, 164 };
    private int _leftover = Targets[0];
    private int _turnCount = 1;
    private bool _turnJustCompleted;

    public string PrimaryDisplayValue => _leftover.ToString();

    public IReadOnlyList<DartThrowResult> CurrentTurnThrows => _turnThrows;

    public int Score { get; private set; }

    public int MaxScore => Targets.Length;

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
            Score++;
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
