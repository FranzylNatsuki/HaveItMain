using System;

namespace HaveItMain.ViewModels;

public class TaskItemViewModel : ViewModelBase
{
    public string Title { get; }
    public DateTime Date { get; }
    public string Urgency { get; }
    public DateTime date_created { get; }
    public bool isFinished { get; }

    public TaskItemViewModel(
        string title,
        DateTime date,
        DateTime date_created,
        string urgency,
        bool isFinished = false)
    {
        Title = title;

        Date = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            date.Hour,
            date.Minute,
            0);

        this.date_created = new DateTime(
            date_created.Year,
            date_created.Month,
            date_created.Day,
            date_created.Hour,
            date_created.Minute,
            0);

        Urgency = urgency;
        this.isFinished = isFinished;
    }
    
    public override string ToString()
    {
        return $"{Title} | Due: {Date:yyyy-MM-dd} | Created: {date_created:yyyy-MM-dd} | Urgency: {Urgency} | Done: {isFinished}";
    }
}