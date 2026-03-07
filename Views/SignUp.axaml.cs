using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HaveItMain.Services;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class SignUp : Window
{
    public SignUp()
    {
        InitializeComponent();
    }
    private void Register_Click(object? sender, RoutedEventArgs e)
    {
// 1. Create the object
        var newAcc = new Account
        {
            Username = TxtUser.Text,
            Password = TxtPass.Text,
            FirstName = TxtFirst.Text,
            LastName = TxtLast.Text,
            Address = TxtAddress.Text,
            BirthDate = DateBirth.SelectedDate, // This fits your DateTimeOffset? BirthDate
            Gender = (ComboGender.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Unknown",
            ContactNumber = TxtContact.Text
        };

        // 2. COMPOUND: Add to the existing list
        App.ServiceState.AllAccounts.Add(newAcc);

        // 3. PERSIST: Save the whole list (Old users + New user)
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        string json = System.Text.Json.JsonSerializer.Serialize(App.ServiceState.AllAccounts, options);
        System.IO.File.WriteAllText("accounts.json", json);

        // 4. SESSION: Set the current user
// 4. SESSION: Set the current user
        App.ServiceState.UserAccount = newAcc;
        App.ServiceState.IsLoggedIn = true;

// 5. NAVIGATE: Create the ViewModel first!
// Assuming your MainWindowViewModel takes the AppState/ServiceState
        var vm = new MainWindowViewModel(App.ServiceState);

        var main = new MainWindow
        {
            DataContext = vm
        };

        main.Show();
        this.Close();
    }
    
    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        // Create the Landing window
        var landing = new Landing(); 
    
        // Show it
        landing.Show();
    
        // Close the current SignUp window
        this.Close();
    }
    
    private void Reveal(object? sender, RoutedEventArgs e)
    {
        if (TxtPass.PasswordChar == '*')
        {
            // Show the password
            TxtPass.PasswordChar = '\0'; 
        
            // Update UI Icons
            eyeopen.IsVisible = true;
            eyeclosed.IsVisible = false;
        }
        else
        {
            // Hide the password
            TxtPass.PasswordChar = '*';
        
            // Update UI Icons
            eyeopen.IsVisible = false;
            eyeclosed.IsVisible = true;
        }
    }
}