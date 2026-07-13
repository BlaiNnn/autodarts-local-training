using AutodartsLocalTraining.Models;

namespace AutodartsLocalTraining.Services;

public class RoundTheBoardSingleFieldsSession
{
    private const int FirstNumber = 1;
    private const int LastNumber = 20;

    private readonly List<Segment> _turnThrows = new();
    private int _apiThrowsSeenCount;

    public int CurrentTargetNumber { get; private set; } = FirstNumber;
    public int ThrowsInCurrentTurn { get; private set; }
    public int Score { get; private set; }
    public bool IsComplete { get; private set; }

    public int MaxScore => (LastNumber - FirstNumber + 1) * 3;

    public IReadOnlyList<Segment> CurrentTurnThrows => _turnThrows;

    public void ProcessThrows(IReadOnlyList<ThrowInfo> throws)
    {
        if (IsComplete) return;

        if (throws.Count < _apiThrowsSeenCount)
        {
            _turnThrows.Clear();
            _apiThrowsSeenCount = 0;
        }

        for (var i = _apiThrowsSeenCount; i < throws.Count; i++)
        {
            var segment = throws[i].Segment;
            _turnThrows.Add(segment);

            if (segment.Number == CurrentTargetNumber && segment.Multiplier == 1)
                Score++;

            if (_turnThrows.Count >= 3)
            {
                AdvanceTarget();
                break;
            }
        }

        _apiThrowsSeenCount = throws.Count;
        ThrowsInCurrentTurn = _turnThrows.Count;
    }

    private void AdvanceTarget()
    {
        _turnThrows.Clear();

        if (CurrentTargetNumber >= LastNumber)
        {
            IsComplete = true;
        }
        else
        {
            CurrentTargetNumber++;
        }
    }
}
