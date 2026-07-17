using System.Windows.Controls;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.Views;

public partial class TrainingResultView : UserControl, IEscapable
{
    private readonly TrainingResultViewModel _viewModel;

    public event EventHandler? ContinueRequested;

    public TrainingResultView(AutodartsClient client, string trainingName, int score, int maxScore, double? priorAverage)
    {
        InitializeComponent();
        ContinueButton.Content = Properties.Resources.Result_ContinueButton;

        _viewModel = new TrainingResultViewModel(client, trainingName, score, maxScore, priorAverage);
        _viewModel.ContinueRequested += (_, _) => ContinueRequested?.Invoke(this, EventArgs.Empty);
        DataContext = _viewModel;
    }

    public void HandleEscape() => _viewModel.ContinueCommand.Execute(null);
}
