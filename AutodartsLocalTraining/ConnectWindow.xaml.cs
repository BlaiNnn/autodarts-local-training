using System.Windows;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining;

public partial class ConnectWindow : Window
{
    private readonly ConnectViewModel _viewModel = new();

    public ConnectWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.Connected += ViewModel_Connected;
        Loaded += ConnectWindow_Loaded;
    }

    private async void ConnectWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.AutoConnectIfConfiguredAsync();
    }

    private void ViewModel_Connected(object? sender, AutodartsClient client)
    {
        var selectionWindow = new TrainingSelectionWindow(client);
        Application.Current.MainWindow = selectionWindow;
        selectionWindow.Show();
        Close();
    }
}
