using System.Windows.Controls;
using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.Views;

public partial class TrainingSelectionView : UserControl, IEscapable
{
    private readonly TrainingSelectionViewModel _viewModel;

    public event EventHandler<TrainingMode>? ProgramSelected;
    public event EventHandler? Disconnected;
    public event EventHandler? QuitRequested;

    public TrainingSelectionView(AutodartsClient client)
    {
        InitializeComponent();
        DisconnectButton.Content = Properties.Resources.Common_Disconnect;
        QuitButton.Content = Properties.Resources.Common_Quit;
        TitleText.Text = Properties.Resources.Selection_Title;

        _viewModel = new TrainingSelectionViewModel(client);
        _viewModel.ProgramSelected += (_, program) => ProgramSelected?.Invoke(this, program);
        _viewModel.Disconnected += (_, _) => Disconnected?.Invoke(this, EventArgs.Empty);
        _viewModel.QuitRequested += (_, _) => QuitRequested?.Invoke(this, EventArgs.Empty);
        DataContext = _viewModel;
    }

    public void HandleEscape() => _viewModel.DisconnectCommand.Execute(null);
}
