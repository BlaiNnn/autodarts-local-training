using System.Windows;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining;

public partial class TrainingSelectionWindow : Window
{
    private readonly TrainingSelectionViewModel _viewModel;

    public TrainingSelectionWindow(AutodartsClient client)
    {
        InitializeComponent();
        _viewModel = new TrainingSelectionViewModel(client);
        _viewModel.ProgramSelected += ViewModel_ProgramSelected;
        _viewModel.Disconnected += ViewModel_Disconnected;
        DataContext = _viewModel;
    }

    private void ViewModel_ProgramSelected(object? sender, TrainingProgram program)
    {
        var runWindow = new TrainingRunWindow(_viewModel.Client, program);
        Application.Current.MainWindow = runWindow;
        runWindow.Show();
        Close();
    }

    private void ViewModel_Disconnected(object? sender, EventArgs e)
    {
        var connectWindow = new ConnectWindow();
        Application.Current.MainWindow = connectWindow;
        connectWindow.Show();
        Close();
    }
}
