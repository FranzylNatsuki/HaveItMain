using System.Collections.ObjectModel;
using HaveItMain.ViewModels;

namespace HaveItMain.Services;

public class AppState
{
    public ObservableCollection<TimerViewModel> Timers { get; } = new();
    
    
}