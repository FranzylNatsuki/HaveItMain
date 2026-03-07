using System;
using System.Collections.ObjectModel;
using HaveItMain.ViewModels;
using ReactiveUI;

namespace HaveItMain.Services;

public class AppState : ReactiveObject
{
    private bool _streakStarted = false;
    public Account account_session;
    
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
    public Account UserAccount { get; set; } = new();
    public bool IsLoggedIn { get; set; }
    
    private bool _termsAndConditions = true;
    public bool TermsAndConditions
    {
        get => _termsAndConditions;
        set => this.RaiseAndSetIfChanged(ref _termsAndConditions, value);
    }

    private bool _notificationsGlobal = true;
    public bool NotificationsGlobal
    {
        get => _notificationsGlobal;
        set => this.RaiseAndSetIfChanged(ref _notificationsGlobal, value);
    }

    private bool _enableSfx = true;
    public bool EnableSfx
    {
        get => _enableSfx;
        set => this.RaiseAndSetIfChanged(ref _enableSfx, value);
    }

    private bool _dyselxic = true;
    public bool Dyslexic
    {
        get => _dyselxic;
        set => this.RaiseAndSetIfChanged(ref _dyselxic, value);
    }

    private int _volume = 50;  
    public int Volume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }

    

    // Constructor (Empty for now is fine!)
    public AppState()
    {
    }
}