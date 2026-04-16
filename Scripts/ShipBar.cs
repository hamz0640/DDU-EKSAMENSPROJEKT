using Godot;
using System;

public partial class ShipBar : TextureProgressBar
{
    [Export] public float FlashSpeed = 3f;
    [Export] public float LowEnergyThreshold = 25.0f;

    private bool _isFlashing = false;
    private Tween _flashTween;
    private bool IsFlashing = false;
    private Tween FlashTween;
    private Tween BarTween;
    private Global _global;

    public override void _Ready()
    {
        _global = Global.GetInstance();

        float maxHealth = _global.GetState<float>("ShipHealth");

        MinValue = 0;
        MaxValue = maxHealth;
        Value = maxHealth;
    }

    public override void _Process(double delta)
    {
        if (_global == null)
            return;

        float currentHealth = _global.GetState<float>("CurrentShipHealth");
        float maxHealth     = _global.GetState<float>("ShipHealth");

        if (maxHealth <= 0)
            return;

        Value = currentHealth;

        float percentage = (currentHealth / maxHealth) * 100f;

        if (percentage <= LowEnergyThreshold && !_isFlashing)
            StartFlash();
        else if (percentage > LowEnergyThreshold && _isFlashing)
            StopFlash();
    }
    private void StartFlash()
    {
        IsFlashing = true;
        FlashTween?.Kill();
        FlashTween = CreateTween().SetLoops();
        FlashTween.TweenProperty(this, "modulate", new Color(1, 0.2f, 0.2f), 1f / FlashSpeed);
        FlashTween.TweenProperty(this, "modulate", new Color(1, 1, 1), 1f / FlashSpeed);
    }

    private void StopFlash()
    {
        IsFlashing = false;
        FlashTween?.Kill();
        Modulate = new Color(1, 1, 1);
    }

}