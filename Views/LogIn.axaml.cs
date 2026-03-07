using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HaveItMain.Services;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();
    }

    private void Login_Click(object? sender, RoutedEventArgs e)
    {
        string inputUser = TxtUser.Text ?? "";
        string inputPass = TxtPass.Text ?? "";

        // IMPORTANT: Search the LIST, not the single object
        var foundAccount = App.ServiceState.AllAccounts
            .FirstOrDefault(a => a.Username == inputUser && a.Password == inputPass);

        if (foundAccount != null)
        {
            // 1. Sync the session to the found user
            App.ServiceState.UserAccount = foundAccount;
            App.ServiceState.IsLoggedIn = true;
        
            // 2. Persist the session (using your service)
            new SessionService().SaveSession(true, inputUser);

            // 3. Open Main Window with DataContext
            var mainWin = new MainWindow { DataContext = new MainWindowViewModel(App.ServiceState) };
            mainWin.Show();
            this.Close();
        }
        else
        {
            ErrorMessage.Text = "Invalid username or password.";
            ErrorMessage.IsVisible = true;
        }
    }

    private void Back_Click(object? sender, RoutedEventArgs e)
    {
        var landing = new Landing { DataContext = this.DataContext };
        landing.Show();
        this.Close();
    }

    private void Reveal(object? sender, RoutedEventArgs e)
    {
        if (TxtPass.PasswordChar == '*')
        {
            // Show the password
            TxtPass.PasswordChar = '\0'; 
        
            // Update UI Icons
            eyeopen.IsVisible = true;
            eyeclosed.IsVisible = false;
        }
        else
        {
            // Hide the password
            TxtPass.PasswordChar = '*';
        
            // Update UI Icons
            eyeopen.IsVisible = false;
            eyeclosed.IsVisible = true;
        }
    }
}