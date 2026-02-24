using Avalonia.Controls;

namespace HaveItMain.Views;

public partial class SimpleMessageDialog : BaseDialog
{
    public SimpleMessageDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}