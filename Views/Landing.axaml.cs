using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class Landing : Window
{
    public Landing()
    {
        InitializeComponent();
    }

    private void Login(object? sender, RoutedEventArgs e)
    {
        var main = new MainWindow
        {
            DataContext = new MainWindowViewModel(),
        };

        main.Show();

        this.Close(); // closes Landing window
    }


    private void DemoVideo(object? sender, RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show();
    }
}