using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class FocusMode : Window
{
    private TimerViewModel Timer => (TimerViewModel)DataContext!;
    
    public FocusMode(TimerViewModel timer)
    {
        InitializeComponent();
        DataContext = timer; // bind to the single TimerViewModel
    }
    
    
    private void DeleteTimer(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn &&
            btn.DataContext is TimerViewModel timer)
        {
            timer.Stop();                  // stop countdown
            App.ServiceState.Timers.Remove(timer); // remove from the shared collection
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

    private void CloseButton(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}