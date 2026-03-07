using System.Collections.Generic;
using Avalonia;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Settings : ViewModelBase, IHasTitle
{
    private readonly AppState _state;
    public AppState State => _state; 
    public string Title => "SETTINGS";

    private readonly AccountPersistenceService _accountPersistence;

    public Settings(AppState state)
    {
        _state = state;
        _accountPersistence = new AccountPersistenceService();
    }

    // Call this whenever a checkbox is clicked to make it permanent
    public void SaveSystemSettings()
    {
        // This saves the current State.UserAccount (including the new booleans)
        _accountPersistence.Save(_state.UserAccount);
    }
}