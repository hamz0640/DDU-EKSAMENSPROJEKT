using Godot;

[GlobalClass]
public partial class TutorialStep : Resource
{
    [Export] public string Title { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public Vector2 HighlightPosition { get; set; } = Vector2.Zero;
    [Export] public Vector2 HighlightSize { get; set; } = new Vector2(100, 100);
}