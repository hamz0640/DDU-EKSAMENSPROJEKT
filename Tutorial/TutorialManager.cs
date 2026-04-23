using Godot;
using System.Collections.Generic;

public partial class TutorialManager : Control
{
    [Export] public TutorialStep[] Steps { get; set; } = [];

    [Export] public Color OverlayColor { get; set; } = new Color(0, 0, 0, 0.75f);
    [Export] public Color BorderColor { get; set; } = new Color(1, 1, 1, 1f);
    [Export] public float BorderWidth { get; set; } = 3f;

    // Editor-assigned NodePaths
    [Export] public NodePath RootPath { get; set; }
    [Export] public NodePath InfoBoxPath { get; set; }
    [Export] public NodePath TitleLabelPath { get; set; }
    [Export] public NodePath DescLabelPath { get; set; }
    [Export] public NodePath NextButtonPath { get; set; }
    [Export] public NodePath SkipButtonPath { get; set; }

    // Actual node references
    private Control Root;
    private Panel InfoBox;
    private Label TitleLabel;
    private Label DescLabel;
    private Button NextButton;
    private Button SkipButton;

    private int CurrentStep = 0;

    public override void _Ready()
    {
        // Resolve NodePaths
        Root = GetNode<Control>(RootPath);
        InfoBox = GetNode<Panel>(InfoBoxPath);
        TitleLabel = GetNode<Label>(TitleLabelPath);
        DescLabel = GetNode<Label>(DescLabelPath);
        NextButton = GetNode<Button>(NextButtonPath);
        SkipButton = GetNode<Button>(SkipButtonPath);

        NextButton.Pressed += OnNext;
        SkipButton.Pressed += OnSkip;


        if (Steps.Length > 0)
            ShowStep(0);
        else
            Visible = false;
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

        // Draw overlay
        Root.DrawRect(new Rect2(0, 0, screenSize.X, rect.Position.Y), OverlayColor); // Top
        Root.DrawRect(new Rect2(0, rect.End.Y, screenSize.X, screenSize.Y - rect.End.Y), OverlayColor); // Bottom
        Root.DrawRect(new Rect2(0, rect.Position.Y, rect.Position.X, rect.Size.Y), OverlayColor); // Left
        Root.DrawRect(new Rect2(rect.End.X, rect.Position.Y, screenSize.X - rect.End.X, rect.Size.Y), OverlayColor); // Right

        // Border around highlight
        Root.DrawRect(rect, BorderColor, false, BorderWidth);

        // Draw arrow from info box to highlight
        DrawArrow(rect);
    }

    private void DrawArrow(Rect2 targetRect)
    {
        var infoBoxRect = InfoBox.GetRect();

        Vector2 boxPoint = NearestCorner(infoBoxRect, targetRect.GetCenter());
        Vector2 targetPoint = NearestCorner(targetRect, boxPoint);

        Root.DrawLine(boxPoint, targetPoint, BorderColor, 2f, true);
        DrawArrowHead(targetPoint, boxPoint);
    }

    private Vector2 NearestCorner(Rect2 rect, Vector2 from)
    {
        Vector2[] corners = new Vector2[]
        {
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
            if (d < minDist)
            {
                minDist = d;
                nearest = c;
            }
        }
        return nearest;
    }

    private void DrawArrowHead(Vector2 tip, Vector2 from)
    {
        Vector2 dir = (tip - from).Normalized();
        Vector2 perp = new Vector2(-dir.Y, dir.X);
        float size = 12f;

        Vector2 left = tip - dir * size + perp * (size * 0.5f);
        Vector2 right = tip - dir * size - perp * (size * 0.5f);

        Root.DrawLine(tip, left, BorderColor, 2f);
        Root.DrawLine(tip, right, BorderColor, 2f);
    }

    private void OnNext()
    {
        if (CurrentStep >= Steps.Length - 1)
        {
            Visible = false;
            return;
        }
        ShowStep(CurrentStep + 1);
    }

    private void OnSkip()
    {
        Visible = false;
    }
}