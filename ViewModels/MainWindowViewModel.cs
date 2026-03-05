using System;
using ReactiveUI;
using System.Reactive.Linq;
using HaveItMain.Services;

namespace HaveItMain.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public AppState State { get; }
    
        private readonly StreakService _streakService;
        private readonly StreakPersistenceService _streakPersistence;
        private readonly PersistenceService _taskPersistence;
        
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
            State = state ?? throw new ArgumentNullException(nameof(state));
            
            _streakPersistence = new StreakPersistenceService();
            _taskPersistence = new PersistenceService();
            _streakService = new StreakService(); 
            
            State.CurrentStreak = _streakPersistence.Load();
            State.StreakStarted = State.CurrentStreak != null;
    
            _taskPersistence.Load(State);

            HookTaskCollection();
            
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
            CurrentViewModel = new Dashboard();
        }

        private void HookTaskCollection()
        {
            foreach (var task in State.Tasks)
                SubscribeToTask(task);
            State.Tasks.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (TaskItemViewModel task in e.NewItems)
                        SubscribeToTask(task);
                }
            };
        }

        private void SubscribeToTask(TaskItemViewModel task)
        {
            task.WhenAnyValue(x => x.IsFinished)
                .Where(done => done) // Only fire when task is checked 'True'
                .Subscribe(_ =>
                {
                    // SAFETY GATE: If the user hasn't opted-in to a streak, 
                    // just save the task and stop here.
                    if (State.CurrentStreak == null)
                    {
                        _taskPersistence.Save(State);
                        return;
                    }
                    
                    _streakService.RegisterTaskCompletion(State);
                    _streakPersistence.Save(State.CurrentStreak);
                    _taskPersistence.Save(State);
                });
        }
        
        public void TriggerAlert() {
            _notificationService.ShowNotification("Hello!", "This is a native Windows toast.");
        }


        public void ShowDashboard() => CurrentViewModel = new Dashboard();
        // public void ShowTimer() => CurrentViewModel = new Timer();
        public void ShowSettings() => CurrentViewModel = new Settings();
        public void ShowAccount() => CurrentViewModel = new AccountSettings();
    }
}