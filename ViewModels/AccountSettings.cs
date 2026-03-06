using HaveItMain.Services;

namespace HaveItMain.ViewModels;

public class AccountSettings : ViewModelBase, IHasTitle
{
    public string Title => "ACCOUNT";
    private readonly AppState _state;

    // Inject the state so we can modify the streak
    public AccountSettings(AppState state)
    {
        _state = state;
    }

    public void EraseAllData()
    {
        // 1. Reset Streak in memory
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