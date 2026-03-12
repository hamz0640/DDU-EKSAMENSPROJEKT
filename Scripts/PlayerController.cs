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

    // Jetpack 
    [Export] public float JetpackForce = -500f;
    [Export] public float BoostVelocity = -350f;
    [Export] public float JetpackDrainRate = 5f;
    private EnergyBar _energyBar;

    [Export] public float BoostDuration = 0.15f;

    private enum JetpackState { Grounded, Boosting, Flying }
    private JetpackState _jetpackState = JetpackState.Grounded;
    private float _boostTimer = 0f;
    private bool _isJetpacking = false;
    public override void _Ready()
    {
       _energyBar = GetTree().GetFirstNodeInGroup("energy_bar") as EnergyBar;

        if (_energyBar == null)
        {
            GD.PrintErr("EnergyBar not found in group   ");
        }
    }
    public override void _PhysicsProcess(double delta)
	{
        Vector2 velocity = Velocity;

        if (!IsOnFloor() && !isMounted ) // Hvis ikke på gulv, og ikke mountet på wiren, GRAVITY!
            velocity += GetGravity() * (float)delta;

        bool isFalling = velocity.Y > 50f && !IsOnFloor();
        
        Vector2 inputDirection = Input.GetVector(
            "ui_left", "ui_right", "ui_up", "ui_down"
        );

        // Jetpack implementation 
        bool spaceJustPressed = Input.IsActionJustPressed("ui_accept");
        bool spaceHeld = Input.IsActionPressed("ui_accept");

        if (!isMounted)
        {
            // Reset til grounded
            if (IsOnFloor())
                _jetpackState = JetpackState.Grounded;

          switch (_jetpackState){
            case JetpackState.Grounded:
                _isJetpacking = false;
                if (spaceJustPressed){
                        velocity.Y = -Jump;
                        _jetpackState = JetpackState.Boosting;
                        _boostTimer = BoostDuration;
                        animationPlayer.Play("jetpack jump"); 
                    }
                    break;

            case JetpackState.Boosting:
                _isJetpacking = true;
                _boostTimer -= (float)delta;
                animationPlayer.Play("jetpack boost"); 
                if (_boostTimer <= 0f)
                    _jetpackState = JetpackState.Flying;
                break;

            case JetpackState.Flying:
                if (spaceHeld && _energyBar != null && _energyBar.HasEnergy())
                {
                    
                    _isJetpacking = true;
                    velocity.Y = JetpackForce;
                    _energyBar.DrainPerSecond(JetpackDrainRate, (float)delta);
                    if (Velocity.X > 0.0)
                        animationPlayer.FlipH = false;
                    if (Velocity.X < 0.0)
                        animationPlayer.FlipH = true;
                    
                    animationPlayer.Play("jetpack side");

                    if (Velocity.X == 0)
                        {
                            animationPlayer.Play("jetpack boost");
                        }
                }
                else if (isFalling)
                {
                    _isJetpacking = true;
                    animationPlayer.Play("jetpack fall"); 
                }
                break;
                }
        }


        bool isTouchingWire = false;
        var areas = playerArea2D.GetOverlappingAreas();
        foreach (var area in areas){
            if (area.IsInGroup("wire"))
                isTouchingWire = true;
        } // Tjek om spilleren rører wiren

        if (Input.IsActionJustPressed("ui_mount") && isTouchingWire) // Funktionalitet til at mounte
        {
            isMounted = !isMounted;
            if (isMounted)
            {
                velocity.X = 0; // Fix walk glitch
                velocity.Y = 0; // Fix jump glitch
                Tween tween = GetTree().CreateTween(); // Snap to wire X coordinate
                tween.SetEase(Tween.EaseType.Out);
                tween.SetTrans(Tween.TransitionType.Quint);
                tween.TweenProperty(this, "position", new Vector2(-50, this.Position.Y), 0.5f);
            }

        }

        if (isMounted)
        {
            // Hvis velocity X er 0, så er spilleren mountet og på den rigtige position
            animationPlayer.FlipH = false;
            if(Velocity.X == 0 && velocity.X == 0)
            {
                animationPlayer.Play("climb");
            }

            if (GlobalPosition.Y < -22)
            {
                velocity.Y = 20;
                isMounted = false;
                velocity.X = 120;
            } // spring af wiren når man rammer toppen
            else
            {
                if (Input.IsActionJustPressed("ui_up"))
                    velocity.Y = -WireSpeed;
                if (Input.IsActionJustPressed("ui_down"))
                    velocity.Y = WireSpeed;
                if (Input.IsActionJustReleased("ui_up") || Input.IsActionJustReleased("ui_down"))
                    velocity.Y = 0;

                Vector2 inputDirection2 = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
                if (Input.IsActionJustPressed("ui_accept"))
                {   
                    if (inputDirection2.X > 0)
                    {
                        velocity.Y = -150;
                        velocity.X = 150;
                    }
                    if (inputDirection2.X < 0)
                    {
                        velocity.Y = -150;
                        velocity.X = -150;
                    }
                    isMounted = false;
                } // Hopper af wiren til siden

            } // Bevægelse på wiren

            // Da spilleren kravler på wiren, så kan den hverken bevæge sig sidelæns eller mine
            goto EarlyExit;
        }

        float towards = inputDirection.X == 0 ? 0 : MaxSpeed * inputDirection.X;
        velocity.X = Mathf.MoveToward(velocity.X, towards, Acceleration * (float)delta);

        if (!_isJetpacking)
        {
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
