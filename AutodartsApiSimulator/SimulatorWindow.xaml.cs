using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AutodartsLocalTraining.BoardSimulator.Models;

namespace AutodartsLocalTraining.BoardSimulator;

public partial class SimulatorWindow : Window
{
    private readonly BoardSimulator _simulator;
    private readonly DispatcherTimer _timer;
    private int _multiplier = 1;

    public SimulatorWindow(BoardSimulator simulator)
    {
        InitializeComponent();
        _simulator = simulator;

        RenderState(_simulator.GetState());

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _timer.Tick += (_, _) => RenderState(_simulator.GetState());
        _timer.Start();

        Closed += (_, _) => _timer.Stop();
    }

    private void MultiplierChanged(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton { Tag: string tag } && int.TryParse(tag, out var multiplier))
        {
            _multiplier = multiplier;
        }
    }

    private void Number_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string tag } && int.TryParse(tag, out var number))
        {
            Throw(number, _multiplier);
        }
    }

    private void Bull_Click(object sender, RoutedEventArgs e) => Throw(25, 2);
    private void OuterBull_Click(object sender, RoutedEventArgs e) => Throw(25, 1);
    private void Miss_Click(object sender, RoutedEventArgs e) => Throw(0, 0);
    private void Random_Click(object sender, RoutedEventArgs e) => RenderState(_simulator.AddRandomThrow());
    private void Reset_Click(object sender, RoutedEventArgs e) => RenderState(_simulator.Reset());

    private void Throw(int number, int multiplier) => RenderState(_simulator.AddThrow(number, multiplier));

    private void RenderState(SimState state)
    {
        StatusText.Text = $"Status: {state.Status}";
        ThrowsText.Text = state.Throws.Count == 0
            ? "Throws: -"
            : "Throws: " + string.Join("   ", state.Throws.Select(t => t.Segment.Name));
    }
}
