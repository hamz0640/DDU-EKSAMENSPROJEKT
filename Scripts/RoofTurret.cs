using Godot;
using System;

public partial class RoofTurret : Sprite2D
{
    [Export]
    public Sprite2D Crosshair;
    private bool InTurret = false;
    private bool JustEnteredTurret = false;
    public override void _Ready()
    {
        ((TurretDoor)GetNode("/root/Main/TurretDoor")).ToggleTurret += OnToggleTurret;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (InTurret && !JustEnteredTurret && Input.IsActionJustPressed("interact"))
        {
            ((TurretDoor)GetNode("/root/Main/TurretDoor")).EmitSignal(TurretDoor.SignalName.ToggleTurret);
        }

        JustEnteredTurret = false;

        if (InTurret)
        {
            Vector2 mousePosition = GetGlobalMousePosition();
            Vector2 turretPosition = GlobalPosition;

            float angle = Vector2.Up.AngleTo(mousePosition - turretPosition);
            Rotation = Mathf.MoveToward(Rotation, angle - Mathf.Pi / 2.0f, 0.025f);
            Rotation = Mathf.Clamp(Rotation, -Mathf.Pi * 17 / 16, Mathf.Pi / 16);

            Crosshair.GlobalPosition = mousePosition;
            Crosshair.GlobalRotation = 0;
        }
    }


    private void OnToggleTurret()
    {
        InTurret = !InTurret;
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");

        if (InTurret)
        {
            CanvasLayer UI = (CanvasLayer)GetTree().GetFirstNodeInGroup("UI");
            Crosshair.Show();
            UI.Hide();

            JustEnteredTurret = true;
            Tween tween = GetTree().CreateTween();
            tween.SetParallel(false);

            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Quint);
            tween.TweenProperty(camera, "global_position", new Vector2(208.878f, -96.855f), 0.25);
            
            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(camera, "zoom", new Vector2(1.5f, 1.5f), 1.0);
        } 
        else
        {
            CanvasLayer UI = (CanvasLayer)GetTree().GetFirstNodeInGroup("UI");
            Crosshair.Hide();
            UI.Show();

            Tween tween = GetTree().CreateTween();

            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(camera, "zoom", new Vector2(5.0f, 5.0f), 0.5);

            tween.SetEase(Tween.EaseType.Out);
            tween.SetTrans(Tween.TransitionType.Quint);
            tween.TweenProperty(camera, "position", new Vector2(0, 0), 0.25);
        }
    }
}
