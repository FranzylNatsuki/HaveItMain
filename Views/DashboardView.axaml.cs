using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using ReactiveUI;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class DashboardView : UserControl
{
    private bool Dashboard_Enlarged = false;
    public DashboardView()
    {
        InitializeComponent();
        DataContext = new Dashboard();
    }
    
    private Dashboard ViewModel => DataContext as Dashboard;

    private async void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var window = (Window)this.VisualRoot;

        var dialog = new AddTaskMessage();

        var result = await dialog.ShowDialog<TaskItemViewModel?>(window);

        if (result != null)
        {
            (DataContext as Dashboard)?.AddTask(result);
        }
    }

    private void DeleteTask(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.DataContext is TaskItemViewModel task &&
            DataContext is Dashboard vm)
        {
            vm.RemoveTask(task);
        }
    }

    private void Expand(object? sender, RoutedEventArgs e)
    {
        Dashboard_Enlarged = !Dashboard_Enlarged;

        if (Dashboard_Enlarged)
        {
            // Make Tasks fill both columns
            Grid.SetColumnSpan(TasksPanel, 2);

            // Hide right side
            Streak.IsVisible = false;
            Timers.IsVisible = false;
        }
        else
        {
            // Restore normal layout
            Grid.SetColumnSpan(TasksPanel, 1);

            Streak.IsVisible = true;
            Timers.IsVisible = true;
        }
    }
    
    private void EditSelectedTask(object? sender, RoutedEventArgs e)
    {
        var selectedTask = TASKLISTCONTAINER.SelectedItem as TaskItemViewModel;
        if (selectedTask == null) return;

        var window = (Window)this.VisualRoot;

        var dialog = new AddTaskMessage { PrefillTask = selectedTask };
        var result = dialog.ShowDialog<TaskItemViewModel?>(window).Result; // can await if async

        if (result != null)
        {
            // Copy edited values
            selectedTask.Title = result.Title;
            selectedTask.Date = result.Date;
            selectedTask.Urgency = result.Urgency;
            selectedTask.isFinished = result.isFinished;

            // Save to JSON
            (DataContext as Dashboard)?.SaveTasks();

            // Force ListBox refresh (if needed)
            TASKLISTCONTAINER.SelectedItem = null;
            TASKLISTCONTAINER.SelectedItem = selectedTask;
        }
    }

    private void DeleteSelectedTask(object? sender, RoutedEventArgs e)
    {
        var selectedTask = TASKLISTCONTAINER.SelectedItem as TaskItemViewModel;
        if (selectedTask == null) return;

        (DataContext as Dashboard)?.RemoveTask(selectedTask);
    }

    private void ComboBoxClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.ContextFlyout is MenuFlyout flyout)
        {
            flyout.ShowAt(btn); // opens the flyout at the button
        }
    }
}