using Godot;

public partial class UserError : Control
{
    [Export] public Label ErrorLabel;
    public string ErrorText;
    public float DisplayTime = 2f;

    public override void _Ready()
    {
        ErrorLabel.Text = ErrorText;
        var tween = CreateTween();
        tween.TweenInterval(DisplayTime);
        tween.Chain().TweenProperty(this, "modulate:a", 0, 0.5f);
        tween.TweenCallback(Callable.From(QueueFree));

        int count = GetParent().GetChildCount();
        GlobalPosition = new Vector2(-10, -120 + (count * 60));
    }
}