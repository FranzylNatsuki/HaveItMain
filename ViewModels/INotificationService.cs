using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;

namespace HaveItMain.ViewModels;

public interface INotificationService {
    public void ShowNotification(string title, string message)
    {
        // This is the "Magic" that talks to the Windows Action Center
        new ToastContentBuilder()
            .AddText(title)
            .AddText(message)
            .SetToastDuration(ToastDuration.Short)
            .Show(); 
    }
}