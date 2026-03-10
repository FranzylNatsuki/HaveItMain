using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HaveItMain.Services;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class AccountSettingsView : UserControl
{
    public AccountSettingsView()
    {
        InitializeComponent();
    }

    private async void EraseJson(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AccountSettings vm)
        {
            AudioService.PlaySfx("Discarded.mp3");
            // 1. Setup the Confirmation Dialog
            var confirmDialog = new ConfirmationDialog("Are you sure? This will wipe all tasks and streaks!");
            var topLevel = TopLevel.GetTopLevel(this);

            if (topLevel is Window parentWindow)
            {
                // 2. Wait for the user to click a button
                // The result will be whatever you passed into Close()
                var result = await confirmDialog.ShowDialog<bool>(parentWindow);

                if (result == true)
                {
                    // 3. User clicked "Proceed"
                    vm.EraseAllData();

                    // Show the success message using your existing simple dialog
                    var successDialog = new SimpleMessageDialog("Successfully Deleted!");
                    await successDialog.ShowDialog(parentWindow);

                    Console.WriteLine("Data wiped successfully.");
                }
                else
                {
                    // User clicked "Cancel"
                    Console.WriteLine("Wipe cancelled by user.");
                }
            }
        }
    }

    private async void warning()
    {
        var dialog = new SimpleMessageDialog("Successfully Deleted!");
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is Window parentWindow)
        {
            await dialog.ShowDialog(parentWindow);
        }
    }
    
    private async void Export_Click(object? sender, RoutedEventArgs e)
    {
        // Get the storage provider from the top-level window
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Your Tasks",
            SuggestedFileName = "my_tasks.json",
            DefaultExtension = "json",
            FileTypeChoices = new List<FilePickerFileType> 
            { 
                new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } 
            }
        });

        if (file != null)
        {
            // Use the service from App
            await App.ServiceState.ExportTasks(file.Path.LocalPath);
        }
    }

    private async void Import_Click(object? sender, RoutedEventArgs e)
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return;

        var jsonFilter = new FilePickerFileType("JSON Files")
        {
            Patterns = new[] { "*.json" }
        };

        var result = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select tasks.json to Import",
            AllowMultiple = false, 
            FileTypeFilter = new List<FilePickerFileType> { jsonFilter }
        });

        if (result != null && result.Count > 0)
        {
            await App.ServiceState.ImportTasks(result[0].Path.LocalPath);
        }
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

    private async void Logout_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = (Window)this.VisualRoot!;

        // 2. Pop the Confirmation Dialog
        // (Assuming your ConfirmationDialog constructor takes a string message)
        var dialog = new ConfirmationDialog("Are you sure you want to log out of Have-It?");
        var result = await dialog.ShowDialog<bool>(window);

        if (result)
        {
            var sessionService = new SessionService();
            sessionService.ClearSession();
            App.ServiceState.IsLoggedIn = false;

            // 4. Open Landing
            var landing = new Landing
            {
                DataContext = new Landing() 
            };
            landing.Show();
            
            window.Close();
        }
        else
        {
            window.Focus();
        }
    }
}