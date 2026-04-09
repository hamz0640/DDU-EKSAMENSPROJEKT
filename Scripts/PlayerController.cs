using Godot;
using System;
using System.Security.AccessControl;
using System.Threading;

public partial class PlayerController : CharacterBody2D {
  [Export]
  public float MaxSpeed = 300.0f;

  [Export]
  public float Jump = 250.0f;
  [Export]
  public float Acceleration = 150.0f;
  [Export]
  public float WireSpeed = 80.0f;
  [Export]
  private AnimatedSprite2D AnimationPlayer;
  [Export]
  public float FullBatteryCapacity = 1;
  [Export]
  private Area2D PlayerArea2D;
  [Export]
  public PackedScene BulletScene;
  private float ShootCooldown = 0f;
  [Export]
  public float ShootRate = 0.3f;

  private bool IsMounted = false;
  private bool IsWantedFlip = false;
  private float Timer = 0f;

  // Jetpack
  [Export]
  public float JetpackForce = -500f;
  [Export]
  public float BoostVelocity = -350f;
  [Export]
  public float JetpackDrainRate = 5f;
  private bool IsJumpingSideways = false;

  [Export]
  public float BoostDuration = 0.15f;

  private enum JetpackState { Grounded, Boosting, Flying }
  private JetpackState JetpackStateVar = JetpackState.Grounded;
  private float BoostTimer = 0f;
  private bool IsJetpacking = false;

  private float LastMoveInput = 1f;
  private float InputZeroTimer = 0f;
  private const float InputZeroGrace = 0.1f;  // 100 ms ani delay ig
  private bool IsShooting = false;

  public override void _Ready() {
    AnimationPlayer.AnimationFinished += OnAnimationFinished;
  }

  private void OnAnimationFinished() {
    if (AnimationPlayer.Animation == "shoot")
      IsShooting = false;
  }

