using System;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class TimerViewModel : ViewModelBase
{
    private string _title;
    private TimeSpan _duration;
    private bool _isOver;

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public TimeSpan Duration
    {
        get => _duration;
        set => this.RaiseAndSetIfChanged(ref _duration, value);
    }

    public bool IsOver
    {
        get => _isOver;
        set => this.RaiseAndSetIfChanged(ref _isOver, value);
    }

    public TimerViewModel(string title, TimeSpan duration, bool isOver = false)
    {
        _title = title;
        _duration = duration;
        _isOver = isOver;
    }

    public override string ToString()
    {
        // Format: "TimerTitle - 01:30:45 (Running/Finished)"
        string status = IsOver ? "Finished" : "Running";
        return $"{Title} - {Duration:hh\\:mm\\:ss} ({status})";
    }
}