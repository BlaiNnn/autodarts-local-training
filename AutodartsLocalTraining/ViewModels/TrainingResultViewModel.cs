using System.Windows.Media;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class TrainingResultViewModel : ViewModelBase
{
    private readonly AutodartsClient _client;

    public AutodartsClient Client => _client;
    public string TrainingName { get; }
    public string ScoreText { get; }
    public string TrendText { get; }
    public Brush TrendBrush { get; }

    public RelayCommand ContinueCommand { get; }

    public event EventHandler? ContinueRequested;

    public TrainingResultViewModel(AutodartsClient client, string trainingName, int score, int maxScore, double? priorAverage)
    {
        _client = client;
        TrainingName = trainingName;
        ScoreText = $"{score} / {maxScore}";
        ContinueCommand = new RelayCommand(() => ContinueRequested?.Invoke(this, EventArgs.Empty));

        (TrendText, TrendBrush) = priorAverage switch
        {
            null => (Properties.Resources.Result_NoHistoryYet, Brushes.DodgerBlue),
            var avg when score > avg => (string.Format(Properties.Resources.Result_TrendUp, avg), Brushes.LimeGreen),
            var avg when score < avg => (string.Format(Properties.Resources.Result_TrendDown, avg), Brushes.Firebrick),
            var avg => (string.Format(Properties.Resources.Result_TrendEqual, avg), Brushes.DodgerBlue)
        };
    }
}
