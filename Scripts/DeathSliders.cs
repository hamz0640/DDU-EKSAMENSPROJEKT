using Godot;
using System;
using System.Diagnostics;

public partial class DeathSliders : Control 
{
    [Export] private ColorRect TopDeathSlider;
    [Export] private ColorRect BottomDeathSlider;
    [Export] private Label DeathMessage;

    private bool IsDead = false;

    private void OnQuitPressed()
    {
        string exePath = OS.GetExecutablePath();

        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true
        });

        GetTree().Quit();
    }

    public override void _Process(double delta)
    {
        Global global = Global.GetInstance();

        // FIXED condition (important!)
        if ((global.GetState<float>("CurrentEnergy") <= 0.0f ||
             global.GetState<float>("CurrentShipHealth") <= 0.0f) 
             && IsDead == false)
        {
            IsDead = true;

            Tween tween = GetTree().CreateTween();

            TopDeathSlider.Show();
            BottomDeathSlider.Show();

            tween.SetParallel(true);
            tween.TweenProperty(TopDeathSlider, "position", new Vector2(0, 0), 0.5);
            tween.TweenProperty(BottomDeathSlider, "position", new Vector2(0, 540), 0.5);

            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            tween.Finished += OnSlidersFinished;
        }
    }

    private void OnSlidersFinished()
    {
        DeathMessage.Show();

        DeathMessage.Modulate = new Color(1, 1, 1, 0);
        DeathMessage.Scale = new Vector2(0.5f, 0.5f);

        DeathMessage.PivotOffset = DeathMessage.Size / 2;

        Tween textTween = GetTree().CreateTween();

        // Fade + scale
        textTween.SetParallel(true);
        textTween.TweenProperty(DeathMessage, "modulate:a", 1.0f, 0.6);
        textTween.TweenProperty(DeathMessage, "scale", new Vector2(1, 1), 0.6);

        textTween.SetTrans(Tween.TransitionType.Back);
        textTween.SetEase(Tween.EaseType.Out);

        textTween.Finished += () =>
        {
            Vector2 originalPos = DeathMessage.Position;

            Tween shakeTween = GetTree().CreateTween();

            shakeTween.TweenProperty(DeathMessage, "position", originalPos + new Vector2(10, 0), 0.05);
            shakeTween.TweenProperty(DeathMessage, "position", originalPos + new Vector2(-10, 0), 0.05);
            shakeTween.TweenProperty(DeathMessage, "position", originalPos, 0.05);

            shakeTween.Finished += () =>
            {
                // Wait before restarting game
                GetTree().CreateTimer(1.5).Timeout += () =>
                {
                    OnQuitPressed();
                };
            };
        };
    }
}