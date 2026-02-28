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
            Grid.SetColumnSpan(TasksPanel, 1);

            Streak.IsVisible = true;
            Timers.IsVisible = true;
        }
    }
    
    private async void EditSelectedTask(object? sender, RoutedEventArgs e)
    {
        var selectedTask = TASKLISTCONTAINER.SelectedItem as TaskItemViewModel;
        if (selectedTask == null)
        {
            var message = new SimpleMessageDialog("No Task Selected");
            await message.ShowDialog((Window)this.VisualRoot);
            return;
        }

        var window = (Window)this.VisualRoot;

        var dialog = new AddTaskMessage { PrefillTask = selectedTask };
        var result = await dialog.ShowDialog<TaskItemViewModel?>(window);

        if (result != null)
        {
            selectedTask.Title = result.Title;
            selectedTask.Date = result.Date;
            selectedTask.Urgency = result.Urgency;
            selectedTask.IsFinished = result.IsFinished;

            ViewModel?.SaveTasks();
        }
    }

    private void DeleteSelectedTask(object? sender, RoutedEventArgs e)
    {
        var selectedTask = TASKLISTCONTAINER.SelectedItem as TaskItemViewModel;
        if (selectedTask == null)
        {
            var message = new SimpleMessageDialog("No Task Selected");
            message.ShowDialog((Window)this.VisualRoot);
            return;
        }


        (DataContext as Dashboard)?.RemoveTask(selectedTask);
    }

    private void ComboBoxClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.ContextFlyout is MenuFlyout flyout)
        {
            flyout.ShowAt(btn);
        }
    }
}