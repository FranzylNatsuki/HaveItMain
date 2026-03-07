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
        var loginWin = new Login();
        loginWin.Show();
        this.Close();
    }
    
    private void DemoVideo(object? sender, RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show();
    }

    private void SignUp(object? sender, RoutedEventArgs e)
    {
        var signupWin = new SignUp();
        signupWin.Show();
        this.Close();
    }
}