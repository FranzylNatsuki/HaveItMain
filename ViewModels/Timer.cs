using System.Collections.ObjectModel;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Timer : ViewModelBase, IHasTitle
{
    public string Title => "TIMER";

    public ObservableCollection<TimerViewModel> Timers { get; }

    public Timer(ObservableCollection<TimerViewModel> sharedTimers)
    {
        Timers = sharedTimers; // <-- this is what TimerView binds to
        
        foreach (var t in Timers)
            t.Start(); // starts ticking
    }
}