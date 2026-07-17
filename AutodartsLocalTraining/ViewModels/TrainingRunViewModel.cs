using System.Net.Http;
using System.Windows.Media;
using System.Windows.Threading;
using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class TrainingRunViewModel : ViewModelBase, IDisposable
{
    private static readonly TimeSpan OnlineInterval = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan OfflineInterval = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan HoldInterval = TimeSpan.FromSeconds(1);

    private readonly AutodartsClient _client;
    private readonly DispatcherTimer _timer;
    private readonly DispatcherTimer _holdTimer;
    private readonly ITrainingMode _session;
    private readonly TrainingMode _mode;
    private bool _isPolling;

    // The board only clears its reported throws once a new dart lands after a completed turn - it has
    // no concept of our targets or training sessions. Until that clear is observed, throws already on
    // the board belong to a prior turn/session and must not be replayed against the new target.
    private bool _needsLegBaseline = true;
    private int _staleLegThrowBaseline;
    private int _lastRawThrowCount;

    public string TrainingName { get; }

    public string BoardStatusText
    {
        get;
        private set => SetField(ref field, value);
    } = Properties.Resources.Run_Offline;

    public Brush BoardStatusBrush
    {
        get;
        private set => SetField(ref field, value);
    } = Brushes.Yellow;

    public string TargetNumber
    {
        get;
        private set => SetField(ref field, value);
    } = "1";

    public string Score
    {
        get;
        private set => SetField(ref field, value);
    } = "0";

    public IReadOnlyList<ThrowDisplay> Throws
    {
        get;
        private set => SetField(ref field, value);
    } = BuildEmptyThrows();

    public string StatusText
    {
        get;
        private set => SetField(ref field, value);
    } = " ";

    public RelayCommand AbandonCommand { get; }

    public event EventHandler? Abandoned;
    public event EventHandler<TrainingCompletedEventArgs>? Completed;

    public TrainingRunViewModel(AutodartsClient client, TrainingMode program)
    {
        _client = client;
        TrainingName = nameof(program);
        _mode = program;
        _session = TrainingModeFactory.Create(_mode);
        AbandonCommand = new RelayCommand(Abandon);

        RenderSession();

        _timer = new DispatcherTimer { Interval = OnlineInterval };
        _timer.Tick += Timer_Tick;
        _timer.Start();

        _holdTimer = new DispatcherTimer { Interval = HoldInterval };
        _holdTimer.Tick += HoldTimer_Tick;
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

            var rawThrowCount = state.Throws.Count;
            _lastRawThrowCount = rawThrowCount;

            if (_needsLegBaseline)
            {
                _staleLegThrowBaseline = rawThrowCount;
                _needsLegBaseline = false;
            }
            else if (_staleLegThrowBaseline == 0 || rawThrowCount < _staleLegThrowBaseline)
            {
                _staleLegThrowBaseline = 0;
                var turnCompleted = _session.ProcessThrows(MapThrows(state.Throws));
                if (turnCompleted) StartHold();
            }

            RenderSession();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            UpdateBoardStatus(Properties.Resources.Run_Offline);
            _timer.Interval = OfflineInterval;
            StatusText = Properties.Resources.Run_ConnectionLost;
        }
        finally
        {
            _isPolling = false;
        }
    }

    private void StartHold()
    {
        if (_holdTimer.IsEnabled) return;

        _holdTimer.Start();
    }

    private void HoldTimer_Tick(object? sender, EventArgs e)
    {
        _holdTimer.Stop();
        _staleLegThrowBaseline = _lastRawThrowCount;
        _session.AdvanceToNextTurn();
        RenderSession();

        if (_session.IsComplete)
            CompleteTraining();
    }

    private void CompleteTraining()
    {
        var priorHistory = ScoreHistoryService.LoadHistory(_mode.ToString());
        double? priorAverage = priorHistory.Count > 0 ? priorHistory.Average(entry => entry.Score) : null;
        ScoreHistoryService.AppendResult(_mode.ToString(), _session.Score);

        Completed?.Invoke(this, new TrainingCompletedEventArgs(TrainingName, _session.Score, _session.MaxScore, priorAverage));
    }

    private static IReadOnlyList<DartThrow> MapThrows(IReadOnlyList<ThrowInfo> throws)
        => throws.Select(t => new DartThrow(t.Segment.Number, t.Segment.Multiplier)).ToList();

    private void RenderSession()
    {
        TargetNumber = _session.PrimaryDisplayValue;
        Score = _session.Score.ToString();
        Throws = BuildThrows();

        StatusText = _session.IsComplete
            ? string.Format(Properties.Resources.Run_TrainingComplete, _session.Score, _session.MaxScore)
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

            var throwResult = turnThrows[i];
            var brush = throwResult.Outcome switch
            {
                ThrowOutcome.Good => Brushes.LimeGreen,
                ThrowOutcome.Bad => Brushes.Red,
                _ => Brushes.White
            };
            result.Add(new ThrowDisplay(throwResult.DisplayText, brush));
        }

        return result;
    }

    private static IReadOnlyList<ThrowDisplay> BuildEmptyThrows()
        => [new("-", Brushes.Gray), new("-", Brushes.Gray), new("-", Brushes.Gray)];

    private void UpdateBoardStatus(string status)
    {
        BoardStatusText = status;
        BoardStatusBrush = string.Equals(status, "Throw", StringComparison.OrdinalIgnoreCase)
            ? Brushes.White
            : Brushes.Yellow;
    }

    private void Abandon()
    {
        Dispose();
        Abandoned?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _timer.Stop();
        _holdTimer.Stop();
    }
}

public sealed record TrainingCompletedEventArgs(string TrainingName, int Score, int MaxScore, double? PriorAverage);
