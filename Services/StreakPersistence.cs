using System;
using System.IO;
using System.Text.Json;
using HaveItMain.Services;
using HaveItMain.ViewModels;

public class StreakPersistenceService
{
    private const string FileName = "streak.json";

    public void Save(STREAK? streak)
    {
        if (streak == null) return;

        try
        {
            var json = JsonSerializer.Serialize(streak, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving streak: {ex.Message}");
        }
    }

    public STREAK? Load()
    {
        try
        {
            if (!File.Exists(FileName)) return null;

            var json = File.ReadAllText(FileName);
            var streak = JsonSerializer.Deserialize<STREAK>(json);
            return streak;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading streak: {ex.Message}");
            return null;
        }
    }
}