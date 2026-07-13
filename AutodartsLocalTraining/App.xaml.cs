using System.Windows;

namespace AutodartsLocalTraining;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var connectWindow = new ConnectWindow();
        MainWindow = connectWindow;
        connectWindow.Show();
    }
}
