using Godot;
using System;

public partial class EnergyBar : Control
{
    [Export] public float MaxEnergy = 100f;
    [Export] public float DrainPerSecond = 10f;
    [Export] public float LowEnergyThreshold = 25f;
    [Export] public float FlashSpeed = 3f;
    [Export] public float AnimationSpeed = 5f;

    private TextureProgressBar _bar;
    private float _currentEnergy;
    private float _targetEnergy;
    private bool _isFlashing = false;
    private Tween _flashTween;
    private Tween _barTween;

    public override void _Ready()
    {
        _bar = GetNode<TextureProgressBar>("TextureProgressBar");
        _bar.Scale = new Vector2(3.0f, 3.0f);
        _bar.MinValue = 0;
        _bar.MaxValue = MaxEnergy;
        _currentEnergy = MaxEnergy;
        _targetEnergy = MaxEnergy;
        _bar.Value = MaxEnergy;
    }

    public override void _Process(double delta)
    {
        if (_currentEnergy > 0)
            _currentEnergy -= DrainPerSecond * (float)delta;

        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, MaxEnergy);
        
        // Only animate if target changed
        if (!Mathf.IsEqualApprox(_targetEnergy, _currentEnergy))
        {
            _targetEnergy = _currentEnergy;
            AnimateBar();
        }

        float percentage = (_currentEnergy / MaxEnergy) * 100f;
        if (percentage <= LowEnergyThreshold && !_isFlashing)
            StartFlash();
        else if (percentage > LowEnergyThreshold && _isFlashing)
            StopFlash();
    }

    private void AnimateBar()
    {
        // Kill existing tween if running
        _barTween?.Kill();
        
        // Create new tween for smooth animation
        _barTween = CreateTween();
        _barTween.SetTrans(Tween.TransitionType.Linear);
        _barTween.SetEase(Tween.EaseType.Out);
        
        // Animate the bar value smoothly
        _barTween.TweenProperty(_bar, "value", _targetEnergy, 1f / AnimationSpeed);
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