using Godot;
using System;
using System.IO;

public partial class UserError : Control
{
	[Export] Label ErrorLabel;
	public string ErrorText { get; set; }
    private int ErrorID;
    private float Time = 2f;
    private SceneTreeTimer Cooldown = null;
    Global global = Global.GetInstance();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		ErrorLabel.Text = this.ErrorText;
        Cooldown = GetTree().CreateTimer(Time);

        
        this.GlobalPosition = new Vector2(-35, -150); // afhængig af hvor mange.


    }

	public override void _Process(double delta)
	{
		if (Cooldown.TimeLeft <= 0)
		{
            global.SetState("ErrorCount", ErrorID--);
			QueueFree();
        }
	}

    public void CreateError(string message, int time)
    {
        int errorID = global.GetState<int>("ErrorCount");
        global.SetState("ErrorCount", errorID++);

        var errorScene = GD.Load<PackedScene>("res://Scenes/UserError.tscn");

        var newScene = errorScene.Instantiate<UserError>();
        newScene.ErrorText = message;
        newScene.Time = 5;
        ErrorID = errorID;
        AddChild(newScene);
    }

    public static UserError Instance { get { return Instance; } } }
}
