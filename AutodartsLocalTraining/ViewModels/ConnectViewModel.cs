using System.Net.Http;
using System.Windows.Media;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;

namespace AutodartsLocalTraining.ViewModels;

public class ConnectViewModel : ViewModelBase
{
    private string _ip = "";
    private string _port = "3180";
    private string _statusMessage = "";
    private Brush _statusBrush = Brushes.Gray;
    private bool _isConnecting;

    public string Ip { get => _ip; set => SetField(ref _ip, value); }
    public string Port { get => _port; set => SetField(ref _port, value); }
    public string StatusMessage { get => _statusMessage; private set => SetField(ref _statusMessage, value); }
    public Brush StatusBrush { get => _statusBrush; private set => SetField(ref _statusBrush, value); }

    private bool IsConnecting
    {
        get => _isConnecting;
        set
        {
            if (SetField(ref _isConnecting, value)) ConnectCommand.RaiseCanExecuteChanged();
        }
    }

    public RelayCommand ConnectCommand { get; }

    public event EventHandler<AutodartsClient>? Connected;

    public ConnectViewModel()
    {
        ConnectCommand = new RelayCommand(async () => await ConnectAsync(isAutoConnect: false), () => !IsConnecting);

        var settings = SettingsService.Load();
        Ip = settings.Ip;
        Port = settings.Port;
    }

    public async Task AutoConnectIfConfiguredAsync()
    {
        if (!string.IsNullOrWhiteSpace(Ip))
        {
            await ConnectAsync(isAutoConnect: true);
        }
    }

    private async Task ConnectAsync(bool isAutoConnect)
    {
        var ip = Ip.Trim();
        var port = string.IsNullOrWhiteSpace(Port) ? "3180" : Port.Trim();

        if (string.IsNullOrEmpty(ip))
        {
            SetStatus("Board IP is required.", isError: true);
            return;
        }

        IsConnecting = true;
        SetStatus(isAutoConnect ? $"Reconnecting to {ip}:{port}..." : "Connecting...", isError: false);

        var client = new AutodartsClient(ip, port);
        try
        {
            var state = await client.GetStateAsync();
            if (state is null) throw new Exception("Empty response");

            SettingsService.Save(new ConnectionSettings { Ip = ip, Port = port });
            Connected?.Invoke(this, client);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            client.Dispose();
            SetStatus(isAutoConnect
                ? "Could not reach saved board. Check IP/Port and connect manually."
                : "Could not reach board. Check IP/Port and try again.", isError: true);
            IsConnecting = false;
        }
        catch (Exception)
        {
            client.Dispose();
            SetStatus("Unexpected response from board.", isError: true);
            IsConnecting = false;
        }
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        StatusBrush = isError ? Brushes.Firebrick : Brushes.Gray;
    }
}
