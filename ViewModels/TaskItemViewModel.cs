using System;

namespace HaveItMain.ViewModels;

public class TaskItemViewModel : ViewModelBase
{
    public string Title { get; }
    public DateTime Date { get; }
    public string Urgency { get; }
    public DateTime date_created { get; }
    public bool isFinished { get; }

    public TaskItemViewModel(string title, DateTime date, DateTime date_created, string urgency, bool isFinished = false)
    {
        Title = title;
        Date =  date;
        this.date_created = date_created;
        Urgency = urgency;
        this.isFinished = isFinished;
    }
    
    public override string ToString()
    {
        return $"{Title} | Due: {Date:yyyy-MM-dd} | Created: {date_created:yyyy-MM-dd} | Urgency: {Urgency} | Done: {isFinished}";
    }
}