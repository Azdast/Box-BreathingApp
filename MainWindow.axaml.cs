using Avalonia.Controls;

namespace BreathingApp;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is BreathingViewModel vm)
        {
            vm.ToggleBreathing();
        }
    }
}