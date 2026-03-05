using System;
using HaveItMain.Services;
using HaveItMain.ViewModels;

public class StreakService
{
    public void StartNewStreak(AppState state, int durationDays)
    {
        var streak = new STREAK()
        {
            StartDate = DateTime.Today,
            DurationDays = durationDays
        };

        streak.GenerateDays();
        streak.Evaluate();

        state.CurrentStreak = streak;
    }

    public void Evaluate(AppState state)
    {
        state.CurrentStreak?.Evaluate();
    }

    public void RegisterTaskCompletion(AppState state)
    {
        if (state.CurrentStreak == null)
            return;

        if (state.CurrentStreak.Status == StreakStatus.Broken ||
            state.CurrentStreak.Status == StreakStatus.Completed)
            return;

        state.CurrentStreak.MarkTodayComplete();
    }
}