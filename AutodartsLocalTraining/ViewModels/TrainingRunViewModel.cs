using System.Net.Http;
using System.Windows.Media;
using System.Windows.Threading;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;

namespace AutodartsLocalTraining.ViewModels;

public class TrainingRunViewModel : ViewModelBase, IDisposable
{
    private static readonly TimeSpan OnlineInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan OfflineInterval = TimeSpan.FromSeconds(5);

    private readonly AutodartsClient _client;
    private readonly DispatcherTimer _timer;
    private readonly RoundTheBoardSingleFieldsSession _session = new();
    private bool _isPolling;

    private string _boardStatusText = "Offline";
    private Brush _boardStatusBrush = Brushes.Yellow;
    private string _targetNumber = "1";
    private string _score = "0";
    private IReadOnlyList<ThrowDisplay> _throws = BuildEmptyThrows();
    private string _statusText = " ";

    public string TrainingName { get; }
    public string BoardStatusText { get => _boardStatusText; private set => SetField(ref _boardStatusText, value); }
    public Brush BoardStatusBrush { get => _boardStatusBrush; private set => SetField(ref _boardStatusBrush, value); }
    public string TargetNumber { get => _targetNumber; private set => SetField(ref _targetNumber, value); }
    public string Score { get => _score; private set => SetField(ref _score, value); }
    public IReadOnlyList<ThrowDisplay> Throws { get => _throws; private set => SetField(ref _throws, value); }
    public string StatusText { get => _statusText; private set => SetField(ref _statusText, value); }

    public RelayCommand DisconnectCommand { get; }

    public event EventHandler? Disconnected;

    public TrainingRunViewModel(AutodartsClient client, TrainingProgram program)
    {
        _client = client;
        TrainingName = program.Name;
        DisconnectCommand = new RelayCommand(Disconnect);

        RenderSession();

        _timer = new DispatcherTimer { Interval = OnlineInterval };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private async void Timer_Tick(object? sender, EventArgs e)
    {
        if (_isPolling) return;
        _isPolling = true;

        try
        {
            var state = await _client.GetStateAsync();
            if (state is null) throw new Exception("Empty response");

            UpdateBoardStatus(state.Status);
            _timer.Interval = OnlineInterval;

            _session.ProcessThrows(state.Throws);
            RenderSession();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            UpdateBoardStatus("Offline");
            _timer.Interval = OfflineInterval;
            StatusText = "connection lost";
        }
        finally
        {
            _isPolling = false;
        }
    }

    private void RenderSession()
    {
        TargetNumber = _session.CurrentTargetNumber.ToString();
        Score = _session.Score.ToString();
        Throws = BuildThrows();

        StatusText = _session.IsComplete
            ? $"Training complete! Final score: {_session.Score} / {_session.MaxScore}"
            : " ";
    }

    private IReadOnlyList<ThrowDisplay> BuildThrows()
    {
        var turnThrows = _session.CurrentTurnThrows;
        var result = new List<ThrowDisplay>(3);

        for (var i = 0; i < 3; i++)
        {
            if (i >= turnThrows.Count)
            {
                result.Add(new ThrowDisplay("-", Brushes.Gray));
                continue;
            }

            var segment = turnThrows[i];
            var isHit = segment.Number == _session.CurrentTargetNumber && segment.Multiplier == 1;
            result.Add(new ThrowDisplay(segment.Name, isHit ? Brushes.LimeGreen : Brushes.Red));
        }

        return result;
    }

    private static IReadOnlyList<ThrowDisplay> BuildEmptyThrows()
        => [new ThrowDisplay("-", Brushes.Gray), new ThrowDisplay("-", Brushes.Gray), new ThrowDisplay("-", Brushes.Gray)];

    private void UpdateBoardStatus(string status)
    {
        BoardStatusText = status;
        BoardStatusBrush = string.Equals(status, "Throw", StringComparison.OrdinalIgnoreCase)
            ? Brushes.White
            : Brushes.Yellow;
    }

    private void Disconnect()
    {
        Dispose();
        _client.Dispose();
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose() => _timer.Stop();
}
