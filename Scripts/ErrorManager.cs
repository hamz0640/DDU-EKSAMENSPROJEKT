using Godot;

public partial class ErrorManager : Node
{
    public static ErrorManager Instance { get; private set; }
    private PackedScene _errorScene = GD.Load<PackedScene>("res://Scenes/UserError.tscn");

    public override void _Ready()
    {
        Instance = this;
    }

    public void Notify(string message, float time = 2f)
    {
        var newError = _errorScene.Instantiate<UserError>();
        newError.ErrorText = message;
        newError.DisplayTime = time;

        GetNode("CanvasLayer/Control").AddChild(newError);
    }
}