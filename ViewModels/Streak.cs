using System;
using System.Collections.ObjectModel;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Streak : ViewModelBase, IHasTitle
{
    public string Title => "STREAK";
    
    private readonly AppState _state;
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

        this.RaisePropertyChanged(nameof(StreakStarted));

        _state.WhenAnyValue(x => x.StreakStarted)
            .Subscribe(_ => {
                this.RaisePropertyChanged(nameof(StreakStarted));
            });
        Console.WriteLine($"Streak Started: {_state.StreakStarted}");
    }
    
    public void ResetStreak()
    {
        _state.CurrentStreak = null;
        _state.StreakStarted = false;
        
        try 
        {
            string path = "streak.json";
            if (System.IO.File.Exists(path)) 
            {
                System.IO.File.Delete(path);
                Console.WriteLine("File successfully deleted.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not delete file: {ex.Message}");
        }
        this.RaisePropertyChanged(nameof(StreakStarted));
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