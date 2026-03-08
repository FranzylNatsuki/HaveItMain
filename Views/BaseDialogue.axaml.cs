using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;

namespace HaveItMain.Views;

public partial class BaseDialog : Window
{
    public BaseDialog()
    {
        InitializeComponent();
    }

    public async Task ShowDialogAsync(Window owner)
    {
        await ShowDialog(owner);
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // Global Escape behavior: Close the dialog
        if (e.Key == Key.Escape)
        {
            Close(false); 
        }
    }
}