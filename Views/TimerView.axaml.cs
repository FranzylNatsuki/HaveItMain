// TimerView.xaml.cs

using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class TimerView : UserControl
{
    public TimerView()
    {
        InitializeComponent();
    }

    public void SetViewModel(Timer viewModel)
    {
        DataContext = viewModel;
    }

    private void DeleteTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.DataContext is TimerViewModel timer)
        {
            timer.Stop();                  // stop countdown
            App.State.Timers.Remove(timer); // remove from the shared collection
        }
    }
    
    private void PauseTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button pauseButton)
        {
            pauseButton.IsVisible = false;
            if (pauseButton.Parent is Panel parentPanel)
            {
                var playButton = parentPanel.Children
                    .OfType<Button>()
                    .FirstOrDefault(b => b != pauseButton);
                if (playButton != null)
                    playButton.IsVisible = true;
            }
            
            if (pauseButton.DataContext is TimerViewModel timer)
                timer.Pause();
        }
    }

    private void ResumeTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button playButton)
        {
            playButton.IsVisible = false; // hide the play button

            if (playButton.Parent is Panel parentPanel)
            {
                var pauseButton = parentPanel.Children
                    .OfType<Button>()
                    .FirstOrDefault(b => b != playButton);
                if (pauseButton != null)
                    pauseButton.IsVisible = true;
            }

            // Resume the timer
            if (playButton.DataContext is TimerViewModel timer)
                timer.Resume();
        }
    }

    private void FocusModeEnable(object? sender, RoutedEventArgs e)
    {
    }
}