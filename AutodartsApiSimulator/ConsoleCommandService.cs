using AutodartsLocalTraining.BoardSimulator.Models;

namespace AutodartsLocalTraining.BoardSimulator;

public class ConsoleCommandService(BoardSimulator simulator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PrintHelp();

        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await Console.In.ReadLineAsync(stoppingToken);
            if (line is null) break;

            HandleCommand(line.Trim());
        }
    }

    private void HandleCommand(string line)
    {
        if (line.Length == 0) return;

        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLowerInvariant();

        switch (command)
        {
            case "throw":
                if (parts.Length < 2)
                {
                    Console.WriteLine("Usage: throw <segment>  e.g. throw T20, throw D16, throw 5, throw bull, throw 25, throw miss");
                    break;
                }
                HandleThrow(parts[1]);
                break;

            case "random":
                Print(simulator.AddRandomThrow());
                break;

            case "reset":
                Print(simulator.Reset());
                break;

            case "state":
                Print(simulator.GetState());
                break;

            case "help":
            case "?":
                PrintHelp();
                break;

            default:
                Console.WriteLine($"Unknown command '{command}'. Type 'help' for a list of commands.");
                break;
        }
    }

    private void HandleThrow(string segment)
    {
        if (!TryParseSegment(segment, out var number, out var multiplier))
        {
            Console.WriteLine($"Could not parse segment '{segment}'. Examples: T20, D16, S5, 20, BULL, 25, MISS");
            return;
        }

        Print(simulator.AddThrow(number, multiplier));
    }

    private static bool TryParseSegment(string input, out int number, out int multiplier)
    {
        number = 0;
        multiplier = 1;

        var value = input.Trim().ToUpperInvariant();
        if (value.Length == 0) return false;

        switch (value)
        {
            case "BULL":
            case "DBULL":
            case "B50":
                number = 25;
                multiplier = 2;
                return true;
            case "25":
            case "OB":
            case "BULL25":
                number = 25;
                multiplier = 1;
                return true;
            case "MISS":
            case "0":
                number = 0;
                multiplier = 0;
                return true;
        }

        var prefix = value[0];
        var rest = value[1..];

        if (prefix == 'T' && int.TryParse(rest, out var t))
        {
            number = t;
            multiplier = 3;
            return number is >= 1 and <= 20;
        }

        if (prefix == 'D' && int.TryParse(rest, out var d))
        {
            number = d;
            multiplier = 2;
            return number is >= 1 and <= 20;
        }

        if (prefix == 'S' && int.TryParse(rest, out var s))
        {
            number = s;
            multiplier = 1;
            return number is >= 1 and <= 20;
        }

        if (int.TryParse(value, out var n))
        {
            number = n;
            multiplier = 1;
            return number is >= 1 and <= 20;
        }

        return false;
    }

    private static void Print(SimState state)
    {
        var throwsText = state.Throws.Count == 0
            ? "-"
            : string.Join(", ", state.Throws.Select(t => t.Segment.Name));

        Console.WriteLine($"[{state.Status}] throws: {throwsText}");
    }

    private static void PrintHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Autodarts simulator console. Commands:");
        Console.WriteLine("  throw <segment>   e.g. throw T20, throw D16, throw 5, throw bull, throw 25, throw miss");
        Console.WriteLine("  random            throw a random segment");
        Console.WriteLine("  reset             clear the current turn");
        Console.WriteLine("  state             print the current state");
        Console.WriteLine("  help              show this help");
        Console.WriteLine();
    }
}
