using System.IO;
using System.Text.Json;
using AutodartsLocalTraining.Models;

namespace AutodartsLocalTraining.Services;

public static class ScoreHistoryService
{
    private static readonly string BaseDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AutodartsLocalTraining");

    private static string FilePathFor(string modeKey) => Path.Combine(BaseDirectory, $"scores-{modeKey}.json");

    public static List<ScoreHistoryEntry> LoadHistory(string modeKey)
    {
        var path = FilePathFor(modeKey);

        try
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                var entries = JsonSerializer.Deserialize<List<ScoreHistoryEntry>>(json);
                if (entries is not null) return entries;
            }
        }
        catch
        {
            // Ignore corrupt or unreadable history and fall back to an empty list.
        }

        return new List<ScoreHistoryEntry>();
    }

    public static void AppendResult(string modeKey, int score)
    {
        var history = LoadHistory(modeKey);
        history.Add(new ScoreHistoryEntry { Date = DateTime.UtcNow, Score = score });

        Directory.CreateDirectory(BaseDirectory);
        File.WriteAllText(FilePathFor(modeKey), JsonSerializer.Serialize(history));
    }
}
