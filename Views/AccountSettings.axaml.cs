using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
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
}