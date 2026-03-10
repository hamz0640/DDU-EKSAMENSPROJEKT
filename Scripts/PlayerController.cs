using Godot;
using System;
using System.Security.AccessControl;
using System.Threading;

public partial class PlayerController : CharacterBody2D
{
	[Export] public float MaxSpeed = 300.0f;

    [Export] public HeartsBar HeartsBar;

    [Export] public float Jump = 250.0f;
	[Export] public float Acceleration = 150.0f;
	[Export] public float WireSpeed = 80.0f;
    [Export] public float MiningSpeed = 0.5f;
	[Export] private AnimatedSprite2D animationPlayer;
    [Export] public float fullBatteryCapacity = 1;
    [Export] private Area2D playerArea2D;

    private bool isMounted = false;
    private bool IsWantedFlip = false;
    private float _timer = 0f;



    public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;

        if (!IsOnFloor() && !isMounted) // Hvis ikke på gulv, og ikke mountet på wiren, GRAVITY!
            velocity += GetGravity() * (float)delta;
        
        Vector2 inputDirection = Input.GetVector(
            "ui_left", "ui_right", "ui_up", "ui_down"
        );

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor() && !isMounted) // Hvis ikke mountet og på gulv, JUMP!
            velocity.Y = -Jump;

        bool isTouchingWire = false;
        var areas = playerArea2D.GetOverlappingAreas();
        foreach (var area in areas)
        {
            if (area.IsInGroup("wire"))
                isTouchingWire = true;
        }

        if (Input.IsActionJustPressed("ui_mount") && isTouchingWire) // Funktionalitet til at mounte
        {
            isMounted = !isMounted;
            if (isMounted)
                velocity.Y = 0; // Fix jump glitch
        }

        if (isMounted)
        {
            //animationPlayer.Play("climb");
            GlobalPosition = new Vector2(48, GlobalPosition.Y); // Snap to wire X coordinate

            if (isTouchingWire == false)
            {
                velocity.Y = 20;
                isMounted = false;
                velocity.X = 120; // spring af wiren
            }
            else
            {
                if (Input.IsActionJustPressed("ui_up"))
                    velocity.Y = -WireSpeed;
                if (Input.IsActionJustPressed("ui_down"))
                    velocity.Y = WireSpeed;

                if (Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down"))
                    velocity.Y = 0;
            }

            // Da spilleren kravler på wiren, så kan den hverken bevæge sig sidelæns eller mine
            goto EarlyExit;
        }

        float towards = inputDirection.X == 0 ? 0 : MaxSpeed * inputDirection.X;
        velocity.X = Mathf.MoveToward(velocity.X, towards, Acceleration * (float)delta);

        if (Mathf.Abs(velocity.X) != MaxSpeed && !isMounted && Velocity.X != 0) // Matematik til animationen
            ShowSpecificFrame(animationPlayer, "acceleration", (int)(Mathf.Abs(Velocity.X) / (MaxSpeed / 5.0)));

        if (Velocity.X > 0.0)
            animationPlayer.FlipH = false;
        if (Velocity.X < 0.0)
            animationPlayer.FlipH = true;
        
        if (Velocity.X == 0.0 && inputDirection.X == 0.0)
        {
            animationPlayer.FlipH = false;
            animationPlayer.Play("idle");
        }
        
        if (Mathf.Abs(Velocity.X) == MaxSpeed)
        {
            animationPlayer.Play("move");
        }

        if (inputDirection.Angle() % (Mathf.Pi / 2.0) == 0 && inputDirection != Vector2.Zero) // Mine implementation
        {
            Ground ground = (Ground)GetTree().GetFirstNodeInGroup("Ground");

            Vector2I tileDirection = (Vector2I)inputDirection;
            Vector2I tilePosition  = ground.ToTilePosition(GlobalPosition);
            Vector2I miningTilePosition = tilePosition + tileDirection;

            bool blocked = false;

            if (tileDirection.X != 0)
                blocked = IsOnWall();
            else if (tileDirection.Y > 0)
                blocked = IsOnFloor();
            else if (tileDirection.Y < 0)
                blocked = IsOnCeiling();

            if (!blocked)
                goto EarlyExit;

            TileData tileData = ground.GroundLayer.GetCellTileData(miningTilePosition);
            if (tileData == null)
                goto EarlyExit;

            float tileHealth = ground.TileHealth[miningTilePosition];
            float newTileHealth = tileHealth - MiningSpeed * (float)delta;

            if (newTileHealth <= 0.0)
                ground.BreakTile(miningTilePosition);
            else
                ground.TileHealth[miningTilePosition] = newTileHealth;
        }

        EarlyExit:

        Velocity = velocity;
        MoveAndSlide();
    }
    private static void ShowSpecificFrame(AnimatedSprite2D animationPlayer, string animation, int frame)
    {
        animationPlayer.Stop(); // Stop den først
        animationPlayer.Animation = animation;
        animationPlayer.Frame = frame; // Sæt frame bagefter
    }

}
