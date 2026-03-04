using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HaveItMain.Services;
using HaveItMain.ViewModels;
using HaveItMain.Views;

namespace HaveItMain;

public partial class App : Application
{
    public static AppState ServiceState { get; set; } = new();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceState.NotificationService = new WindowsNotificationService();
            
            var viewModel = new MainWindowViewModel(ServiceState);

            desktop.MainWindow = new Landing
            {
                DataContext = viewModel
            };
        }
        base.OnFrameworkInitializationCompleted();
    }
}