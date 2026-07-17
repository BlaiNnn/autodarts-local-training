using System.Windows.Media;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public class ThrowDisplay(string text, Brush foreground)
{
    public string Text { get; } = text;
    public Brush Foreground { get; } = foreground;
}
