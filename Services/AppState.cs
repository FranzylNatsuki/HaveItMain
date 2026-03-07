using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using HaveItMain.ViewModels;
using ReactiveUI;

namespace HaveItMain.Services;

public class AppState : ReactiveObject
{
    private bool _streakStarted = false;
    
    public ObservableCollection<Account> AllAccounts { get; set; } = new();
    
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
    
    public AppState()
    {
        LoadAccounts();
    }
    
    public void LoadAccounts()
    {
        if (System.IO.File.Exists("accounts.json"))
        {
            try
            {
                string json = System.IO.File.ReadAllText("accounts.json");
                var loadedAccounts = System.Text.Json.JsonSerializer.Deserialize<List<Account>>(json);
                if (loadedAccounts != null)
                {
                    AllAccounts = new ObservableCollection<Account>(loadedAccounts);
                }
            }
            catch { /* Handle corrupted JSON if needed */ }
        }
    }
    
    public async Task ExportTasks(string destinationPath)
    {
        try
        {
            // Just copy the existing file to the user's chosen location
            if (File.Exists("tasks.json"))
            {
                File.Copy("tasks.json", destinationPath, true);
                NotificationService?.ShowNotification("Export Success", "Backup created!");
            }
        }
        catch (Exception ex) { /* handle error */ }
    }

    public async Task ImportTasks(string sourcePath)
    {
        try
        {
            string destinationPath = "tasks.json";
    
            // 1. Copy the imported file over the current one
            File.Copy(sourcePath, destinationPath, true);

            // 2. Use YOUR existing Load method
            // It already clears this.Tasks and adds the new ones!
            var persistence = new PersistenceService(); 
        
            // Pass 'this' (the AppState) into the method
            persistence.Load(this); 

            NotificationService?.ShowNotification("Import Success", "Tasks updated!");
        }
        catch (Exception ex) 
        { 
            System.Diagnostics.Debug.WriteLine($"Import failed: {ex.Message}");
        }
    }
}