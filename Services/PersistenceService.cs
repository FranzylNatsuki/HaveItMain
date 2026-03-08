using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Linq;
using HaveItMain.Services;
using HaveItMain.ViewModels;

public class PersistenceService
{
    private const string FileName = "tasks.json";

    public void Save(AppState state)
    {
        // 1. Setup the options
        var options = new JsonSerializerOptions { WriteIndented = true };

        // 2. Turn the tasks into a JSON string
        var json = JsonSerializer.Serialize(state.Tasks, options);

        // 3. DEBUG: This will print the JSON to your Rider "Debug" window
        System.Diagnostics.Debug.WriteLine("--- SAVING JSON START ---");
        System.Diagnostics.Debug.WriteLine(json);
        System.Diagnostics.Debug.WriteLine("--- SAVING JSON END ---");

        // 4. Actually save it to the file
        File.WriteAllText(FileName, json);
    }

    public void Load(AppState state)
    {
        if (!File.Exists(FileName)) return;

        var json = File.ReadAllText(FileName);
        var tasks = JsonSerializer.Deserialize<ObservableCollection<TaskItemViewModel>>(json);

        if (tasks == null) return;

        // 1. Clear existing
        state.Tasks.Clear();

        // 2. Sort the incoming tasks by Date BEFORE adding them to the UI
        var sortedTasks = tasks.OrderBy(t => t.Date).ToList();

        // 3. Add them in the correct order
        foreach (var t in sortedTasks)
        {
            state.Tasks.Add(t);
        }
        
        CheckForDueTasks(state);
    }
    
    private void CheckForDueTasks(AppState state)
    {
        if (!state.NotificationsGlobal) return;

        // Filter tasks that are: 
        // 1. Due Today 
        // 2. Not finished 
        var dueTodayCount = state.Tasks.Count(t => 
            t.Date.Date == DateTime.Today && !t.IsFinished);

        if (dueTodayCount > 0)
        {
            string message = dueTodayCount == 1 
                ? "You have 1 task due today!" 
                : $"You have {dueTodayCount} tasks due today!";

            state.NotificationService?.ShowNotification("Have-It", message);
        
            // Optional: Play a "reminder" chime
            // AudioService.PlaySfx("reminder.wav");
        }
    }
}