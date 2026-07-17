using System.Windows;
using System.Windows.Controls;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.Views;

public partial class ConnectView : UserControl, IEscapable
{
    private readonly ConnectViewModel _viewModel = new();

    public event EventHandler<AutodartsClient>? Connected;

    public ConnectView()
    {
        InitializeComponent();
        TitleText.Text = Properties.Resources.Connect_Title;
        BoardIpLabel.Text = Properties.Resources.Connect_BoardIpLabel;
        PortLabel.Text = Properties.Resources.Connect_PortLabel;
        ConnectButton.Content = Properties.Resources.Connect_ConnectButton;

        DataContext = _viewModel;
        _viewModel.Connected += (_, client) => Connected?.Invoke(this, client);
        Loaded += async (_, _) => await _viewModel.AutoConnectIfConfiguredAsync();
    }

    public void HandleEscape() => Application.Current.Shutdown();
}
