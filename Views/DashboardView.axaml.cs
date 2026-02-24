using System;
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
}