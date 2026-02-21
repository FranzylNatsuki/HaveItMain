using System;
using ReactiveUI;
using System.Reactive.Linq;

namespace HaveItMain.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
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

        public MainWindowViewModel()
        {
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
        }

        public void ShowDashboard() => CurrentViewModel = new Dashboard();
        public void ShowTimer() => CurrentViewModel = new Timer();
        public void ShowStreak() => CurrentViewModel = new Streak();
        public void ShowSettings() => CurrentViewModel = new Settings();
        public void ShowAccount() => CurrentViewModel = new AccountSettings();
    }
}