using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace HaveItMain.Views;

public partial class ConfirmationDialog : BaseDialog
{
    public ConfirmationDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    private void Confirm_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Closes the window and returns 'true'
        Close(true);
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Closes the window and returns 'false'
        Close(false);
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Confirm_Click(null, new RoutedEventArgs());
            e.Handled = true;
        }
        else
        {
            base.OnKeyDown(e);
        }
    }
}