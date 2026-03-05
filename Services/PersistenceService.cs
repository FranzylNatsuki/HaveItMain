using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using HaveItMain.Services;
using HaveItMain.ViewModels;

public class PersistenceService
{
    private const string FileName = "tasks.json";

    public void Save(AppState state)
    {
        var json = JsonSerializer.Serialize(state.Tasks, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(FileName, json);
    }

    public void Load(AppState state)
    {
        if (!File.Exists(FileName))
            return;

        var json = File.ReadAllText(FileName);

        var tasks = JsonSerializer.Deserialize<ObservableCollection<TaskItemViewModel>>(json);

        if (tasks == null)
            return;

        state.Tasks.Clear();

        foreach (var t in tasks)
            state.Tasks.Add(t);
    }
}