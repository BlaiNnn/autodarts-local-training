using System.Net.Http;
using System.Windows.Media;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class ConnectViewModel : ViewModelBase
{
    public string Ip { get; set => SetField(ref field, value); } = "";

    public string Port { get; set => SetField(ref field, value); } = "3180";

    public string StatusMessage { get; private set => SetField(ref field, value); } = "";

    public Brush StatusBrush { get; private set => SetField(ref field, value); } = Brushes.Gray;

    private bool IsConnecting { get; set { if (SetField(ref field, value)) ConnectCommand.RaiseCanExecuteChanged(); } }

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
            SetStatus(Properties.Resources.Connect_BoardIpRequired, isError: true);
            return;
        }

        IsConnecting = true;
        SetStatus(isAutoConnect
            ? string.Format(Properties.Resources.Connect_Reconnecting, ip, port)
            : Properties.Resources.Connect_Connecting, isError: false);

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
                ? Properties.Resources.Connect_CouldNotReachSaved
                : Properties.Resources.Connect_CouldNotReachManual, isError: true);
            IsConnecting = false;
        }
        catch (Exception)
        {
            client.Dispose();
            SetStatus(Properties.Resources.Connect_UnexpectedResponse, isError: true);
            IsConnecting = false;
        }
    }

    private void SetStatus(string message, bool isError)
    {
        StatusMessage = message;
        StatusBrush = isError ? Brushes.Firebrick : Brushes.Gray;
    }
}
