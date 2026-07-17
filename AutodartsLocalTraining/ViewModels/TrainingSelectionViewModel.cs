using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class TrainingSelectionViewModel : ViewModelBase
{
    private readonly AutodartsClient _client;

    public AutodartsClient Client => _client;
    public string ConnectionText { get; }
    public IReadOnlyList<ProgramCardViewModel> Programs { get; }

    public RelayCommand DisconnectCommand { get; }
    public RelayCommand QuitCommand { get; }

    public event EventHandler<TrainingMode>? ProgramSelected;
    public event EventHandler? Disconnected;
    public event EventHandler? QuitRequested;

    public TrainingSelectionViewModel(AutodartsClient client)
    {
        _client = client;
        ConnectionText = $"{client.Ip}:{client.Port}";
        Programs = typeof(TrainingMode).GetEnumValues().OfType<TrainingMode>().Select(BuildCard).ToList();
        DisconnectCommand = new RelayCommand(Disconnect);
        QuitCommand = new RelayCommand(Quit);
    }

    private ProgramCardViewModel BuildCard(TrainingMode program)
    {
        var mode = program;
        var history = ScoreHistoryService.LoadHistory(mode.ToString());
        var best = history.Count > 0
            ? history.OrderByDescending(entry => entry.Score).ThenByDescending(entry => entry.Date).First()
            : null;
        var maxScore = TrainingModeFactory.Create(mode).MaxScore;

        var recordText = best is null ? "No record yet" : $"Best: {best.Score} / {maxScore}";
        var recordDateText = best is null ? "" : $"on {best.Date.ToLocalTime():g}";

        return new ProgramCardViewModel(nameof(program), recordText, recordDateText, () => ProgramSelected?.Invoke(this, program));
    }

    private void Disconnect()
    {
        _client.Dispose();
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    private void Quit()
    {
        _client.Dispose();
        QuitRequested?.Invoke(this, EventArgs.Empty);
    }
}
