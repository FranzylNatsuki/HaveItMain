using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CommunityToolkit.Mvvm.Input;
using HaveItMain.Services;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public partial class AccountSettings : ViewModelBase, IHasTitle
{
    public string Title => "ACCOUNT";
    private readonly AppState _state;
    private readonly PersistenceService _taskPersistence;
    private readonly StreakPersistenceService _streakPersistence;
    private readonly AccountPersistenceService _accountPersistence;
    public string[] GenderOptions { get; } = { "Male", "Female", "Other", "Prefer not to say" };

    public bool IsAnyFieldEditing => EditableAccount != null && (
        EditableAccount.IsFirstNameEditing || 
        EditableAccount.IsLastNameEditing || 
        EditableAccount.IsAddressEditing || 
        EditableAccount.IsBirthDateEditing || 
        EditableAccount.IsGenderEditing || 
        EditableAccount.IsContactNumberEditing || 
        EditableAccount.IsPasswordEditing);

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
    
    
    public void SaveChanges() 
    {
        // 1. Sync sandbox (what's in the textboxes) to real state (the account in the list)
        _state.UserAccount.FirstName = EditableAccount.FirstName;
        _state.UserAccount.LastName = EditableAccount.LastName;
        _state.UserAccount.Address = EditableAccount.Address;
        _state.UserAccount.BirthDate = EditableAccount.BirthDate;
        _state.UserAccount.Gender = EditableAccount.Gender;
        _state.UserAccount.ContactNumber = EditableAccount.ContactNumber;
        _state.UserAccount.Password = EditableAccount.Password;

        // 2. Lock the UI (The "Pencils" go away)
        DisableAllEditing();

        // 3. Save the WHOLE list to accounts.json
        _accountPersistence.Save(_state.AllAccounts.ToList());

        // 4. Update the "Last Logged In" session
        // NOTE: Use _state.UserAccount.Username because 'UserAccount' 
        // usually belongs to the State, not the ViewModel directly.
        var sessionService = new SessionService();
        sessionService.SaveSession(true, _state.UserAccount.Username);

        // 5. Trigger your new Snackbar!
        NotificationService.Show("Changes Saved");
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

        // Tell the UI that the buttons should now hide
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
    }
    [RelayCommand]
    public void ToggleFirstNameEdit()
    {
        EditableAccount.IsFirstNameEditing = !EditableAccount.IsFirstNameEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsFirstNameEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void ToggleLastNameEdit()
    {
        EditableAccount.IsLastNameEditing = !EditableAccount.IsLastNameEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsLastNameEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void ToggleBdayEdit()
    {
        EditableAccount.IsBirthDateEditing = !EditableAccount.IsBirthDateEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsBirthDateEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void ToggleGenderEdit()
    {
        EditableAccount.IsGenderEditing = !EditableAccount.IsGenderEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsGenderEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void ToggleContactEdit()
    {
        EditableAccount.IsContactNumberEditing = !EditableAccount.IsContactNumberEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsContactNumberEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void ToggleAddressEdit()
    {
        EditableAccount.IsAddressEditing = !EditableAccount.IsAddressEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsAddressEditing) NotificationService.Show("Field is now editable");
    }

    [RelayCommand]
    public void TogglePasswordEdit()
    {
        EditableAccount.IsPasswordEditing = !EditableAccount.IsPasswordEditing;
        this.RaisePropertyChanged(nameof(IsAnyFieldEditing));
        if (EditableAccount.IsPasswordEditing) NotificationService.Show("Field is now editable");
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