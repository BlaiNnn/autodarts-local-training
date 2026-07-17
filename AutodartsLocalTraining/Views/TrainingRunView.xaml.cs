using System.Windows.Controls;
using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.WinUI.ViewModels;

namespace AutodartsLocalTraining.WinUI.Views;

public partial class TrainingRunView : UserControl, IEscapable, IDisposable
{
    private readonly TrainingRunViewModel _viewModel;

    public event EventHandler? Abandoned;
    public event EventHandler<TrainingCompletedEventArgs>? Completed;

    public TrainingRunView(AutodartsClient client, TrainingMode program)
    {
        InitializeComponent();
        AbandonButton.Content = Properties.Resources.Run_AbandonButton;

        _viewModel = new TrainingRunViewModel(client, program);
        _viewModel.Abandoned += (_, _) => Abandoned?.Invoke(this, EventArgs.Empty);
        _viewModel.Completed += (_, e) => Completed?.Invoke(this, e);
        DataContext = _viewModel;
    }

    public void HandleEscape() => _viewModel.AbandonCommand.Execute(null);

    public void Dispose() => _viewModel.Dispose();
}
