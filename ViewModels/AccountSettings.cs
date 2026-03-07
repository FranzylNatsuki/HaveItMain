using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class AccountSettings : ViewModelBase, IHasTitle
{
    public string Title => "ACCOUNT";
    private readonly AppState _state;
    private readonly PersistenceService _taskPersistence;
    private readonly StreakPersistenceService _streakPersistence;
    private readonly AccountPersistenceService _accountPersistence;
    public string[] GenderOptions { get; } = { "Male", "Female", "Other", "Prefer not to say" };

// Property to toggle the password character
    private char _passwordChar = '*';
    public char PasswordChar
    {
        get => _passwordChar;
        set => this.RaiseAndSetIfChanged(ref _passwordChar, value);
    }

    public void Reveal()
    {
        PasswordChar = PasswordChar == '*' ? '\0' : '*';
    }
    
    private Account _editableAccount;
    public Account EditableAccount 
    { 
        get => _editableAccount; 
        set => this.RaiseAndSetIfChanged(ref _editableAccount, value); 
    }
    public AppState State => _state; 
    private Account _backupAccount;

    // Inject the state so we can modify the streak
    public AccountSettings(AppState state)
    {
        _state = state;
        _taskPersistence = new PersistenceService();
        _streakPersistence = new StreakPersistenceService();
        _accountPersistence = new AccountPersistenceService();
    
        // This creates your "EditableAccount" (The Sandbox)
        ResetEditableAccount();
    }
    
    private void ResetEditableAccount()
    {
        var current = _state.UserAccount;
        EditableAccount = new Account
        {
            FirstName = current.FirstName,
            LastName = current.LastName,
            Address = current.Address,
            BirthDate = current.BirthDate,
            Gender = current.Gender,
            ContactNumber = current.ContactNumber,
            Password = current.Password,
            Username = current.Username
        };
    }
    
    private void CreateBackup()
    {
        // Clone the current data so we can revert if needed
        var current = _state.UserAccount;
        _backupAccount = new Account
        {
            FirstName = current.FirstName,
            LastName = current.LastName,
            Address = current.Address,
            BirthDate = current.BirthDate,
            Gender = current.Gender,
            ContactNumber = current.ContactNumber,
            Password = current.Password,
        };
    }
    
    
    public void SaveChanges(List<Account> accounts) // Change the parameter to List<Account>
    {
// 1. Sync sandbox to real state
        _state.UserAccount.FirstName = EditableAccount.FirstName;
        _state.UserAccount.LastName = EditableAccount.LastName;
        _state.UserAccount.Address = EditableAccount.Address;
        _state.UserAccount.BirthDate = EditableAccount.BirthDate;
        _state.UserAccount.Gender = EditableAccount.Gender;
        _state.UserAccount.ContactNumber = EditableAccount.ContactNumber;
        _state.UserAccount.Password = EditableAccount.Password;

        // 2. Lock the UI
        DisableAllEditing();

        // 3. Save the WHOLE list. 
        // Now that the Service expects a List, this line won't error!
        _accountPersistence.Save(_state.AllAccounts.ToList());
    }
    
    public void CancelChanges()
    {
        ResetEditableAccount();
        DisableAllEditing();

        System.Diagnostics.Debug.WriteLine("Changes discarded.");
    }

    private void DisableAllEditing()
    {
        if (EditableAccount == null) return;
        EditableAccount.IsFirstNameEditing = false;
        EditableAccount.IsLastNameEditing = false;
        EditableAccount.IsBirthDateEditing = false;
        EditableAccount.IsGenderEditing = false;
        EditableAccount.IsContactNumberEditing = false;
        EditableAccount.IsAddressEditing = false;
        EditableAccount.IsPasswordEditing = false;
    }
    public void ToggleFirstNameEdit() => EditableAccount.IsFirstNameEditing = true;
    public void ToggleLastNameEdit() => EditableAccount.IsLastNameEditing = true;
    public void ToggleBdayEdit() => EditableAccount.IsBirthDateEditing = true;
    public void ToggleGenderEdit() => EditableAccount.IsGenderEditing = true;
    public void ToggleContactEdit() => EditableAccount.IsContactNumberEditing = true;
    public void ToggleAddressEdit() => EditableAccount.IsAddressEditing = true;
    public void TogglePasswordEdit() => EditableAccount.IsPasswordEditing = true;

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