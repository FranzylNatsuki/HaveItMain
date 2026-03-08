using System;
using System.Collections.Generic;
using System.Linq;
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
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
        
        public List<string> SearchSuggestions { get; } = new()
        {
            "task","Timer", "Streak", "Settings", "Account Settings", "New Task", "New Timer", "Add Timer", "Add Task", "Account"
        };
        
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
            
            var accountPersistence = new AccountPersistenceService();
            var allAccountsList = accountPersistence.Load();
            
            var loadedAccount = accountPersistence.Load();
            State.AllAccounts = new System.Collections.ObjectModel.ObservableCollection<Account>(allAccountsList);
            
            _streakPersistence = new StreakPersistenceService();
            _taskPersistence = new PersistenceService();
            _streakService = new StreakService(); 
            
            if (State.UserAccount == null || string.IsNullOrEmpty(State.UserAccount.Username))
            {
                State.UserAccount = allAccountsList.FirstOrDefault() ?? new Account { Username = "Guest" };
            }
            
            _taskPersistence.Load(State);
            
            var tasksToRemove = State.Tasks
                .Where(t => t.IsFinished && t.CompletedDate.HasValue && t.CompletedDate < DateTime.Today)
                .ToList();

            foreach (var task in tasksToRemove)
            {
                State.Tasks.Remove(task);
            }
            
            _taskPersistence.Save(State);
            
            State.CurrentStreak = _streakPersistence.Load();
            if (State.CurrentStreak != null)
            {
                State.CurrentStreak.NotificationService = State.NotificationService;
                State.CurrentStreak.Evaluate();
            }
            State.StreakStarted = State.CurrentStreak != null;

            HookTaskCollection();
            
            IObservable<ViewModelBase> obs = this.WhenAnyValue(x => x.CurrentViewModel);
            obs.Subscribe(vm =>
            {
                if (vm is IHasTitle t)
                    CurrentTitle = t.Title;
                else
                    CurrentTitle = "";
            });
            CurrentViewModel = new Dashboard(state);
        }

        private void HookTaskCollection()
        {
            foreach (var task in State.Tasks)
                SubscribeToTask(task);

            State.Tasks.CollectionChanged += (s, e) =>
            {
                // 1. If a NEW task is added, subscribe to its IsFinished property
                if (e.NewItems != null)
                {
                    foreach (TaskItemViewModel task in e.NewItems)
                        SubscribeToTask(task);
            
                    // Save since the list grew
                    _taskPersistence.Save(State);
                }

                // 2. THIS IS THE MISSING PART: Save when a task is REMOVED
                if (e.OldItems != null)
                {
                    _taskPersistence.Save(State);
                }
            };
        }

        private void SubscribeToTask(TaskItemViewModel task)
        {
            task.WhenAnyValue(x => x.IsFinished)
                .Skip(1) // <--- THIS IS CRITICAL. It ignores the first value (from the JSON load)
                .Subscribe(isDone =>
                {
                    if (isDone)
                    {
                        _streakService.RegisterTaskCompletion(State);
                    }
                    // Always save when a change happens
                    _taskPersistence.Save(State);
                    _streakPersistence.Save(State.CurrentStreak);
                });
        }
        
        public void OnTaskToggled(object parameter)
        {
            if (parameter is TaskItemViewModel task)
            {
                // 1. Manually trigger the date logic since the setter is being flaky
                if (task.IsFinished)
                {
                    task.CompletedDate = DateTime.Today;
                    _streakService.RegisterTaskCompletion(State);
                }
                else
                {
                    task.CompletedDate = null;
                }

                // 2. Force the Save immediately
                _taskPersistence.Save(State);
                _streakPersistence.Save(State.CurrentStreak);
        
                System.Diagnostics.Debug.WriteLine($"Method triggered for: {task.Title}. CompletedDate: {task.CompletedDate}");
            }
        }
        
        public void TriggerAlert() {
            _notificationService.ShowNotification("Hello!", "This is a native Windows toast.");
        }


        public void ShowDashboard() => CurrentViewModel = new Dashboard(State);
        // public void ShowTimer() => CurrentViewModel = new Timer();
        public void ShowSettings() => CurrentViewModel = new Settings(State);
        public void ShowAccount() => CurrentViewModel = new AccountSettings(State);
    }
}