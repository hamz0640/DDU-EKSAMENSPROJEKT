using Godot;
using System;

public partial class EnergyBar : Control
{
	[Export] public float MaxEnergy = 100f;
    [Export] public float DrainPerSecond = 10f;    
    [Export] public float SmoothSpeed = 5f;
    [Export] public float LowEnergyThreshold = 25f;
    [Export] public float FlashSpeed = 3f;

    private TextureProgressBar _bar;
    private float _currentEnergy;
    private float _displayedEnergy;
    private bool _isFlashing = false;
    private Tween _flashTween;

    public override void _Ready()
    {
        _bar = GetNode<TextureProgressBar>("TextureProgressBar");
        _bar.MinValue = 0;
        _bar.MaxValue = MaxEnergy;

        _currentEnergy = MaxEnergy;
        _displayedEnergy = MaxEnergy;
        _bar.Value = MaxEnergy;
    }

    public override void _Process(double delta)
    {
        // energi over tid
        if (_currentEnergy > 0)
            _currentEnergy -= DrainPerSecond * (float)delta;
        
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, MaxEnergy);

        //  animation
        _displayedEnergy = Mathf.Lerp(_displayedEnergy, _currentEnergy, SmoothSpeed * (float)delta);
        _bar.Value = _displayedEnergy;

        //  flash
        float percentage = (_currentEnergy / MaxEnergy) * 100f;
        if (percentage <= LowEnergyThreshold && !_isFlashing)
            StartFlash();
        else if (percentage > LowEnergyThreshold && _isFlashing)
            StopFlash();
    }

    private void StartFlash()
    {
        _isFlashing = true;
        _flashTween?.Kill();
        _flashTween = CreateTween().SetLoops();
        _flashTween.TweenProperty(_bar, "modulate", new Color(1, 0.2f, 0.2f), 1f / FlashSpeed);
        _flashTween.TweenProperty(_bar, "modulate", new Color(1, 1, 1), 1f / FlashSpeed);
    }

    private void StopFlash()
    {
        _isFlashing = false;
        _flashTween?.Kill();
        _bar.Modulate = new Color(1, 1, 1);
    }
}

