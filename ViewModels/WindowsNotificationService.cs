using System.Runtime.Versioning;
using Microsoft.Toolkit.Uwp.Notifications;

namespace HaveItMain.ViewModels;

public class WindowsNotificationService : INotificationService
{
    public void ShowNotification(string title, string message) {
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .Show(); 
    }
}