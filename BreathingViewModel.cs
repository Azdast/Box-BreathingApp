using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Avalonia.Threading;

namespace BreathingApp;

public class BreathingViewModel : INotifyPropertyChanged
{
    private DispatcherTimer? _timer;
    private double _progress = 0;
    private double _ballX, _ballY;
    private string _phaseText = "Inhale";
    private string _subText = "BREATHE IN SLOWLY";
    private string _phaseColor = "#b8e4f4";
    private bool _isRunning = false;
    private int _countdown = 4;

    private const double SquareLeft = 50;
    private const double SquareTop = 50;
    private const double SideLength = 200;
    private const double BallRadius = 8;
    private const double GlowRadius = 12;
    private const double HighlightRadius = 3;

    public double BallX
    {
        get => _ballX;
        private set { _ballX = value; OnPropertyChanged(); }
    }

    public double BallY
    {
        get => _ballY;
        private set { _ballY = value; OnPropertyChanged(); }
    }

    public double GlowX => _ballX - (GlowRadius - BallRadius);
    public double GlowY => _ballY - (GlowRadius - BallRadius);
    public double HighlightX => _ballX + (BallRadius - HighlightRadius) - 2;
    public double HighlightY => _ballY + (HighlightRadius);

    public string PhaseText
    {
        get => _phaseText;
        private set { _phaseText = value; OnPropertyChanged(); }
    }

    public string SubText
    {
        get => _subText;
        private set { _subText = value; OnPropertyChanged(); }
    }

    public string PhaseColor
    {
        get => _phaseColor;
        private set { _phaseColor = value; OnPropertyChanged(); }
    }

    public int Countdown
    {
        get => _countdown;
        private set { _countdown = value; OnPropertyChanged(); }
    }

    public string ButtonText => _isRunning ? "STOP" : "START";

    public void ToggleBreathing()
    {
        if (_isRunning) StopBreathing();
        else StartBreathing(secondsPerSide: 4.0);
    }

    private void StartBreathing(double secondsPerSide = 4.0)
    {
        if (_isRunning) return;
        _progress = 0;
        UpdateBallPosition();
        UpdatePhase(secondsPerSide);

        var interval = TimeSpan.FromMilliseconds(50);
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (s, e) =>
        {
            double increment = interval.TotalSeconds / secondsPerSide;
            _progress += increment;
            if (_progress >= 4.0) _progress = 0;
            UpdateBallPosition();
            UpdatePhase(secondsPerSide);
        };
        _timer.Start();
        _isRunning = true;
        OnPropertyChanged(nameof(ButtonText));
    }

    private void StopBreathing()
    {
        _timer?.Stop();
        _timer = null;
        _isRunning = false;
        Countdown = 4;
        OnPropertyChanged(nameof(ButtonText));
    }

    private void UpdateBallPosition()
    {
        double side = Math.Floor(_progress);
        double t = _progress - side;

        switch (side)
        {
            case 0:
                BallX = SquareLeft + t * SideLength - BallRadius;
                BallY = SquareTop - BallRadius;
                break;
            case 1:
                BallX = SquareLeft + SideLength - BallRadius;
                BallY = SquareTop + t * SideLength - BallRadius;
                break;
            case 2:
                BallX = SquareLeft + (1 - t) * SideLength - BallRadius;
                BallY = SquareTop + SideLength - BallRadius;
                break;
            case 3:
                BallX = SquareLeft - BallRadius;
                BallY = SquareTop + (1 - t) * SideLength - BallRadius;
                break;
        }

        OnPropertyChanged(nameof(GlowX));
        OnPropertyChanged(nameof(GlowY));
        OnPropertyChanged(nameof(HighlightX));
        OnPropertyChanged(nameof(HighlightY));
    }

    private void UpdatePhase(double secondsPerSide)
    {
        double t = _progress - Math.Floor(_progress);
        Countdown = Math.Max(1, (int)Math.Ceiling((1 - t) * secondsPerSide));

        int idx = (int)Math.Floor(_progress);
        (PhaseText, SubText, PhaseColor) = idx switch
        {
            0 => ("Inhale", "BREATHE IN SLOWLY",  "#b8e4f4"),
            1 => ("Hold",   "HOLD YOUR BREATH",   "#f4e4b8"),
            2 => ("Exhale", "BREATHE OUT SLOWLY", "#b8f4d4"),
            _ => ("Hold",   "HOLD YOUR BREATH",   "#f4e4b8"),
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}