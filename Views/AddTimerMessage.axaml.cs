using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class AddTimerMessage : Window
{
    public INotificationService? NotificationService { get; set; }
    
    public TaskItemViewModel? PrefillTask { get; set; }
    
    public AddTimerMessage()
    {
        InitializeComponent();
        
        this.Opened += (_, _) =>
        {
            if (PrefillTask == null) return;

            TaskTitleBox.Text = PrefillTask.Title;
        }; 
    }
    
    public async Task ShowDialogAsync(Window owner)
    {
        await ShowDialog(owner);
    }
    
    private void Discard(object? sender, RoutedEventArgs e)
    {
        Close();
    }
    
    private async void Warning(string message)
    {
        var dialog = new SimpleMessageDialog(message);
        await dialog.ShowDialog(this);
    }

    private void Add(object? sender, RoutedEventArgs e)
    {
        var title = TaskTitleBox.Text ?? "New Timer";
        var hours = (int)(HoursBox.Value ?? 0);
        var minutes = (int)(MinutesBox.Value ?? 0);
        var seconds = (int)(SecondsBox.Value ?? 0);
        var isNotified = false;

        if (IsNotify.IsChecked == true)
        {
            isNotified = true;
        }

        if (hours == 0 && minutes == 0 && seconds == 0)
        {
            Warning("Duration cannot be 0!");
            HoursBox.Value = 0;
            MinutesBox.Value = 0;
            SecondsBox.Value = 0;
            return;
        }
        if (hours > 23)
        {
            Warning("Maximum value for hour is 23!");
            HoursBox.Value = 0;
            MinutesBox.Value = 0;
            SecondsBox.Value = 0;
            return;
        }
        if (minutes > 59)
        {
            Warning("Maximum value for minute is 59!");
            HoursBox.Value = 0;
            MinutesBox.Value = 0;
            SecondsBox.Value = 0;
            return;
        }
        if (seconds > 59)
        {
            Warning("Maximum value for second is 59!");
            HoursBox.Value = 0;
            MinutesBox.Value = 0;
            SecondsBox.Value = 0;
            return;
        }
        var duration = new TimeSpan(hours, minutes, seconds);
        var task = new TimerViewModel(title, duration, isNotified, NotificationService, PrefillTask, false);
        
        task.Start();
        Close(task);
    }
}