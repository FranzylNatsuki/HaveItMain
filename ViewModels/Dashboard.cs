using System;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Dashboard : ViewModelBase, IHasTitle
{
    private readonly ObservableAsPropertyHelper<bool> _hasNoTasks;
    private readonly ObservableAsPropertyHelper<bool> _hasNoTimers;
    
    public bool HasNoTasks => _hasNoTasks.Value;
    public bool HasNoTimers => _hasNoTimers.Value;
    
    private const string TasksFileName = "tasks.json";
    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public ObservableCollection<TimerViewModel> Timers => App.ServiceState.Timers;
    public ReactiveCommand<Unit, Unit> AddTimerCommand { get; }
    public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }

    private TaskItemViewModel? _lastDeletedTask;
    private int _lastDeletedIndex = -1;
    public bool CanUndo => _lastDeletedTask != null;

    public string Title => "DASHBOARD";

    public Dashboard()
    {
        LoadTasks();
        _hasNoTasks = this.WhenAnyValue(x => x.Tasks.Count)
            .Select(count => count == 0)
            .ToProperty(this, x => x.HasNoTasks);
        _hasNoTimers = this.WhenAnyValue(x => x.Timers.Count)
            .Select(count => count == 0)
            .ToProperty(this, x => x.HasNoTimers);
    }

    public void AddTask(TaskItemViewModel task)
    {
        Tasks.Add(task);
        SaveTasks();
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
        SaveTasks();
    }

    public void RemoveTimer(TimerViewModel timer)
    {
        Timers.Remove(timer);
        SaveTasks();
    }
    
    public void SaveTasks()
    {
        try
        {
            var json = JsonSerializer.Serialize(Tasks, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TasksFileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving tasks: " + ex.Message);
        }
    }

    // Load tasks from JSON
    private void LoadTasks()
    {
        try
        {
            if (File.Exists(TasksFileName))
            {
                var json = File.ReadAllText(TasksFileName);
                var tasks = JsonSerializer.Deserialize<ObservableCollection<TaskItemViewModel>>(json);

                if (tasks != null)
                {
                    foreach (var t in tasks)
                        Tasks.Add(t); // keep UI bound
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading tasks: " + ex.Message);
        }
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

        SaveTasks();
    }
}