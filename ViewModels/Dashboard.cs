using System;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Dashboard : ViewModelBase, IHasTitle
{
    private readonly AppState _state;
    private readonly StreakService _streakService;
    public bool StreakStarted => _state.StreakStarted;
    public string FormattedStartDate => _state.CurrentStreak?.StartDate.ToString("MMMM dd, yyyy") ?? "No Start Date";
    public bool IsTodayCompleted => _state.CurrentStreak?.IsTodayDone ?? false;
    public int CurrentTally => _state.CurrentStreak?.CompletedDaysCount ?? 0;
    public bool Is7DayStreak => _state.CurrentStreak?.DurationDays == 7;
    public bool Is14DayStreak => _state.CurrentStreak?.DurationDays == 14;
    public bool Show14DayActiveUI => StreakStarted && Is14DayStreak;
    
    private readonly ObservableAsPropertyHelper<bool> _hasNoTasks;
    private readonly ObservableAsPropertyHelper<bool> _hasNoTimers;
    
    public bool HasNoTasks => _hasNoTasks.Value;
    public bool HasNoTimers => _hasNoTimers.Value;
    
    private const string TasksFileName = "tasks.json";
    public ObservableCollection<TaskItemViewModel> Tasks => App.ServiceState.Tasks;
    public ObservableCollection<TimerViewModel> Timers => App.ServiceState.Timers;
    public ReactiveCommand<Unit, Unit> AddTimerCommand { get; }
    public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }

    private TaskItemViewModel? _lastDeletedTask;
    private int _lastDeletedIndex = -1;
    public bool CanUndo => _lastDeletedTask != null;

    public string Title => "DASHBOARD";

    public Dashboard(AppState state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        
        _streakService = new StreakService();
        
        _state = state; // No underscore? Check your Dashboard class fields!

        if (_state?.CurrentStreak != null)
        {
            _state.CurrentStreak.PropertyChanged += (s, e) => {
                this.RaisePropertyChanged(nameof(StreakStarted));
                this.RaisePropertyChanged(nameof(CurrentTally));
                this.RaisePropertyChanged(nameof(IsTodayCompleted));
            };
        }
        _state.WhenAnyValue(x => x.StreakStarted, x => x.CurrentStreak)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(StreakStarted));
                this.RaisePropertyChanged(nameof(Is7DayStreak));
                this.RaisePropertyChanged(nameof(Is14DayStreak));
                this.RaisePropertyChanged(nameof(Show14DayActiveUI));
            });
        _hasNoTasks = this.WhenAnyValue(x => x.Tasks.Count)
            .Select(count => count == 0)
            .ToProperty(this, x => x.HasNoTasks);
        _hasNoTimers = this.WhenAnyValue(x => x.Timers.Count)
            .Select(count => count == 0)
            .ToProperty(this, x => x.HasNoTimers);
        _state.WhenAnyValue(x => x.StreakStarted, x => x.CurrentStreak)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(StreakStarted));
                this.RaisePropertyChanged(nameof(Is7DayStreak));
                this.RaisePropertyChanged(nameof(Is14DayStreak));
            });
    }

    public void AddTask(TaskItemViewModel task)
    {
        Tasks.Add(task);
    }

    public void AddTimer(TimerViewModel timer)
    {
        timer.OnFinished += RemoveTimer;
        Timers.Add(timer);
    }
    
    public void RemoveTask(TaskItemViewModel task)
    {
        _lastDeletedIndex = Tasks.IndexOf(task);
        _lastDeletedTask = task;
        Tasks.Remove(task);
    }

    public void RemoveTimer(TimerViewModel timer)
    {
        Timers.Remove(timer);
    }
    public void UndoDelete()
    {
        if (_lastDeletedTask == null) return;

        if (_lastDeletedIndex >= 0 && _lastDeletedIndex <= Tasks.Count)
            Tasks.Insert(_lastDeletedIndex, _lastDeletedTask);
        else
            Tasks.Add(_lastDeletedTask);

        _lastDeletedTask = null;
        _lastDeletedIndex = -1;
    }
// Now update these methods to use the service!
    public void StartNewStreak() 
    {
        _streakService.StartNewStreak(_state, 7);
        // Save the change
        new StreakPersistenceService().Save(_state.CurrentStreak);
    }

    public void StartNewStreakFourteen() 
    {
        _streakService.StartNewStreak(_state, 14);
        // Save the change
        new StreakPersistenceService().Save(_state.CurrentStreak);
    }
}