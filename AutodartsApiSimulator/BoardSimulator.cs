using AutodartsLocalTraining.BoardSimulator.Models;

namespace AutodartsLocalTraining.BoardSimulator;

public class BoardSimulator
{
    private readonly object _lock = new();
    private readonly Random _random = new();
    private readonly List<SimSegment> _throws = new();
    private string _status = "Waiting";

    public SimState GetState()
    {
        lock (_lock)
        {
            return new SimState
            {
                Status = _status,
                NumThrows = _throws.Count,
                Throws = _throws.Select(s => new SimThrowInfo { Segment = s }).ToList()
            };
        }
    }

    public SimState AddThrow(int number, int multiplier)
    {
        lock (_lock)
        {
            if (_throws.Count >= 3) _throws.Clear();
            _throws.Add(BuildSegment(number, multiplier));
            _status = "Throw";
            return GetStateUnlocked();
        }
    }

    public SimState AddRandomThrow()
    {
        var (number, multiplier) = RollRandomSegment();
        return AddThrow(number, multiplier);
    }

    public SimState Reset()
    {
        lock (_lock)
        {
            _throws.Clear();
            _status = "Waiting";
            return GetStateUnlocked();
        }
    }

    public bool IsTurnComplete()
    {
        lock (_lock)
        {
            return _throws.Count >= 3;
        }
    }

    private SimState GetStateUnlocked() => new()
    {
        Status = _status,
        NumThrows = _throws.Count,
        Throws = _throws.Select(s => new SimThrowInfo { Segment = s }).ToList()
    };

    private (int number, int multiplier) RollRandomSegment()
    {
        if (_random.Next(0, 21) == 0)
        {
            return (25, _random.Next(0, 2) == 0 ? 1 : 2);
        }

        var number = _random.Next(1, 21);
        var multiplier = _random.Next(0, 10) switch
        {
            0 => 3,
            1 or 2 => 2,
            _ => 1
        };
        return (number, multiplier);
    }

    private static SimSegment BuildSegment(int number, int multiplier)
    {
        var name = (number, multiplier) switch
        {
            (25, 2) => "Bull",
            (25, _) => "25",
            (_, 3) => $"T{number}",
            (_, 2) => $"D{number}",
            _ => $"{number}"
        };
        return new SimSegment { Number = number, Multiplier = multiplier, Name = name };
    }
}
