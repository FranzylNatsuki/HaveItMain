using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;


public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    
    private void Dashboard_Click(object? sender, RoutedEventArgs e)
    {
        // cast DataContext to your MainWindowViewModel
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard(); // show dashboard
        }
    }

    private void Timer_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Timer(); 
        }
    }

    private void Streak_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Streak();
        }
    }

    private void Settings_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Settings();
        }
    }

    private void AccountSettings_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new AccountSettings();
        }
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard();
        }
    }
}