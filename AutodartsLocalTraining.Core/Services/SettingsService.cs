using System.IO;
using System.Text.Json;
using AutodartsLocalTraining.Models;

namespace AutodartsLocalTraining.Services;

public static class SettingsService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "AutodartsLocalTraining",
        "settings.json");

    public static ConnectionSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var settings = JsonSerializer.Deserialize<ConnectionSettings>(json);
                if (settings is not null) return settings;
            }
        }
        catch
        {
            // Ignore corrupt or unreadable settings and fall back to defaults.
        }

        return new ConnectionSettings { Ip = "", Port = "3180" };
    }

    public static void Save(ConnectionSettings settings)
    {
        var directory = Path.GetDirectoryName(FilePath)!;
        Directory.CreateDirectory(directory);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings));
    }
}
