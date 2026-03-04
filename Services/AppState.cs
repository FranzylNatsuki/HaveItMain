using System.Collections.ObjectModel;
using HaveItMain.ViewModels;

namespace HaveItMain.Services;

public class AppState
{
    public ObservableCollection<TimerViewModel> Timers { get; } = new();
    public INotificationService? NotificationService { get; set; }
    public bool IsLoggedIn { get; set; }
}