  public override void _PhysicsProcess(double delta) {
    Vector2 velocity = Velocity;

    // Hvis ikke på gulv, og ikke mountet på wiren, GRAVITY!
    if (!IsOnFloor() && !IsMounted)
      velocity += GetGravity() * (float)delta;

    bool IsFalling = velocity.Y > 100f && !IsOnFloor();

    Vector2 inputDirection =
        Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

    // Jetpack implementation
    bool SpaceJustPressed = Input.IsActionJustPressed("ui_accept");
    bool SpaceHeld = Input.IsActionPressed("ui_accept");
    bool ShootPressed = Input.IsActionPressed("shoot");

    ShootCooldown -= (float)delta;

    Global global = Global.GetInstance();

    if (!IsMounted) {
      if (inputDirection.X > 0.0f)
        AnimationPlayer.FlipH = false;
      if (inputDirection.X < 0.0f)
        AnimationPlayer.FlipH = true;

      if (ShootPressed && IsOnFloor() && !IsMounted && ShootCooldown <= 0f) {
        IsShooting = true;
        ShootCooldown = ShootRate;
        AnimationPlayer.Play("shoot");

       Robotbullet bullet = BulletScene.Instantiate<Robotbullet>();

        bullet.Direction = AnimationPlayer.FlipH ? -1 : 1;
        
        GetParent().AddChild(bullet);
        Vector2 offset = new Vector2(-1, 2);
        bullet.GlobalPosition = GlobalPosition + offset;
        
      }

      if (IsOnFloor())
        JetpackStateVar = JetpackState.Grounded;

      switch (JetpackStateVar) {
        case JetpackState.Grounded:
          IsJetpacking = false;
          IsJumpingSideways = false;  // Nulstil når man lander
          if (SpaceJustPressed) {
            velocity.Y = -Jump;
            JetpackStateVar = JetpackState.Boosting;
            BoostTimer = BoostDuration;

            float jumpInput = Input.GetAxis("ui_left", "ui_right");
            if (Mathf.Abs(jumpInput) > 0.1f) {
              AnimationPlayer.FlipH = jumpInput < 0.0;
              AnimationPlayer.Play("jetpack jump side");
              IsJumpingSideways = true;
            } else {
              AnimationPlayer.Play("jetpack jump");
              IsJumpingSideways = false;
            }
          }
          break;

        case JetpackState.Boosting:
          IsJetpacking = true;
          BoostTimer -= (float)delta;

          float boostMoveInput = Input.GetAxis("ui_left", "ui_right");

          if (IsJumpingSideways) {
            // Skift til side-animation hvis man skifter retning
            if (Mathf.Abs(boostMoveInput) > 0.1f)
              AnimationPlayer.FlipH = boostMoveInput < 0.0;
            AnimationPlayer.Play("jetpack jump side");
          } else if (Mathf.Abs(boostMoveInput) > 0.1f) {
            AnimationPlayer.FlipH = boostMoveInput < 0.0;
            AnimationPlayer.Play("jetpack side");
            IsJumpingSideways = true;
          } else {
            AnimationPlayer.Play("jetpack boost");
          }

          if (BoostTimer <= 0f)
            JetpackStateVar = JetpackState.Flying;
          break;

        case JetpackState.Flying:
          float currentEnergy = global.GetState<float>("CurrentEnergy");
          if (SpaceHeld && currentEnergy > 0) {
            IsJetpacking = true;
            velocity.Y = JetpackForce;
            currentEnergy -= JetpackDrainRate * (float)delta;
            currentEnergy = Mathf.Max(currentEnergy, 0);
            global.SetState("CurrentEnergy", currentEnergy);
          } else {
            IsJetpacking = false;
          }

          float moveInput = Input.GetAxis("ui_left", "ui_right");

          if (Mathf.Abs(moveInput) > 0.1f) {
            LastMoveInput = moveInput;
            InputZeroTimer = 0f;
          } else {
            InputZeroTimer += (float)delta;
          }

          float effectiveMoveInput =
              (InputZeroTimer < InputZeroGrace) ? LastMoveInput : 0f;

          if (Velocity.Y > 0.0) {
            AnimationPlayer.Play("jetpack fall");
          } else if (Mathf.Abs(effectiveMoveInput) > 0.1f) {
            AnimationPlayer.FlipH = effectiveMoveInput < 0.0;
            AnimationPlayer.Play("jetpack side");
          } else {
            AnimationPlayer.Play("jetpack boost");
          }
          break;
      }
    }

    bool isTouchingWire = false;
    var areas = PlayerArea2D.GetOverlappingAreas();
    foreach (var area in areas) {
      if (area.IsInGroup("wire"))
        isTouchingWire = true;
    }  // Tjek om spilleren rører wiren

    if (Input.IsActionJustPressed("ui_mount") &&
        isTouchingWire)  // Funktionalitet til at mounte
    {
      IsMounted = !IsMounted;
      if (IsMounted) {
        velocity.X = 0;                         // Fix walk glitch
        velocity.Y = 0;                         // Fix jump glitch
        Tween tween = GetTree().CreateTween();  // Snap to wire X coordinate
        tween.SetEase(Tween.EaseType.Out);
        tween.SetTrans(Tween.TransitionType.Quint);
        tween.TweenProperty(this, "position", new Vector2(-50, this.Position.Y),
                            0.5f);
      }
    }

    if (IsMounted) {
      // Hvis velocity X er 0, så er spilleren mountet og på den rigtige
      // position
      AnimationPlayer.FlipH = false;
      if (Velocity.X == 0 && velocity.X == 0) {
        AnimationPlayer.Play("climb");
      }

      if (GlobalPosition.Y < -22) {
        velocity.Y = 20;
        IsMounted = false;
        velocity.X = 120;
      }  // spring af wiren når man rammer toppen
      else {
        if (Input.IsActionJustPressed("ui_up"))
          velocity.Y = -WireSpeed;
        if (Input.IsActionJustPressed("ui_down"))
          velocity.Y = WireSpeed;
        if (Input.IsActionJustReleased("ui_up") ||
            Input.IsActionJustReleased("ui_down"))
          velocity.Y = 0;

        Vector2 inputDirection2 =
            Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (Input.IsActionJustPressed("ui_accept")) {
          if (inputDirection2.X > 0) {
            velocity.Y = -150;
            velocity.X = 150;
          }
          if (inputDirection2.X < 0) {
            velocity.Y = -150;
            velocity.X = -150;
          }
          IsMounted = false;
        }  // Hopper af wiren til siden

      }  // Bevægelse på wiren

      // Da spilleren kravler på wiren, så kan den hverken bevæge sig sidelæns
      // eller mine
      goto EarlyExit;
    }

    float towards = inputDirection.X == 0 ? 0 : MaxSpeed * inputDirection.X;
    velocity.X =
        Mathf.MoveToward(velocity.X, towards, Acceleration * (float)delta);

    if (!IsJetpacking && !IsShooting) {
      if (Mathf.Abs(velocity.X) != MaxSpeed && !IsMounted && Velocity.X != 0) {
        AnimationPlayer.Play("acceleration");
      }

      if (Velocity.X > 0.0)
        AnimationPlayer.FlipH = false;
      if (Velocity.X < 0.0)
        AnimationPlayer.FlipH = true;

      if (Velocity.X == 0.0 && inputDirection.X == 0.0) {
        AnimationPlayer.FlipH = false;
        AnimationPlayer.Play("idle");
      }

      if (Mathf.Abs(Velocity.X) == MaxSpeed) {
        AnimationPlayer.Play("move");
      }
    }

    if (!ShootPressed && IsShooting) {
      IsShooting = false;
    }

    if (inputDirection.Angle() % (Mathf.Pi / 2.0) == 0 &&
        inputDirection != Vector2.Zero)  // Mine implementation
    {
      Ground ground = (Ground)GetTree().GetFirstNodeInGroup("Ground");

      Vector2I tileDirection = (Vector2I)inputDirection;
      Vector2I tilePosition = ground.ToTilePosition(GlobalPosition);
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

      TileData tileData =
          ground.GroundLayer.GetCellTileData(miningTilePosition);
      if (tileData == null)
        goto EarlyExit;

      float miningSpeed = global.GetState<float>("MiningSpeed");

      float tileHealth = ground.TileHealth[miningTilePosition];
      float newTileHealth = tileHealth - miningSpeed * (float)delta;

      if (newTileHealth <= 0.0)
        ground.BreakTile(miningTilePosition);
      else
        ground.TileHealth[miningTilePosition] = newTileHealth;
    }

  EarlyExit:

    Velocity = velocity;
    MoveAndSlide();
  }

  private static void ShowSpecificFrame(AnimatedSprite2D AnimationPlayer,
                                        string Animation, int frame) {
    AnimationPlayer.Stop();  // Stop den først
    AnimationPlayer.Animation = Animation;
    AnimationPlayer.Frame = frame;  // Sæt frame bagefter
  }
}
