using Godot;
using System;

public partial class EnergyBar : Control
{
    [Export] public float MaxEnergy = 100f;
    [Export] public float DrainPerSecond = 10f;
    [Export] public float LowEnergyThreshold = 25f; //hvor energien anses som lav 
    [Export] public float FlashSpeed = 3f;
    [Export] public float AnimationSpeed = 5f;

    private TextureProgressBar _bar;
    private float _currentEnergy;
    private float _targetEnergy;
    private bool _isFlashing = false;
    private Tween _flashTween; // en tween er en som objekt bruges til at animere sændringer 
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
        if (_currentEnergy > 0) //energi falder over tid
            _currentEnergy -= DrainPerSecond * (float)delta;

        _currentEnergy = Mathf.Clamp(_currentEnergy, 0, MaxEnergy); //sikrer at energi aldrig går mod nul eller over max.
        
        if (!Mathf.IsEqualApprox(_targetEnergy, _currentEnergy))//hvis værdien har ændret sig så start animation.
        {
            _targetEnergy = _currentEnergy;
            AnimateBar();
        }

        float percentage = (_currentEnergy / MaxEnergy) * 100f;
        if (percentage <= LowEnergyThreshold && !_isFlashing) //Start blink
            StartFlash();
        else if (percentage > LowEnergyThreshold && _isFlashing)
            StopFlash();
    }

    private void AnimateBar()
    {
        _barTween?.Kill(); //stop gammel animation
        
        _barTween = CreateTween();//lav ny tween
        _barTween.SetTrans(Tween.TransitionType.Linear); //lineær animation.
        _barTween.SetEase(Tween.EaseType.Out); //starter hurtigt → slutter langsomt.
        
        _barTween.TweenProperty(_bar, "value", _targetEnergy, 1f / AnimationSpeed);
    }

    private void StartFlash()
    {
        _isFlashing = true;
        _flashTween?.Kill();
        _flashTween = CreateTween().SetLoops();
        _flashTween.TweenProperty(_bar, "modulate", new Color(1, 0.2f, 0.2f), 1f / FlashSpeed);//farve ændring
        _flashTween.TweenProperty(_bar, "modulate", new Color(1, 1, 1), 1f / FlashSpeed); 
    }

    private void StopFlash()
    {
        _isFlashing = false;
        _flashTween?.Kill(); 
        _bar.Modulate = new Color(1, 1, 1);
    }
}