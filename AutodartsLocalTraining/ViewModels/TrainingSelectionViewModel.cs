using AutodartsLocalTraining.Models;
using AutodartsLocalTraining.Services;

namespace AutodartsLocalTraining.ViewModels;

public class TrainingSelectionViewModel : ViewModelBase
{
    private readonly AutodartsClient _client;
    private TrainingProgram? _selectedProgram;

    public AutodartsClient Client => _client;
    public string ConnectionText { get; }
    public IReadOnlyList<TrainingProgram> Programs { get; }

    public TrainingProgram? SelectedProgram
    {
        get => _selectedProgram;
        set
        {
            if (!SetField(ref _selectedProgram, value)) return;
            if (value is not null) ProgramSelected?.Invoke(this, value);
        }
    }

    public RelayCommand DisconnectCommand { get; }

    public event EventHandler<TrainingProgram>? ProgramSelected;
    public event EventHandler? Disconnected;

    public TrainingSelectionViewModel(AutodartsClient client)
    {
        _client = client;
        ConnectionText = $"{client.Ip}:{client.Port}";
        Programs = TrainingProgramService.LoadAll();
        DisconnectCommand = new RelayCommand(Disconnect);
    }

    private void Disconnect()
    {
        _client.Dispose();
        Disconnected?.Invoke(this, EventArgs.Empty);
    }
}
