using Godot;
using System;

public partial class RoofTurret : Sprite2D
{
    [Export]
    public Sprite2D Crosshair;
    private bool InTurret = false;
    private bool JustEnteredTurret = false;
    private SceneTreeTimer Cooldown = null;
    private Vector2 CrosshairPosition = Vector2.Zero;
    private float TurretVelocity = 0.0f;
    private float MaxTurretVelocity = 2.0f;
    private float TurretAcceleration = 5.0f;
    AudioStreamPlayer2D sfx;
    public override void _Ready()
    {
        Cooldown = GetTree().CreateTimer(2.0f);
        sfx = GetNode<AudioStreamPlayer2D>("Shot");
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
            Vector2 direction = Input.GetVector("left", "right", "up", "down");
            CrosshairPosition += direction * 250.0f * (float)delta;

            float angle = CrosshairPosition.Rotated(Mathf.Pi / 2.0f).Angle();
            TurretVelocity += Mathf.Sign(angle - Rotation - Mathf.Pi / 2.0f) * TurretAcceleration * (float)delta;
            TurretVelocity = Mathf.Clamp(TurretVelocity, -MaxTurretVelocity, MaxTurretVelocity);
            TurretVelocity -= TurretVelocity * 4f * (float)delta;
            if (Mathf.Abs(TurretVelocity) > 0.1f)
            {
                Rotation += TurretVelocity * (float)delta;
                Rotation = Rotation % (Mathf.Pi * 2);
            }

            CrosshairPosition.X = Mathf.Clamp(CrosshairPosition.X, -620.0f, 620.0f);
            CrosshairPosition.Y = Mathf.Clamp(CrosshairPosition.Y, -340.0f, 80.0f);

            Crosshair.GlobalPosition = GlobalPosition + CrosshairPosition;
            Crosshair.GlobalRotation = 0;
        }

        if (InTurret && Input.IsActionJustPressed("jump") && Cooldown.TimeLeft <= 0)
        {
            Tracker tracker = Tracker.GetInstance();
            tracker.IncrementTracking("Wave:TimesShot", 1u);

            Cooldown = GetTree().CreateTimer(2.0f);
            PackedScene bulletScene = (PackedScene)GD.Load("res://Scenes/roof_turret_bullet.tscn");
            RoofTurretBullet bullet = (RoofTurretBullet)bulletScene.Instantiate();

            bullet.Velocity = Vector2.Right.Rotated(Rotation).Normalized() * 1000.0f;
            bullet.Rotation = Rotation;
            bullet.GlobalPosition = GlobalPosition + Vector2.Right.Rotated(Rotation).Normalized() * 60.0f;

            Tween tween = GetTree().CreateTween();
            tween.SetParallel(false);

            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(this, "offset", new Vector2(15, 0), 0.2f);

            tween.SetEase(Tween.EaseType.InOut);
            tween.SetTrans(Tween.TransitionType.Linear);
            tween.TweenProperty(this, "offset", new Vector2(30, 0), 1.8f);
            sfx.Play();
            GetTree().Root.AddChild(bullet);
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
