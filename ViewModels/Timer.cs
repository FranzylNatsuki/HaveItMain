using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Timer : ViewModelBase, IHasTitle
{
    public string Title => "TIMER";
    
    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public ReactiveCommand<Unit, Unit> AddTimerCommand { get; }
}