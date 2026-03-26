using Godot;
using System;

public partial class EnergyBar : TextureProgressBar
{
    [Export] public float FlashSpeed = 3f;
    [Export] public float AnimationSpeed = 5f;

    
    private float LowEnergyThreshold = 25.0f;

    private bool IsFlashing = false;
    private Tween FlashTween;
    private Tween BarTween;

    public override void _Ready()
    {
        Global global = Global.GetInstance();

        MinValue = 0;
        MaxValue = global.GetState<float>("MaxEnergy");

        Value = global.GetState<float>("MaxEnergy");
    }

    public override void _Process(double delta)
    {
        Global global = Global.GetInstance();

        float currentEnergy = global.GetState<float>("CurrentEnergy");
        float maxEnergy     = global.GetState<float>("MaxEnergy");
        float percentage = currentEnergy / maxEnergy * 100f;

        Value = percentage;

        if (percentage <= LowEnergyThreshold && !IsFlashing)
            StartFlash();
        else if (percentage > LowEnergyThreshold && IsFlashing)
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