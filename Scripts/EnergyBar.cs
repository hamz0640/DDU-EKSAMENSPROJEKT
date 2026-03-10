using Godot;
using System;

public partial class EnergyBar : TextureProgressBar
{
    [Export] public float MaxEnergy = 100f;
    [Export] public float LowEnergyThreshold = 25f;
    [Export] public float FlashSpeed = 3f;
    [Export] public float AnimationSpeed = 5f;

    private float _currentEnergy;
    private float _targetEnergy;

    private bool _isFlashing = false;
    private Tween _flashTween;
    private Tween _barTween;

    public override void _Ready()
    {
        MinValue = 0;
        MaxValue = MaxEnergy;

        _currentEnergy = MaxEnergy;
        _targetEnergy = MaxEnergy;
        Value = MaxEnergy;
    }

    public override void _Process(double delta)
    {
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

    public void Drain(float amount)
    {
        _currentEnergy -= amount;
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, MaxEnergy);
    }

    public void DrainPerSecond(float rate, float delta)
    {
        Drain(rate * (float)delta);
    }

    private void AnimateBar()
    {
        _barTween?.Kill();
        _barTween = CreateTween();
        _barTween.SetTrans(Tween.TransitionType.Linear);
        _barTween.SetEase(Tween.EaseType.Out);
        _barTween.TweenProperty(this, "value", _targetEnergy, 1f / AnimationSpeed);
    }

    private void StartFlash()
    {
        _isFlashing = true;
        _flashTween?.Kill();
        _flashTween = CreateTween().SetLoops();
        _flashTween.TweenProperty(this, "modulate", new Color(1, 0.2f, 0.2f), 1f / FlashSpeed);
        _flashTween.TweenProperty(this, "modulate", new Color(1, 1, 1), 1f / FlashSpeed);
    }

    private void StopFlash()
    {
        _isFlashing = false;
        _flashTween?.Kill();
        Modulate = new Color(1, 1, 1);
    }

    public bool HasEnergy()
    {
        return _currentEnergy > 0;
    }

    public void AddEnergy(float amount)
    {
        _currentEnergy += amount;
        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, MaxEnergy);
    }
}