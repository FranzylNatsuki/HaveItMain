using System;
using System.Collections.Generic;
using System.Linq;
using HaveItMain.ViewModels;

namespace HaveItMain.Services;

public class STREAK
{
    public DateTime StartDate { get; set; }
    public int DurationDays { get; set; }

    public List<StreakDay> Days { get; set; } = new();

    public StreakStatus Status { get; set; } = StreakStatus.Unactivated;

    public DateTime EndDate => StartDate.AddDays(DurationDays - 1);

    public void GenerateDays()
    {
        Days.Clear();

        for (int i = 0; i < DurationDays; i++)
        {
            Days.Add(new StreakDay
            {
                Date = StartDate.AddDays(i),
                IsCompleted = false
            });
        }
    }

    public void Evaluate()
    {
        var today = DateTime.Today;

        if (today > EndDate)
        {
            if (Days.All(d => d.IsCompleted))
                Status = StreakStatus.Completed;
            else
                Status = StreakStatus.Broken;

            return;
        }

        foreach (var day in Days.Where(d => d.Date < today))
        {
            if (!day.IsCompleted)
            {
                Status = StreakStatus.Broken;
                return;
            }
        }

        var todayEntry = Days.FirstOrDefault(d => d.Date == today);

        if (todayEntry == null)
        {
            Status = StreakStatus.Broken;
            return;
        }

        Status = todayEntry.IsCompleted
            ? StreakStatus.Active
            : StreakStatus.Unactivated;
    }

    public void MarkTodayComplete()
    {
        var todayEntry = Days.FirstOrDefault(d => d.Date == DateTime.Today);

        if (todayEntry == null)
            return;

        todayEntry.IsCompleted = true;

        Evaluate();
    }
}