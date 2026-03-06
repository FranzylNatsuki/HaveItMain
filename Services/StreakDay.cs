using System;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class StreakDay : ViewModelBase
{
    public DateTime Date { get; set; }

    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        // This line tells the UI to flip the "IsVisible" switch
        set => this.RaiseAndSetIfChanged(ref _isCompleted, value);
    }
}