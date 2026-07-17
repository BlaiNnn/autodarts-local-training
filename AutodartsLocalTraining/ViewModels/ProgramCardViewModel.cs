namespace AutodartsLocalTraining.ViewModels;

public sealed class ProgramCardViewModel
{
    public string Name { get; }
    public string RecordText { get; }
    public string RecordDateText { get; }
    public RelayCommand SelectCommand { get; }

    public ProgramCardViewModel(string name, string recordText, string recordDateText, Action select)
    {
        Name = name;
        RecordText = recordText;
        RecordDateText = recordDateText;
        SelectCommand = new RelayCommand(select);
    }
}
