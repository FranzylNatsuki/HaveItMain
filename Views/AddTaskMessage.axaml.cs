using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class AddTaskMessage : Window
{
    private string _urgency = "";
    public AddTaskMessage()
    {
        InitializeComponent();
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

        // 📅 Get date
        var date = DueDatePicker.SelectedDate?.DateTime ?? DateTime.Now;

        // ⏰ Get time
        var time = TimePicker.SelectedTime ?? DateTime.Now.TimeOfDay;

        // 🧩 Combine them into a full DateTime
        var due = date.Date + time;

        var task = new TaskItemViewModel(
            title,
            due,
            DateTime.Now,
            _urgency,
            false
        );

        Console.WriteLine("Test: " + task.ToString());

        Close(task); // return object to caller
    }

    private void Not_Urgent(object? sender, RoutedEventArgs e)
    {
        _urgency = "Not Urgent";
        NUrgent.IsChecked = true;       // The one you clicked
        NPending.IsChecked = false;     // Others reset
        UrgentZ.IsChecked = false;
    }
    
    private void Pending(object? sender, RoutedEventArgs e)
    {
        _urgency = "Pending";
        NUrgent.IsChecked = false;       // The one you clicked
        NPending.IsChecked = true;     // Others reset
        UrgentZ.IsChecked = false;
    }
    
    private void Urgent(object? sender, RoutedEventArgs e)
    {
        _urgency = "Urgent";
        NUrgent.IsChecked = false;       // The one you clicked
        NPending.IsChecked = false;     // Others reset
        UrgentZ.IsChecked = true;
    }
}