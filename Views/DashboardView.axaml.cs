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
            AudioService.PlaySfx("AddTimerTask.mp3");
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
                    AudioService.PlaySfx("AddTimerTask.mp3");
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
            AudioService.PlaySfx("AddTimerTask.mp3");
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
            AudioService.PlaySfx("Discarded.mp3");
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
            AudioService.PlaySfx("Discarded.mp3");
        }
    }
    
    private async void UndoDelete(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not Dashboard vm)
            return;

        if (!vm.CanUndo) // nothing to undo
        {
            NotificationService.Show("No task to undo");
            return;
        }
        
        AudioService.PlaySfx("AddTimerTask.mp3");
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
            NotificationService.Show("No Task Selected");
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

    private async void DeleteSelectedTask(object? sender, RoutedEventArgs e)
    {
        var selectedTask = TASKLISTCONTAINER.SelectedItem as TaskItemViewModel;
        var window = (Window)this.VisualRoot!;

        if (selectedTask == null)
        {
            NotificationService.Show("No Task Selected");
            window.Focus(); // Grab focus back for shortcuts
            return;
        }
    
        // Optional: Add a confirmation here so they don't accidental-delete!
        (DataContext as Dashboard)?.RemoveTask(selectedTask);
        AudioService.PlaySfx("Discarded.mp3");
        window.Focus();
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

    private void IsFinishedSound(object? sender, RoutedEventArgs e)
    {
        if (sender is CheckBox finishedBox)
        {
            if (finishedBox.IsChecked == true)
            {
                AudioService.PlaySfx("finish_task.mp3");
            }
        }
    }
    
    private void SortTimeAscending(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Dashboard vm) vm.SortTimeAscending();
    }

    private void SortTimeDescending(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Dashboard vm) vm.SortTimeDescending();
    }

    private void SortAlphaAscending(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Dashboard vm) vm.SortAlphaAscending();
    }

    private void SortAlphaDescending(object? sender, RoutedEventArgs e)
    {
        if (DataContext is Dashboard vm) vm.SortAlphaDescending();
    }
}