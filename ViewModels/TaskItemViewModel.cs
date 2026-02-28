using System;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class TaskItemViewModel : ViewModelBase
{
    private string _title;
    private DateTime _date;
    private string _urgency;
    private bool _isFinished;

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public DateTime Date
    {
        get => _date;
        set => this.RaiseAndSetIfChanged(ref _date, value);
    }

    public string Urgency
    {
        get => _urgency;
        set => this.RaiseAndSetIfChanged(ref _urgency, value);
    }

    public DateTime date_created { get; }  // keep read-only
    public bool IsFinished
    {
        get => _isFinished;
        set => this.RaiseAndSetIfChanged(ref _isFinished, value);
    }

    public TaskItemViewModel(string title, DateTime date, DateTime date_created, string urgency, bool isFinished = false)
    {
        _title = title;
        _date = date;
        _urgency = urgency;
        _isFinished = isFinished;
        this.date_created = date_created;
    }
    
    public override string ToString()
    {
        return $"{Title} | Due: {Date:yyyy-MM-dd} | Created: {date_created:yyyy-MM-dd} | Urgency: {Urgency} | Done: {IsFinished}";
    }
}