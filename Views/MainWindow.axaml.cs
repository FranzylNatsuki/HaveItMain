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
        // DataContext = new MainWindowViewModel();
    }

    public void initialize_core_settings()
    {
        LEFTMENUBAR.IsVisible = true;
    }
    
    private void ComboBoxClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.ContextFlyout is MenuFlyout flyout)
        {
            flyout.ShowAt(btn);
        }
    }
    
    private void Demo_VideoClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show(); // or ShowDialog(this) if modal
    }
    
    private void Dashboard_Click(object? sender, RoutedEventArgs e)
    {
        // cast DataContext to your MainWindowViewModel
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard(vm.State); // show dashboard
        }
    }

    private void Timer_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            var timerVM = new Timer(App.ServiceState.Timers);
            vm.CurrentViewModel = timerVM;
        }
    }
    private void Streak_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Streak(vm.State);
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
            vm.CurrentViewModel = new AccountSettings(vm.State);
        }
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard(vm.State);
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
        var dialog = new AddTaskMessage();
        var result = await dialog.ShowDialog<TaskItemViewModel?>(this);

        if (result != null)
        {
            if (DataContext is MainWindowViewModel mainVM)
            {
                if (mainVM.CurrentViewModel is Dashboard dashboardVM)
                {
                    dashboardVM.AddTask(result);
                }
                else
                {
                    mainVM.State.Tasks.Add(result);
                }
            }
        }
    }

    private void Landing(object? sender, RoutedEventArgs e)
    {
        var landing = new AboutNoLogin_();
        landing.Show();
    }
}
