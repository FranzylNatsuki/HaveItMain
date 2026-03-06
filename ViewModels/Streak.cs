using System;
using System.Collections.ObjectModel;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Streak : ViewModelBase, IHasTitle
{
    public string Title => "STREAK";
    
    public AppState State => _state;
    public readonly AppState _state;
    private readonly StreakPersistenceService _persistence;
    public bool StreakStarted => _state.StreakStarted;
    
    public bool Is7DayStreak => _state.CurrentStreak?.DurationDays == 7;
    public bool Is14DayStreak => _state.CurrentStreak?.DurationDays == 14;
    public bool IsStreakOver => _state.CurrentStreak?.Status == StreakStatus.Completed || _state.CurrentStreak?.Status == StreakStatus.Broken;
    public bool IsStreakSuccess => _state.CurrentStreak?.Status == StreakStatus.Completed;
    public bool IsStreakFailed => _state.CurrentStreak?.Status == StreakStatus.Broken;

    public Streak(AppState state)
    {
        _state = state;
        _persistence = new StreakPersistenceService();

// This tells the UI: "Something inside the Streak changed, re-check all fire bindings!"
        _state.WhenAnyValue(x => x.CurrentStreak)
            .Subscribe(_ => {
                this.RaisePropertyChanged(nameof(state));
                this.RaisePropertyChanged(nameof(Is7DayStreak));
                this.RaisePropertyChanged(nameof(Is14DayStreak));
            });
        Console.WriteLine($"Streak Started: {_state.StreakStarted}");
    }
    
    
    public void StartNewStreak()
    {
        var newStreak = new Services.STREAK 
        { 
            StartDate = DateTime.Today, 
            DurationDays = 7 
        };
        newStreak.GenerateDays();
        
        _state.CurrentStreak = newStreak;
        _state.StreakStarted = true;
        
        _persistence.Save(newStreak); 
        
        Console.WriteLine("New streak created and saved to streak.json!");
    }
    
    public void StartNewStreakFourteen()
    {
        var newStreak = new Services.STREAK 
        { 
            StartDate = DateTime.Today, 
            DurationDays = 14
        };
        newStreak.GenerateDays();
        
        _state.CurrentStreak = newStreak;
        _state.StreakStarted = true;
        
        _persistence.Save(newStreak); 
        
        Console.WriteLine("New streak created and saved to streak.json!");
    }
}