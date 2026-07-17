using System.Globalization;
using System.Resources;

namespace AutodartsLocalTraining.Properties;

/// <summary>
/// Hand-written accessor over Resources.resx (dotnet build doesn't run the VS-only
/// ResXFileCodeGenerator single-file-generator, so there's no auto-generated Designer.cs).
/// Adding another culture later only requires a matching Resources.&lt;culture&gt;.resx file
/// next to this one - ResourceManager picks it automatically via satellite assembly fallback.
/// </summary>
internal static class Resources
{
    private static readonly ResourceManager ResourceManager =
        new("AutodartsLocalTraining.Properties.Resources", typeof(Resources).Assembly);

    public static string Connect_Title => Get();
    public static string Connect_BoardIpLabel => Get();
    public static string Connect_PortLabel => Get();
    public static string Connect_ConnectButton => Get();
    public static string Connect_BoardIpRequired => Get();
    public static string Connect_Connecting => Get();
    public static string Connect_Reconnecting => Get();
    public static string Connect_CouldNotReachSaved => Get();
    public static string Connect_CouldNotReachManual => Get();
    public static string Connect_UnexpectedResponse => Get();

    public static string Common_Disconnect => Get();
    public static string Common_Quit => Get();

    public static string Selection_Title => Get();
    public static string Selection_NoRecordYet => Get();
    public static string Selection_BestScoreFormat => Get();
    public static string Selection_RecordDateFormat => Get();

    public static string Run_AbandonButton => Get();
    public static string Run_Offline => Get();
    public static string Run_ConnectionLost => Get();
    public static string Run_TrainingComplete => Get();

    public static string Result_ContinueButton => Get();
    public static string Result_NoHistoryYet => Get();
    public static string Result_TrendUp => Get();
    public static string Result_TrendDown => Get();
    public static string Result_TrendEqual => Get();

    private static string Get([System.Runtime.CompilerServices.CallerMemberName] string name = "")
        => ResourceManager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
}
