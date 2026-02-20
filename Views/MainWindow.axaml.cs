using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace HaveItMain.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Console.WriteLine("GWAPO SI JAV");
    }
}