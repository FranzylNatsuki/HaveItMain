using System.IO;
using HaveItMain.Services;

namespace HaveItMain.ViewModels;

public class AccountSettings : ViewModelBase, IHasTitle
{
    public string Title => "ACCOUNT";
    private readonly AppState _state;
    private readonly PersistenceService _taskPersistence;
    private readonly StreakPersistenceService _streakPersistence;

    // Inject the state so we can modify the streak
    public AccountSettings(AppState state)
    {
        _state = state;
        // You might need to initialize these or pass them in from MainWindowViewModel
        _taskPersistence = new PersistenceService();
        _streakPersistence = new StreakPersistenceService();
    }

    public void EraseAllData()
    {
        _state.CurrentStreak = null;
        _state.StreakStarted = false;

        // 2. Delete the files
        string[] files = { "streak.json", "tasks.json" };
        foreach (var file in files)
        {
            if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);
        }
        
        // 3. Optional: Clear tasks too
        _state.Tasks.Clear();
    }
}