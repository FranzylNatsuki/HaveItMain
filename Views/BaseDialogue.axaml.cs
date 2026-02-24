using System.Threading.Tasks;
using Avalonia.Controls;

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
}