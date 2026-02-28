using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;


public partial class MainWindow : Window
{
    
    private bool _sidebarCollapsed = false;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }

    public void initialize_core_settings()
    {
        LEFTMENUBAR.IsVisible = true;
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
            // Create Timer ViewModel with the shared Timers collection
            var timerVM = new Timer(vm.Dashboard.Timers);
            vm.CurrentViewModel = timerVM;
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

    private void LEFTMENUBAR_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var sidebarColumn = Windowgrid.ColumnDefinitions[0];

        if (_sidebarCollapsed)
        {
            LeftMenuContent.IsVisible = true;
            sidebarColumn.Width = new GridLength(72); // expanded width
        }
        else
        {
            LeftMenuContent.IsVisible = false;
            sidebarColumn.Width = new GridLength(10); // collapsed width
        }

        _sidebarCollapsed = !_sidebarCollapsed;
    }

    private async void NEW_ROUTINE(object? sender, RoutedEventArgs e)
    {
        // Dim the current window (parent)
        // this.Opacity = 0.5;
    
        var dialog = new SimpleMessageDialog("Task saved!");
        await dialog.ShowDialog(this); // 'this' = parent Window
    
        // Restore opacity
        // this.Opacity = 1;
    }
}
