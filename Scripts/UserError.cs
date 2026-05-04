using Godot;

public partial class UserError : Control
{
    [Export] public Label ErrorLabel;
    public string ErrorText;
    public float DisplayTime = 2f;
    private Tween tween;
    [Export] TextureProgressBar progressBar;

    public override void _Ready()
    {
        ErrorLabel.Text = ErrorText;
        this.tween = CreateTween();
        tween.TweenInterval(DisplayTime);
        tween.Chain().TweenProperty(this, "modulate:a", 0, 0.5f);
        tween.TweenCallback(Callable.From(QueueFree));

        int count = GetParent().GetChildCount();
        GlobalPosition = new Vector2(-10, -120 + (count * 135));
    }

    public override void _PhysicsProcess(double delta)
    {
        double percentage = 100-(tween.GetTotalElapsedTime()/(double)DisplayTime)*100;
        progressBar.Value = percentage;
    }
}