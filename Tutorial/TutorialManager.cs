using Godot;

public partial class TutorialManager : Control
{
    [Export] public TutorialStep[] Steps { get; set; } = [];

    [Export] public Color OverlayColor { get; set; } = new Color(0, 0, 0, 0.75f);
    [Export] public Color BorderColor { get; set; } = new Color(1, 1, 1, 1f);
    [Export] public float BorderWidth { get; set; } = 3f;

    [Export] public float FadeDuration { get; set; } = 0.4f;
    [Export] public float StepTransitionDuration { get; set; } = 0.2f;

    // Editor-assigned NodePaths
    [Export] public NodePath RootPath { get; set; }
    [Export] public NodePath InfoBoxPath { get; set; }
    [Export] public NodePath TitleLabelPath { get; set; }
    [Export] public NodePath DescLabelPath { get; set; }
    [Export] public NodePath NextButtonPath { get; set; }
    [Export] public NodePath SkipButtonPath { get; set; }
    private Control Root;
    private Panel InfoBox;
    private Label TitleLabel;
    private Label DescLabel;
    private Button NextButton;
    private Button SkipButton;

    private int CurrentStep = 0;

    // Animation variabels
    private float overlayAlpha = 0f;
    private float infoBoxAlpha = 0f;
    private float infoBoxSlideOffset = 0f;
    private Tween activeTween;

    public override void _Ready()
    {
        Global global = Global.GetInstance();
        global.SetState("PlayerCanMove", false);

        Root = GetNode<Control>(RootPath);
        InfoBox = GetNode<Panel>(InfoBoxPath);
        TitleLabel = GetNode<Label>(TitleLabelPath);
        DescLabel = GetNode<Label>(DescLabelPath);
        NextButton = GetNode<Button>(NextButtonPath);
        SkipButton = GetNode<Button>(SkipButtonPath);

        NextButton.Pressed += OnNext;
        SkipButton.Pressed += OnSkip;

        Modulate = new Color(1, 1, 1, 0f);
        InfoBox.Position = new Vector2(InfoBox.Position.X, InfoBox.Position.Y + 80f);

        ApplyStyle();

        if (Steps.Length > 0)
            PlayIntroAnimation(() => ShowStep(0));
        else
            Visible = false;
    }

