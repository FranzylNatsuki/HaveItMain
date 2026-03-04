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
        var currentViewModel = this.DataContext;

        // 2. Create the new window
        var mainWindow = new MainWindow
        {
            DataContext = currentViewModel // <--- THIS IS THE KEY
        };

        // 3. Show the new window and close the landing
        mainWindow.Show();
        this.Close();
    }


    private void DemoVideo(object? sender, RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show();
    }
}