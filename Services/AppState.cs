using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
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
    
    private STREAK? _currentStreak;
    public STREAK? CurrentStreak
    {
        get => _currentStreak;
        set => this.RaiseAndSetIfChanged(ref _currentStreak, value);
    }
    
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

    private bool _enableSfx = false;
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

    private int _sfxVolume = 60;
    public int SfxVolume
    {
        get => _sfxVolume;
        set => this.RaiseAndSetIfChanged(ref _sfxVolume, value);
    }
    
    public AppState()
    {
        LoadAccounts();
    }
    
    private bool _isDyslexicEnabled;
    public bool IsDyslexicEnabled
    {
        get => _isDyslexicEnabled;
        set 
        {
            if (value != _isDyslexicEnabled)
            {
                _isDyslexicEnabled = value;
                UpdateAppFont(value);
                this.RaisePropertyChanged(nameof(IsDyslexicEnabled));
            }
        }
    }
    
    
    private void UpdateAppFont(bool enabled)
    {
        if (Application.Current == null) return;
        var res = Application.Current.Resources;
    
        res["AppFont"] = enabled ? res["DyslexicFont"] : res["NunitoFont"];
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
                    
                    var sessionService = new SessionService();
                    var session = sessionService.LoadSession();

                    if (session.IsSignedIn && !string.IsNullOrEmpty(session.LastUsername))
                    {
                        var savedUser = AllAccounts.FirstOrDefault(a => a.Username == session.LastUsername);
                        if (savedUser != null)
                        {
                            UserAccount = savedUser;
                            IsLoggedIn = true;
                        }
                        else
                        {
                            UserAccount = AllAccounts.FirstOrDefault() ?? new Account();
                        }
                    }
                    else
                    {
                        UserAccount = AllAccounts.FirstOrDefault() ?? new Account();
                    }
                }
            }
            catch
            {
                
            }
        }
    }
    
    
    public async Task ExportTasks(string destinationPath)
    {
        try
        {
            if (File.Exists("tasks.json"))
            {
                File.Copy("tasks.json", destinationPath, true);
                NotificationService?.ShowNotification("Export Success", "Backup created!");
            }
        }
        catch (Exception ex)
        {
            
        }
    }

    public async Task ImportTasks(string sourcePath)
    {
        try
        {
            string destinationPath = "tasks.json";
            File.Copy(sourcePath, destinationPath, true);
            var persistence = new PersistenceService(); 
            persistence.Load(this); 

            NotificationService?.ShowNotification("Import Success", "Tasks updated!");
        }
        catch (Exception ex) 
        { 
            System.Diagnostics.Debug.WriteLine($"Import failed: {ex.Message}");
        }
    }
}