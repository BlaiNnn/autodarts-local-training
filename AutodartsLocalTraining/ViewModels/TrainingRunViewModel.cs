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

    private readonly AutodartsClient _client;
    private readonly DispatcherTimer _timer;
    private readonly ITrainingMode _session;
    private readonly TrainingMode _trainingMode;
    private bool _isPolling;
    private int _processedThrowCount;
    private bool _awaitingRealTakeout;
    private bool _sawTakeoutStatus;

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

    public TrainingRunViewModel(AutodartsClient client, TrainingMode trainingMode)
    {
        _client = client;
        TrainingName = Properties.Resources.TrainingModeCaption(trainingMode.ToString());
        _trainingMode = trainingMode;
        _session = TrainingModeFactory.Create(_trainingMode);
        AbandonCommand = new RelayCommand(Abandon);

        RenderSession();

        _timer = new DispatcherTimer { Interval = OnlineInterval };
        _timer.Tick += Timer_Tick;

        _ = StartAsync();
    }

    private async Task StartAsync()
    {
        // Reset before the first poll so a leg from a prior session isn't replayed against this one's target.
        await ResetBoardAsync();
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

            if (_awaitingRealTakeout)
            {
                // noticed that Autodarts has now really shifted to the takeout routine
                if (state.Status.Contains("Takeout", StringComparison.OrdinalIgnoreCase))
                    _sawTakeoutStatus = true;

                // Only when Autodarts itself reports “Throw” again AND the throw list
                // is truly empty is the removal physically confirmed -> only then proceed
                if (_sawTakeoutStatus
                    && string.Equals(state.Status, "Throw", StringComparison.OrdinalIgnoreCase)
                    && state.Throws.Count == 0)
                {
                    _awaitingRealTakeout = false;
                    _sawTakeoutStatus = false;
                    _processedThrowCount = 0;

                    _session.AdvanceToNextTurn();
                    RenderSession();

                    if (_session.IsComplete)
                        CompleteTraining();
                }
            }
            else
            {
                var throws = MapThrows(state.Throws);
                for (var i = _processedThrowCount; i < throws.Count; i++)
                {
                    _processedThrowCount = i + 1;
                    if (_session.ProcessThrow(throws[i]))
                    {
                        _awaitingRealTakeout = true;
                        break;
                    }
                }

                RenderSession();
            }
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
    
    private async Task ResetBoardAsync()
    {
        try
        {
            await _client.ResetBoardAsync();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
        }
    }

    private void CompleteTraining()
    {
        var priorHistory = ScoreHistoryService.LoadHistory(_trainingMode.ToString());
        double? priorAverage = priorHistory.Count > 0 ? priorHistory.Average(entry => entry.Score) : null;
        ScoreHistoryService.AppendResult(_trainingMode.ToString(), _session.Score);

        Completed?.Invoke(this,
            new TrainingCompletedEventArgs(TrainingName, _session.Score, _session.MaxScore, priorAverage));
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
    }
}

public sealed record TrainingCompletedEventArgs(string TrainingName, int Score, int MaxScore, double? PriorAverage);