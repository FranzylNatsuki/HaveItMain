using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace HaveItMain.Views;

public partial class AboutNoLogin_ : Window
{
    public AboutNoLogin_()
    {
        InitializeComponent();
    }
    
    private void DemoVideo(object? sender, RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show();
    }
}