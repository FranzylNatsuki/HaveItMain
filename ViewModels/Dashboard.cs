using System;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Dashboard : ViewModelBase, IHasTitle
{
    private const string TasksFileName = "tasks.json";
    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public ObservableCollection<TimerViewModel> Timers => App.State.Timers;
    public ReactiveCommand<Unit, Unit> AddTimerCommand { get; }
    public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }

    private TaskItemViewModel? _lastDeletedTask;
    private int _lastDeletedIndex = -1;
    public bool CanUndo => _lastDeletedTask != null;

    public string Title => "DASHBOARD";

    public Dashboard()
    {
        LoadTasks();
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