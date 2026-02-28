using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class AddTaskMessage : Window
{
    private string _urgency = "";
    public TaskItemViewModel? PrefillTask { get; set; }
    

    public AddTaskMessage()
    {
        InitializeComponent();

        this.Opened += (_, _) =>
        {
            if (PrefillTask == null) return;

            TaskTitleBox.Text = PrefillTask.Title;
            DueDatePicker.SelectedDate = PrefillTask.Date;
            TimePicker.SelectedTime = PrefillTask.Date.TimeOfDay;

            switch (PrefillTask.Urgency)
            {
                case "Not Urgent": NUrgent.IsChecked = true; break;
                case "Pending": NPending.IsChecked = true; break;
                case "Urgent": UrgentZ.IsChecked = true; break;
            }
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

    private void Add(object? sender, RoutedEventArgs e)
    {
        var title = TaskTitleBox.Text ?? "Untitled";
        var date = DueDatePicker.SelectedDate?.DateTime ?? DateTime.Now;
        var time = TimePicker.SelectedTime ?? DateTime.Now.TimeOfDay;
        var due = date.Date + time;

        var task = new TaskItemViewModel(
            title,
            due,
            PrefillTask?.date_created ?? DateTime.Now,
            _urgency,
            PrefillTask?.IsFinished ?? false
        );

        Close(task);
    }
    private void UrgencyToggle(object? sender, RoutedEventArgs e)
    {
        var clicked = sender as ToggleButton;
        if (clicked == null) return;

        if (clicked.IsChecked == false)
        {
            _urgency = "";
            return;
        }

        if (clicked == NUrgent)
            _urgency = "Not Urgent";
        else if (clicked == NPending)
            _urgency = "Pending";
        else if (clicked == UrgentZ)
            _urgency = "Urgent";

        foreach (var btn in new[] { NUrgent, NPending, UrgentZ })
        {
            if (btn != clicked)
                btn.IsChecked = false;
        }
    }
}