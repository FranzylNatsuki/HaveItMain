using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace HaveItMain.Views;

public partial class SimpleMessageDialog : BaseDialog
{
    public SimpleMessageDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
        this.Opened += (s, e) => this.Focus();
    }

    private void Ok_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Ok_Click(null, new RoutedEventArgs());
            e.Handled = true;
        }
        else
        {
            base.OnKeyDown(e); // Let Esc be handled by BaseDialog
        }
    }
}