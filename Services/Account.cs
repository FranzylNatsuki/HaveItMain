using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HaveItMain.Services;

public partial class Account : ObservableObject
{
    [ObservableProperty] private string _username = "";
    [ObservableProperty][NotifyPropertyChangedFor(nameof(FullName))] private string _firstName = "";
    [ObservableProperty][NotifyPropertyChangedFor(nameof(FullName))] private string _lastName = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _gender = "";
    [ObservableProperty] private string _contactNumber = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private bool _isSignedIn = false;
    
    [ObservableProperty] private bool _isFirstNameEditing;
    [ObservableProperty] private bool _isLastNameEditing;
    [ObservableProperty] private bool _isAddressEditing;
    [ObservableProperty] private bool _isBirthDateEditing;
    [ObservableProperty] private bool _isGenderEditing;
    [ObservableProperty] private bool _isContactNumberEditing;
    [ObservableProperty] private bool _isPasswordEditing;

    private DateTimeOffset? _birthDate = DateTimeOffset.Now.Date;

    public DateTimeOffset? BirthDate
    {
        get => _birthDate;
        set 
        {
            SetProperty(ref _birthDate, value?.Date);
        }
    }
    public string FullName => $"{FirstName} {LastName}";
}