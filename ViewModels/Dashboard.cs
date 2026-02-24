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
    public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }

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
    public void RemoveTask(TaskItemViewModel task)
    {
        Tasks.Remove(task);
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
}