using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Threading;

namespace BreathingApp;

public class BreathingViewModel : INotifyPropertyChanged
{
    private DispatcherTimer? _timer;
    private double _progress = 0;          // 0..4 (full loop)
    private double _ballX, _ballY;
    private string _phaseText = "Inhale";
    private bool _isRunning = false;

    // Square geometry
    private const double SquareLeft = 50;
    private const double SquareTop = 50;
    private const double SideLength = 200;
    private const double HalfBall = 10;

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

    public string PhaseText
    {
        get => _phaseText;
        private set { _phaseText = value; OnPropertyChanged(); }
    }

    public string ButtonText => _isRunning ? "Stop" : "Start";

    public void ToggleBreathing()
    {
        if (_isRunning)
            StopBreathing();
        else
            StartBreathing(secondsPerSide: 4.0);
    }

    private void StartBreathing(double secondsPerSide = 4.0)
    {
        if (_isRunning) return;

        // Reset to the very beginning
        _progress = 0;
        UpdateBallPosition();
        UpdatePhaseText();

        var interval = TimeSpan.FromMilliseconds(50);
        _timer = new DispatcherTimer { Interval = interval };
        _timer.Tick += (s, e) =>
        {
            double increment = interval.TotalSeconds / secondsPerSide;
            _progress += increment;
            if (_progress >= 4.0) _progress = 0;

            UpdateBallPosition();
            UpdatePhaseText();
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
        OnPropertyChanged(nameof(ButtonText));
        // Do NOT reset progress – so ball stays where it stopped.
        // But next Start will reset it anyway.
    }

    private void UpdateBallPosition()
    {
        double side = Math.Floor(_progress);
        double t = _progress - side;

        switch (side)
        {
            case 0:
                BallX = SquareLeft + t * SideLength - HalfBall;
                BallY = SquareTop - HalfBall;
                break;
            case 1:
                BallX = SquareLeft + SideLength - HalfBall;
                BallY = SquareTop + t * SideLength - HalfBall;
                break;
            case 2:
                BallX = SquareLeft + (1 - t) * SideLength - HalfBall;
                BallY = SquareTop + SideLength - HalfBall;
                break;
            case 3:
                BallX = SquareLeft - HalfBall;
                BallY = SquareTop + (1 - t) * SideLength - HalfBall;
                break;
        }
    }

    private void UpdatePhaseText()
    {
        if (_progress < 1) PhaseText = "Inhale";
        else if (_progress < 2) PhaseText = "Hold";
        else if (_progress < 3) PhaseText = "Exhale";
        else PhaseText = "Hold";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}