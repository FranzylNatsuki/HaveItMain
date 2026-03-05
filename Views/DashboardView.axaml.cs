using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    private async void SetTaskTimerClick(object? sender, RoutedEventArgs e)
    {
        // 1. Get the Task from the button's DataContext
        // Note: The Image is the sender if you put PointerPressed on the Image. 
        // It's safer to check the parent if needed, but let's assume sender is the control holding the data.
        if (sender is Control control && control.DataContext is TaskItemViewModel selectedTask)
        {
            // 2. Get the Dashboard ViewModel (the DataContext of the UserControl itself)
            if (this.DataContext is Dashboard vm)
            {
                var window = (Window)this.VisualRoot!;
            
                // 3. Open the dialog and pass the pre-filled task
                var dialog = new AddTimerMessage { PrefillTask = selectedTask };
                var result = await dialog.ShowDialog<TimerViewModel?>(window);
            
                if (result != null)
                {
                    vm.AddTimer(result);
                }
            }
        }
    }
    private async void AddTimerMethod(object? sender, PointerPressedEventArgs e)
    {
        var window = (Window)this.VisualRoot;

        var dialog = new AddTimerMessage();

        var result = await dialog.ShowDialog<TimerViewModel?>(window);

        if (result != null)
        {
            (DataContext as Dashboard)?.AddTimer(result);
        }
    }
    private void PauseTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is TimerViewModel timer)
            timer.Pause();
    }

    private void ResumeTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is TimerViewModel timer)
            timer.Resume();
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
    
    private void DeleteTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.DataContext is TimerViewModel timer &&
            DataContext is Dashboard vm)
        {
            timer.Stop();
            vm.RemoveTimer(timer);
        }
    }
    
    private async void UndoDelete(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not Dashboard vm)
            return;

        if (!vm.CanUndo) // nothing to undo
        {
            var message = new SimpleMessageDialog("No task to undo");
            await message.ShowDialog((Window)this.VisualRoot!);
            return;
        }

        vm.UndoDelete();
    }

    private void Expand(object? sender, RoutedEventArgs e)
    {
        Dashboard_Enlarged = !Dashboard_Enlarged;

        if (Dashboard_Enlarged)
        {
            ExpandButton.IsVisible = false;
            ShrinkButton.IsVisible = true;
            Grid.SetColumnSpan(TasksPanel, 2);
            Streak.IsVisible = false;
            Timers.IsVisible = false;
        }
        else
        {
            ExpandButton.IsVisible = true;
            ShrinkButton.IsVisible = false;
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
    
    private void FocusModeEnable(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is TimerViewModel timer)
        {
            var parentWindow = btn.GetVisualRoot() as Window;
            if (parentWindow != null)
            {
                parentWindow.WindowState = WindowState.Minimized;
            }
            var focus = new FocusMode(timer);
            
            focus.Closed += (_, _) =>
            {
                if (parentWindow != null)
                    parentWindow.WindowState = WindowState.Normal;
            };

            focus.Show();
        }
    }
}