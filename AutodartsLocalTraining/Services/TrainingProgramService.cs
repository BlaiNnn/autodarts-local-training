using System.IO;
using System.Text.Json;
using AutodartsLocalTraining.Models;

namespace AutodartsLocalTraining.Services;

public static class TrainingProgramService
{
    private static readonly string ProgramsDirectory =
        Path.Combine(AppContext.BaseDirectory, "TrainingPrograms");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static List<TrainingProgram> LoadAll()
    {
        var programs = new List<TrainingProgram>();

        if (!Directory.Exists(ProgramsDirectory))
            return programs;

        foreach (var file in Directory.EnumerateFiles(ProgramsDirectory, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(file);
                var program = JsonSerializer.Deserialize<TrainingProgram>(json, JsonOptions);
                if (program is not null && program.Sequence.Count > 0)
                    programs.Add(program);
            }
            catch (JsonException)
            {
                // Skip malformed training program files rather than failing startup.
            }
        }

        return programs;
    }
}
