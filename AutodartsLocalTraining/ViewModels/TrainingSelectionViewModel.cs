using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class TrainingSelectionViewModel : ViewModelBase
{
    private readonly AutodartsClient _client;

    public AutodartsClient Client => _client;
    public string ConnectionText { get; }
    public IReadOnlyList<TrainingModeCardViewModel> TrainingModes { get; }

    public RelayCommand DisconnectCommand { get; }
    public RelayCommand QuitCommand { get; }

    public event EventHandler<TrainingMode>? TrainingModeSelected;
    public event EventHandler? Disconnected;
    public event EventHandler? QuitRequested;

    public TrainingSelectionViewModel(AutodartsClient client)
    {
        _client = client;
        ConnectionText = $"{client.Ip}:{client.Port}";
        TrainingModes = typeof(TrainingMode).GetEnumValues().OfType<TrainingMode>().Select(BuildCard).ToList();
        DisconnectCommand = new RelayCommand(Disconnect);
        QuitCommand = new RelayCommand(Quit);
    }

    private TrainingModeCardViewModel BuildCard(TrainingMode trainingMode)
    {
        var history = ScoreHistoryService.LoadHistory(trainingMode.ToString());
        var best = history.Count > 0
            ? history.OrderByDescending(entry => entry.Score).ThenByDescending(entry => entry.Date).First()
            : null;
        var maxScore = TrainingModeFactory.Create(trainingMode).MaxScore;

        var recordText = best is null ? "No record yet" : $"Best: {best.Score} / {maxScore}";
        var recordDateText = best is null ? "" : $"on {best.Date.ToLocalTime():g}";
        var caption = Properties.Resources.TrainingModeCaption(trainingMode.ToString());

        return new TrainingModeCardViewModel(caption, recordText, recordDateText, () => TrainingModeSelected?.Invoke(this, trainingMode));
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
