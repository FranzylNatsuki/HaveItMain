using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class AccountSettingsView : UserControl
{
    public AccountSettingsView()
    {
        InitializeComponent();
    }

    private void EraseJson(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AccountSettings vm)
        {
            vm.EraseAllData();
            warning();
            Console.WriteLine("Data wiped successfully.");
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
}