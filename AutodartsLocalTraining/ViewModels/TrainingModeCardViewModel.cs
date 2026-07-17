using AutodartsLocalTraining.ViewModels;

namespace AutodartsLocalTraining.WinUI.ViewModels;

public sealed class TrainingModeCardViewModel
{
    public string Name { get; }
    public string RecordText { get; }
    public string RecordDateText { get; }
    public RelayCommand SelectCommand { get; }

    public TrainingModeCardViewModel(string name, string recordText, string recordDateText, Action select)
    {
        Name = name;
        RecordText = recordText;
        RecordDateText = recordDateText;
        SelectCommand = new RelayCommand(select);
    }
}
