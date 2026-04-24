using Godot;

public partial class WaveAnnouncement : Control
{
    private Panel Banner;
    private Panel LeftBorder;
    private Panel RightBorder;
    private Label SubLabel;
    private Panel Divider;
    private Label WaveLabel;
    private Tween Tween;

    public override void _Ready()
    {
        Banner      = GetNode<Panel>("Banner");
        LeftBorder  = GetNode<Panel>("Banner/LeftBorder");
        RightBorder = GetNode<Panel>("Banner/RightBorder");
        SubLabel    = GetNode<Label>("Banner/SubLabel");
        Divider     = GetNode<Panel>("Banner/Divider");
        WaveLabel   = GetNode<Label>("Banner/WaveLabel");

        Visible = false;

        WaveManager.GetInstance().WaveStarted += OnWaveStarted;
    }

    private void OnWaveStarted() => ShowWave();

    private void ShowWave()
    {
        Visible = true;

        Banner.Modulate      = new Color(1, 1, 1, 0);
        LeftBorder.Scale     = new Vector2(0f, 1f);
        RightBorder.Scale    = new Vector2(0f, 1f);
        SubLabel.Modulate    = new Color(1, 1, 1, 0);
        Divider.Modulate     = new Color(1, 1, 1, 0);
        WaveLabel.Modulate   = new Color(1, 1, 1, 0);
        WaveLabel.Scale      = new Vector2(0.8f, 0.8f);

        Tween?.Kill();
        Tween = CreateTween();

        // Background fades in
        Tween.TweenProperty(Banner, "modulate:a", 1.0f, 0.15f);

        // Red borders slice in 
        Tween.Parallel().TweenProperty(LeftBorder,  "scale:x", 1.0f, 0.2f)
              .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quart);
        Tween.Parallel().TweenProperty(RightBorder, "scale:x", 1.0f, 0.2f)
              .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Quart);

        // Sub label fades 
        Tween.TweenProperty(SubLabel, "modulate:a", 1.0f, 0.15f);

        // Divider wipes 
        Tween.TweenProperty(Divider, "modulate:a", 1.0f, 0.1f);

        //  Wave label pops 
        Tween.Parallel().TweenProperty(WaveLabel, "modulate:a", 1.0f, 0.2f);
        Tween.Parallel().TweenProperty(WaveLabel, "scale", new Vector2(1.1f, 1.1f), 0.2f).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

        // back to normal
        Tween.TweenProperty(WaveLabel, "scale", Vector2.One, 0.1f);

        // Hold
        Tween.TweenInterval(4.0f);

        // Everything fades tf out
        Tween.TweenProperty(Banner,   "modulate:a", 0.0f, 0.35f);
        Tween.Parallel().TweenProperty(SubLabel,  "modulate:a", 0.0f, 0.35f);
        Tween.Parallel().TweenProperty(Divider,   "modulate:a", 0.0f, 0.35f);
        Tween.Parallel().TweenProperty(WaveLabel, "modulate:a", 0.0f, 0.35f);

        Tween.Finished += () => Visible = false;
    }
}