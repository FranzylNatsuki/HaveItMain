using System;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class Dashboard : ViewModelBase, IHasTitle
{
    public ObservableCollection<TaskItemViewModel> Tasks { get; } = new();
    public ReactiveCommand<Unit, Unit> AddTaskCommand { get; }

    public string Title => "DASHBOARD";

    public Dashboard()
    {
        
    }

    public void AddTask(TaskItemViewModel task)
    {
        Tasks.Add(task);
    }
    public void RemoveTask(TaskItemViewModel task)
    {
        Tasks.Remove(task);
    }
}