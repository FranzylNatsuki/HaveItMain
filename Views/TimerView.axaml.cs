// TimerView.xaml.cs
using Avalonia.Controls;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class TimerView : UserControl
{
    public TimerView()
    {
        InitializeComponent();
    }

    public void SetViewModel(Timer viewModel)
    {
        DataContext = viewModel; // <-- THIS is key
    }
}