using System.Windows;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining;

public partial class TrainingRunWindow : Window
{
    private readonly TrainingRunViewModel _viewModel;

    public TrainingRunWindow(AutodartsClient client, TrainingProgram program)
    {
        InitializeComponent();
        _viewModel = new TrainingRunViewModel(client, program);
        _viewModel.Disconnected += ViewModel_Disconnected;
        DataContext = _viewModel;

        Closed += (_, _) => _viewModel.Dispose();
    }

    private void ViewModel_Disconnected(object? sender, EventArgs e)
    {
        var connectWindow = new ConnectWindow();
        Application.Current.MainWindow = connectWindow;
        connectWindow.Show();
        Close();
    }
}
