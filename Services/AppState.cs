using System;
using System.Collections.ObjectModel;
using HaveItMain.ViewModels;
using ReactiveUI;

namespace HaveItMain.Services;

public class AppState : ReactiveObject
{
    private bool _streakStarted = false;
    
    public bool StreakStarted
    {
        get => _streakStarted;
        set => this.RaiseAndSetIfChanged(ref _streakStarted, value);
    }

    // 3. Current Streak Property
    private STREAK? _currentStreak;
    public STREAK? CurrentStreak
    {
        get => _currentStreak;
        set => this.RaiseAndSetIfChanged(ref _currentStreak, value);
    }

    // Collections and Services
    public ObservableCollection<TimerViewModel> Timers { get; } = new();
    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public INotificationService? NotificationService { get; set; }
    public bool IsLoggedIn { get; set; }

    // Constructor (Empty for now is fine!)
    public AppState()
    {
    }
}