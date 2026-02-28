using System;
using System.Reactive.Linq;
using Avalonia.Threading;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class TimerViewModel : ViewModelBase
{
    private string _title;
    private TimeSpan _duration;
    private bool _isOver;
    public string DisplayTime => Duration.ToString(@"hh\:mm\:ss"); // bind this to TextBlock

    private IDisposable? _timerSubscription;

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
    
    public void Start()
    {
        if (IsOver) return;

        // Dispose previous if any
        _timerSubscription?.Dispose();

        _timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeWhile(_ => Duration.TotalSeconds > 0)
            .Subscribe(_ =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Duration = Duration - TimeSpan.FromSeconds(1);
                    this.RaisePropertyChanged(nameof(DisplayTime)); // notify UI
                    if (Duration.TotalSeconds <= 0)
                        IsOver = true;
                });
            });
    }

    public void Stop()
    {
        _timerSubscription?.Dispose();
    }
    public override string ToString()
    {
        // Format: "TimerTitle - 01:30:45 (Running/Finished)"
        string status = IsOver ? "Finished" : "Running";
        return $"{Title} - {Duration:hh\\:mm\\:ss} ({status})";
    }
}