    private void PlayIntroAnimation(System.Action onDone = null)
    {
        KillTween();
        activeTween = CreateTween().SetParallel();

        
        activeTween.TweenProperty(this, "modulate:a", 1f, FadeDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

     
        float targetY = InfoBox.Position.Y - 80f;
        activeTween.TweenProperty(InfoBox, "position:y", targetY, FadeDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);

        if (onDone != null)
            activeTween.Chain().TweenCallback(Callable.From(onDone));
    }

    private void PlayStepTransition(System.Action onMidpoint, System.Action onDone = null)
    {
        KillTween();
        activeTween = CreateTween();

        activeTween.TweenProperty(InfoBox, "modulate:a", 0f, StepTransitionDuration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Sine);

        activeTween.TweenCallback(Callable.From(onMidpoint));

        activeTween.TweenProperty(InfoBox, "position:y", InfoBox.Position.Y + 20f, 0f);

        var parallel = activeTween.SetParallel(true);
        activeTween.TweenProperty(InfoBox, "modulate:a", 1f, StepTransitionDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
        activeTween.TweenProperty(InfoBox, "position:y", InfoBox.Position.Y, StepTransitionDuration)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);
        activeTween.SetParallel(false);

        activeTween.TweenCallback(Callable.From(() => Root.QueueRedraw()));

        if (onDone != null)
            activeTween.TweenCallback(Callable.From(onDone));
    }

    private void PlayOutroAnimation(System.Action onDone = null)
    {
        Global global = Global.GetInstance();
        global.SetState("PlayerCanMove", true);

        KillTween();
        activeTween = CreateTween().SetParallel();

        activeTween.TweenProperty(this, "modulate:a", 0f, FadeDuration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Cubic);

        activeTween.TweenProperty(InfoBox, "position:y", InfoBox.Position.Y + 80f, FadeDuration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);

        if (onDone != null)
            activeTween.Chain().TweenCallback(Callable.From(onDone));
    }

    private void KillTween()
    {
        if (activeTween != null && activeTween.IsValid())
            activeTween.Kill();
        activeTween = null;
    }


    private void ShowStep(int index)
    {
        CurrentStep = index;
        var step = Steps[index];

        TitleLabel.Text = step.Title;
        DescLabel.Text = step.Description;
        NextButton.Text = (index >= Steps.Length - 1) ? "Done" : "Next";

        Root.QueueRedraw();
    }


    public override void _Draw()
    {
        if (CurrentStep >= Steps.Length) return;
        var step = Steps[CurrentStep];
        var screenSize = Root.Size;
        var rect = new Rect2(step.HighlightPosition, step.HighlightSize);

        Root.DrawRect(new Rect2(0, 0, screenSize.X, rect.Position.Y), OverlayColor);
        Root.DrawRect(new Rect2(0, rect.End.Y, screenSize.X, screenSize.Y - rect.End.Y), OverlayColor);
        Root.DrawRect(new Rect2(0, rect.Position.Y, rect.Position.X, rect.Size.Y), OverlayColor);
        Root.DrawRect(new Rect2(rect.End.X, rect.Position.Y, screenSize.X - rect.End.X, rect.Size.Y), OverlayColor);

        Root.DrawRect(rect, BorderColor, false, BorderWidth);

        DrawArrow(rect);
    }

    private void DrawArrow(Rect2 targetRect)
    {
        var infoBoxRect = InfoBox.GetRect();
        Vector2 boxPoint = NearestCorner(infoBoxRect, targetRect.GetCenter());
        Vector2 targetPoint = NearestCorner(targetRect, boxPoint);

        Color neonCyan = new Color(0.0f, 0.9f, 1.0f, 1f);
        Color neonGlow = new Color(0.0f, 0.9f, 1.0f, 0.15f);
        Color neonMid  = new Color(0.0f, 0.9f, 1.0f, 0.35f);

        // Yderste glow lag (tykkest, mest gennemsigtig)
        Root.DrawLine(boxPoint, targetPoint, neonGlow, 12f, true);
        // Midterste glow lag
        Root.DrawLine(boxPoint, targetPoint, neonMid, 5f, true);
        // Kerne linjen (skarp og lys)
        Root.DrawLine(boxPoint, targetPoint, neonCyan, 1.5f, true);

        DrawArrowHead(targetPoint, boxPoint, neonCyan, neonGlow);
    }

    private void DrawArrowHead(Vector2 tip, Vector2 from, Color coreColor, Color glowColor)
    {
        Vector2 dir = (tip - from).Normalized();
        Vector2 perp = new Vector2(-dir.Y, dir.X);
        float size = 14f;

        Vector2 left  = tip - dir * size + perp * (size * 0.5f);
        Vector2 right = tip - dir * size - perp * (size * 0.5f);

        // Fyldt trekant som pilehoved
        Vector2[] triangle = { tip, left, right };

        // Glow lag under trekanten
        Color glowFill = new Color(glowColor.R, glowColor.G, glowColor.B, 0.25f);
        Root.DrawPolygon(triangle, new Color[] { glowFill, glowFill, glowFill });

        Root.DrawPolyline(new Vector2[] { tip, left, right, tip }, coreColor, 1.5f, true);

        Root.DrawCircle(tip, 3.5f, coreColor);
        Root.DrawArc(tip, 6f, 0, Mathf.Tau, 16, new Color(coreColor.R, coreColor.G, coreColor.B, 0.4f), 1f);
    }

    private Vector2 NearestCorner(Rect2 rect, Vector2 from)
    {
        Vector2[] corners = {
            rect.Position,
            new Vector2(rect.End.X, rect.Position.Y),
            new Vector2(rect.Position.X, rect.End.Y),
            rect.End
        };

        Vector2 nearest = corners[0];
        float minDist = from.DistanceTo(corners[0]);
        foreach (var c in corners)
        {
            float d = from.DistanceTo(c);
            if (d < minDist) { minDist = d; nearest = c; }
        }
        return nearest;
    }

    private void OnNext()
    {
        if (CurrentStep >= Steps.Length - 1)
        {
            PlayOutroAnimation(() => { Visible = false; });
            return;
        }

        int nextIndex = CurrentStep + 1;
        PlayStepTransition(
            onMidpoint: () => ShowStep(nextIndex)
        );
    }

    private void OnSkip()
    {
        PlayOutroAnimation(() => { Visible = false; });
    }

    private void ApplyStyle()
    {
        var panelStyle = new StyleBoxFlat();
        panelStyle.BgColor = new Color(0.05f, 0.07f, 0.12f, 0.85f);

        panelStyle.BorderColor = new Color(0.0f, 0.9f, 1.0f, 1f);
        panelStyle.SetBorderWidthAll(2);

        panelStyle.SetCornerRadiusAll(8);

        panelStyle.ExpandMarginTop = 2f;
        panelStyle.ExpandMarginBottom = 2f;
        panelStyle.ExpandMarginLeft = 2f;
        panelStyle.ExpandMarginRight = 2f;

        panelStyle.ShadowColor = new Color(0.0f, 0.9f, 1.0f, 0.3f);
        panelStyle.ShadowSize = 12;
        panelStyle.ShadowOffset = new Vector2(0, 4);

        InfoBox.AddThemeStyleboxOverride("panel", panelStyle);

        TitleLabel.AddThemeColorOverride("font_color", new Color(0.0f, 0.95f, 1.0f, 1f));
        TitleLabel.AddThemeFontSizeOverride("font_size", 45);

        DescLabel.AddThemeColorOverride("font_color", new Color(0.75f, 0.85f, 0.9f, 1f));
        DescLabel.AddThemeFontSizeOverride("font_size", 30);

        StyleButton(NextButton, new Color(0.0f, 0.9f, 1.0f), new Color(0.02f, 0.08f, 0.15f));
        StyleButton(SkipButton,  new Color(0.4f, 0.4f, 0.5f), new Color(0.08f, 0.08f, 0.12f));
    }

    private void StyleButton(Button btn, Color borderColor, Color bgColor)
    {
        // Normal
        var normal = new StyleBoxFlat();
        normal.BgColor = bgColor;
        normal.BorderColor = borderColor;
        normal.SetBorderWidthAll(2);
        normal.SetCornerRadiusAll(6);
        normal.ContentMarginLeft = 16;
        normal.ContentMarginRight = 16;
        normal.ContentMarginTop = 6;
        normal.ContentMarginBottom = 6;
        normal.ShadowColor = new Color(borderColor.R, borderColor.G, borderColor.B, 0.25f);
        normal.ShadowSize = 6;

        var hover = (StyleBoxFlat)normal.Duplicate();
        hover.BgColor = new Color(
            bgColor.R + 0.08f,
            bgColor.G + 0.08f,
            bgColor.B + 0.1f,
            1f
        );
        hover.ShadowSize = 10;
        hover.ShadowColor = new Color(borderColor.R, borderColor.G, borderColor.B, 0.5f);

        var pressed = (StyleBoxFlat)normal.Duplicate();
        pressed.BgColor = new Color(
            borderColor.R * 0.3f,
            borderColor.G * 0.3f,
            borderColor.B * 0.3f,
            1f
        );

        btn.AddThemeStyleboxOverride("normal", normal);
        btn.AddThemeStyleboxOverride("hover", hover);
        btn.AddThemeStyleboxOverride("pressed", pressed);
        btn.AddThemeStyleboxOverride("focus", normal); 

        btn.AddThemeColorOverride("font_color", borderColor);
        btn.AddThemeColorOverride("font_hover_color", new Color(1f, 1f, 1f, 1f));
        btn.AddThemeColorOverride("font_pressed_color", new Color(1f, 1f, 1f, 1f));
        btn.AddThemeFontSizeOverride("font_size", 25);
    }
}   