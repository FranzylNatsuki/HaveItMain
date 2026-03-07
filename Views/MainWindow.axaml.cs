using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using HaveItMain.Services;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;


public partial class MainWindow : Window
{
    
    private bool _sidebarCollapsed = false;
    
    public MainWindow()
    {
        InitializeComponent();
        // DataContext = new MainWindowViewModel();
    }

    public void initialize_core_settings()
    {
        LEFTMENUBAR.IsVisible = true;
    }
    
    private void ComboBoxClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.ContextFlyout is MenuFlyout flyout)
        {
            flyout.ShowAt(btn);
        }
    }
    
    private void Demo_VideoClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var videoWindow = new Video();
        videoWindow.Show(); // or ShowDialog(this) if modal
    }
    
    private void Dashboard_Click(object? sender, RoutedEventArgs e)
    {
        // cast DataContext to your MainWindowViewModel
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard(vm.State); // show dashboard
        }
    }

    private void Timer_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            var timerVM = new Timer(App.ServiceState.Timers);
            vm.CurrentViewModel = timerVM;
        }
    }
    private void Streak_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Streak(vm.State);
        }
    }

    private void Settings_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Settings(vm.State);
        }
    }

    private void AccountSettings_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new AccountSettings(vm.State);
        }
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.CurrentViewModel = new Dashboard(vm.State);
        }
    }

    private void LEFTMENUBAR_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var sidebarColumn = Windowgrid.ColumnDefinitions[0];

        if (_sidebarCollapsed)
        {
            LeftMenuContent.IsVisible = true;
            sidebarColumn.Width = new GridLength(72); // expanded width
        }
        else
        {
            LeftMenuContent.IsVisible = false;
            sidebarColumn.Width = new GridLength(10); // collapsed width
        }

        _sidebarCollapsed = !_sidebarCollapsed;
    }

    private async void NEW_ROUTINE(object? sender, RoutedEventArgs e)
    {
        var dialog = new AddTaskMessage();
        var result = await dialog.ShowDialog<TaskItemViewModel?>(this);

        if (result != null)
        {
            if (DataContext is MainWindowViewModel mainVM)
            {
                if (mainVM.CurrentViewModel is Dashboard dashboardVM)
                {
                    dashboardVM.AddTask(result);
                }
                else
                {
                    mainVM.State.Tasks.Add(result);
                }
            }
        }
    }
    
    private async void NEW_TIMER(object? sender, RoutedEventArgs e)
    {
        var dialog = new AddTimerMessage();
        var result = await dialog.ShowDialog<TimerViewModel?>(this);

        if (result != null)
        {
            if (DataContext is MainWindowViewModel mainVM)
            {
                if (mainVM.CurrentViewModel is Dashboard dashboardVM)
                {
                    dashboardVM.AddTimer(result);
                }
                else
                {
                    mainVM.State.Timers.Add(result);
                }
            }
        }
    }

    private void Landing(object? sender, RoutedEventArgs e)
    {
        var landing = new AboutNoLogin_();
        landing.Show();
    }
    
    private async void Search_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is AutoCompleteBox box && box.SelectedItem is string selected)
        {
            // 1. Force the UI to close the dropdown immediately
            box.IsDropDownOpen = false;
        
            // 2. Clear the selection so it doesn't re-trigger
            box.SelectedItem = null;
            box.Text = string.Empty;

            // 3. Kill focus so the cursor disappears
            this.Focus();
            
            await System.Threading.Tasks.Task.Delay(100);

            // 5. Run your navigation
            ExecuteNavigation(selected);
        }
    }
    private void Search_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (sender is AutoCompleteBox box)
            {
                string query = box.SelectedItem?.ToString() ?? box.Text;

                if (string.IsNullOrWhiteSpace(query)) return;

                // 1. Run your navigation/dialog logic
                ExecuteNavigation(query);
            
                // 2. Clear the text
                box.Text = string.Empty;
                box.SelectedItem = null;

                // 3. THE HCI FIX: Kill focus on the search bar
                // Focusing the Window itself forces the TextBox to release its "Active" state
                this.Focus(); 
            
                // Optional: If you want to be extra sure the cursor disappears:
                e.Handled = true; 
            }
        }
    }

// Helper to keep your code DRY (Don't Repeat Yourself)
    private void ExecuteNavigation(string query)
    {
        // Make it case-insensitive for better UX
// Make it case-insensitive for better UX
        // Everything in the 'case' must be LOWERCASE now
        switch (query.ToLower().Trim())
        {
            case "dashboard":
            case "streak":
                Dashboard_Click(null, new RoutedEventArgs());
                break;

            case "timer":
                Timer_Click(null, new RoutedEventArgs());
                break;

            case "add timer":
            case "new timer":
                // Make sure NEW_TIMER is defined in your MainWindow.axaml.cs
                NEW_TIMER(null, new RoutedEventArgs());
                break;

            case "settings":
                Settings_Click(null, new RoutedEventArgs());
                break;

            case "account":
            case "account settings":
                AccountSettings_Click(null, new RoutedEventArgs());
                break;

            case "task":
            case "add task":
            case "new task":
            case "new routine": // Adding this since your method is named NEW_ROUTINE
                NEW_ROUTINE(null, new RoutedEventArgs());
                break;
        }
    }
    
    private void MainContainer_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("Pressed");
        // This tells the Window to grab focus, which forces the 
        // AutoCompleteBox to lose its focus and cursor.
        this.Focus();
    
        if (GlobalSearch != null)
        {
            GlobalSearch.IsDropDownOpen = false;
        }

        // Force focus to the Grid, which pulls it out of the AutoCompleteBox
        Windowgrid.Focus();
    }
    
    private void SignOut_Click(object? sender, RoutedEventArgs e)
    {
        // 1. Wipe the persistent session file
        var sessionService = new SessionService();
        sessionService.ClearSession();

        // 2. Reset the AppState (HCI: ensures no data leaks to the next user)
        App.ServiceState.IsLoggedIn = false;
        // Optional: Reset other things like App.ServiceState.Tasks.Clear();

        // 3. Open the Landing Window
        var landing = new Landing
        {
            // Re-pass the ServiceState so the new login can access the account list
            DataContext = new MainWindowViewModel(App.ServiceState)
        };
        landing.Show();

        // 4. Close the current Main Window
        this.Close();
    }
    
    private async void Export_Click(object? sender, RoutedEventArgs e)
    {
        // 1. Open the "Save As" Dialog
        var storage = this.StorageProvider;
        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Export Your Tasks",
            SuggestedFileName = "my_tasks.json",
            DefaultExtension = "json",
            FileTypeChoices = new[] { new FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } } }
        });

        // 2. If they didn't cancel, tell AppState to copy the file there
        if (file != null)
        {
            await App.ServiceState.ExportTasks(file.Path.LocalPath);
        }
    }

    private async void Import_Click(object? sender, RoutedEventArgs e)
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return;

        // 1. Create the filter for JSON
        var jsonFilter = new FilePickerFileType("JSON Files")
        {
            Patterns = new[] { "*.json" }
        };

        // 2. Configure the picker - AllowMultiple is False by default
        var result = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select tasks.json to Import",
            AllowMultiple = false, 
            FileTypeFilter= new List<FilePickerFileType> { jsonFilter }
        });

        // 3. Only grab the first one (Index 0)
        if (result != null && result.Count > 0)
        {
            // Path.LocalPath is the cleanest way to get the string for File.Copy
            string selectedFilePath = result[0].Path.LocalPath;
            await App.ServiceState.ImportTasks(selectedFilePath);
        }
    }
}
