using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using HaveItMain.ViewModels;

namespace HaveItMain.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        this.Loaded += (s, e) => SyncPickerToCurrentFont();
    }
    
    private void SyncPickerToCurrentFont()
    {
        if (Application.Current != null)
        {
            // 1. Correct way to get a resource in modern Avalonia
            // We pass 'null' for the ThemeVariant to get the default/active one
            if (Application.Current.TryGetResource("FontSizeNormal", null, out var resourceValue))
            {
                // 2. Safely convert the object to a double
                if (resourceValue is double actualSize)
                {
                    for (int i = 0; i < FontSizePicker.ItemCount; i++)
                    {
                        if (FontSizePicker.Items[i] is ComboBoxItem item)
                        {
                            string? val = item.Tag?.ToString() ?? item.Content?.ToString();
                            if (double.TryParse(val, out double itemSize) && Math.Abs(itemSize - actualSize) < 0.1)
                            {
                                FontSizePicker.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void IncreaseFont(object? sender, RoutedEventArgs e)
    {
        if (FontSizePicker.SelectedIndex < FontSizePicker.ItemCount - 1)
        {
            FontSizePicker.SelectedIndex++;
            // Manually trigger the update since programatic changes 
            // sometimes skip the UI event
            ApplyCurrentSelection();
        }
    }

    private void DecreaseFont(object? sender, RoutedEventArgs e)
    {
        if (FontSizePicker.SelectedIndex > 0)
        {
            FontSizePicker.SelectedIndex--;
            ApplyCurrentSelection();
        }
    }
    
    private void ApplyCurrentSelection()
    {
        if (FontSizePicker.SelectedItem is ComboBoxItem item)
        {
            string? val = item.Tag?.ToString() ?? item.Content?.ToString();
            if (double.TryParse(val, out double size))
            {
                UpdateGlobalFontSize(size);
            
                // OPTIONAL: Keep the 'Global' state in sync if you have a service
                // App.ServiceState.CurrentFontSize = size; 
            }
        }
    }

    private void UpdateGlobalFontSize(double newSize)
    {
        if (Application.Current != null)
        {
            // Explicitly target the Application-level resource dictionary
            Application.Current.Resources["FontSizeNormal"] = newSize;
            Application.Current.Resources["FontSizeLarge"] = newSize * 2.25;
            
        }
    }
    private void FontSizePicker_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // 1. Safety check: Don't run this while the view is still "waking up"
        // or if the selection was cleared (null)
        if (!this.IsInitialized || FontSizePicker.SelectedItem == null) return; 

        // 2. Use the helper method we created for the buttons
        // This keeps your logic in one place (DRY - Don't Repeat Yourself)
        ApplyCurrentSelection();
    }

    private void DyslexicToggle(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton tb)
        {
            // Use the global App resources
            if (Application.Current != null)
            {
                var res = Application.Current.Resources;
            
                if (tb.IsChecked == true)
                    res["AppFont"] = res["DyslexicFont"];
                else
                    res["AppFont"] = res["NunitoFont"];
            }
        }
    }
    
    
}