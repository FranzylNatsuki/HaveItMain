using System;
using ReactiveUI;

namespace HaveItMain.ViewModels;

public class StreakDay : ViewModelBase
{
    public DateTime Date { get; set; }
    public bool IsCompleted { get; set; }
}