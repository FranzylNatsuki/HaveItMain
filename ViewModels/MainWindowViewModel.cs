using System;
using ReactiveUI;
using System.Reactive.Linq;
using HaveItMain.Services;

namespace HaveItMain.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public AppState State { get; }
        private readonly INotificationService _notificationService;
        private ViewModelBase _currentViewModel;
        public Dashboard Dashboard { get; } = new Dashboard();
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
        // Reactive Title
        private string _currentTitle;
        public string CurrentTitle
        {
            get => _currentTitle;
            private set => this.RaiseAndSetIfChanged(ref _currentTitle, value);
        }

        public MainWindowViewModel(AppState state)
        {
            State = state;
            CurrentViewModel = new Dashboard();

            // Explicitly cast to IObservable<ViewModelBase> to fix CS1660
            IObservable<ViewModelBase> obs = this.WhenAnyValue(x => x.CurrentViewModel);
            obs.Subscribe(vm =>
            {
                if (vm is IHasTitle t)
                    CurrentTitle = t.Title;
                else
                    CurrentTitle = "";
            });
            
            State.NotificationService.ShowNotification(
                "Welcome to HaveIt!", 
                "Your productivity journey starts now."
            );
            // Initialize your other properties (like Dashboard or Timers) here
            // CurrentViewModel = new DashboardViewModel();
        }
        
        public void TriggerAlert() {
            _notificationService.ShowNotification("Hello!", "This is a native Windows toast.");
        }


        public void ShowDashboard() => CurrentViewModel = new Dashboard();
        // public void ShowTimer() => CurrentViewModel = new Timer();
        public void ShowStreak() => CurrentViewModel = new Streak();
        public void ShowSettings() => CurrentViewModel = new Settings();
        public void ShowAccount() => CurrentViewModel = new AccountSettings();
    }
}