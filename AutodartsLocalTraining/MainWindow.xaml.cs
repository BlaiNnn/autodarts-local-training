using System.Windows;
using System.Windows.Input;
using AutodartsLocalTraining.Modes;
using AutodartsLocalTraining.Services;
using AutodartsLocalTraining.WinUI.ViewModels;
using AutodartsLocalTraining.WinUI.Views;

namespace AutodartsLocalTraining.WinUI;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape && MainContentHost.Content is IEscapable escapable)
                escapable.HandleEscape();
        };

        ShowConnect();
    }

    private void SetContent(object view)
    {
        if (MainContentHost.Content is IDisposable disposable) disposable.Dispose();
        MainContentHost.Content = view;
    }

    private void ShowConnect()
    {
        var view = new ConnectView();
        view.Connected += (_, client) => ShowSelection(client);
        SetContent(view);
    }

    private void ShowSelection(AutodartsClient client)
    {
        var view = new TrainingSelectionView(client);
        view.ProgramSelected += (_, program) => ShowRun(client, program);
        view.Disconnected += (_, _) => ShowConnect();
        view.QuitRequested += (_, _) => Application.Current.Shutdown();
        SetContent(view);
    }

    private void ShowRun(AutodartsClient client, TrainingMode program)
    {
        var view = new TrainingRunView(client, program);
        view.Abandoned += (_, _) => ShowSelection(client);
        view.Completed += (_, e) => ShowResult(client, e);
        SetContent(view);
    }

    private void ShowResult(AutodartsClient client, TrainingCompletedEventArgs e)
    {
        var view = new TrainingResultView(client, e.TrainingName, e.Score, e.MaxScore, e.PriorAverage);
        view.ContinueRequested += (_, _) => ShowSelection(client);
        SetContent(view);
    }
}
