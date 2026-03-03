using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class TimerViewModel : ViewModelBase
{
    public event Action<TimerViewModel>? OnFinished;
    private string _title;
    private TimeSpan _duration;
    private readonly TimeSpan _totalDuration;
    private readonly bool isNotified = true;
    public double Progress => Math.Max(0, Duration.TotalSeconds / _totalDuration.TotalSeconds);
    public double DashOffset => (1 - Progress) * 100; // 100 = full circle length
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
        set
        {
            this.RaiseAndSetIfChanged(ref _duration, value);
            this.RaisePropertyChanged(nameof(DisplayTime));
            this.RaisePropertyChanged(nameof(Progress)); // important
        }
    }

    public bool IsOver
    {
        get => _isOver;
        set => this.RaiseAndSetIfChanged(ref _isOver, value);
    }

    public TimerViewModel(string title, TimeSpan duration, bool isNotified, bool isOver = false)
    {
        _title = title;
        _duration = duration;
        _isOver = isOver;
        _totalDuration = duration;  // store original for progress
        this.isNotified = isNotified;
    }
    
    public void Pause()
    {
        // Just dispose the timer but don't reset Duration
        _timerSubscription?.Dispose();
        _timerSubscription = null;
    }
    
    public void Resume()
    {
        if (IsOver || _timerSubscription != null) return; // already running or finished

        _timerSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
            .TakeWhile(_ => Duration.TotalSeconds > 0)
            .Subscribe(_ =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Duration -= TimeSpan.FromSeconds(1);
                    this.RaisePropertyChanged(nameof(DisplayTime));
                    if (Duration.TotalSeconds <= 0)
                        IsOver = true;
                });
            });
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
                    {
                        IsOver = true;
                        if (isNotified)
                        {
                            Notify($"{Title} timer finished!");
                        }
                        _timerSubscription?.Dispose();
                        OnFinished?.Invoke(this); 
                    }
                });
            });
    }
    public void Notify(string message) // REPLACE IN WINDOWS
    {
        try
        {
            string appleScript = $"display notification \"{message}\" with title \"HaveItMain\"";

            var process = new ProcessStartInfo
            {
                FileName = "osascript",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            process.ArgumentList.Add("-e");
            process.ArgumentList.Add(appleScript);

            Process.Start(process);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to show notification: " + ex.Message);
        }
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