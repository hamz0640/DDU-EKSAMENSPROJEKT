using Godot;
using System;
using System.Data;
using System.Security.AccessControl;
using System.Threading;

public partial class PlayerController : CharacterBody2D 
{
	[ExportGroup("Basic Movement")]
	[Export]
	public float MaxSpeed = 75.0f;
	[Export]
	public float BasicAcceleration = 300.0f;
	[Export]
	public float JumpForce = 250.0f;
	[ExportGroup("Wire Movement")]
	[Export]
	public float MaxWireSpeed = 100.0f;
	[Export]
	public float WireAcceleration = 600.0f;
	[Export]
	public Area2D WireArea = null;
	[Export]
	public float DismountBoost = 150.0f;
	[ExportGroup("Air Movement")]
	[Export]
	public float MaxJetpackSpeed = 100.0f;
	[Export]
	public float JetpackAcceleration = 100.0f;

	private State CurrentState = State.Ground;
	private bool JetpackActivated = false;

	private int TimeSinceMountChange = 0;

	private AnimatedSprite2D anim = null;
	private bool InTurret = false;

	enum State { Ground, Air, Wire };


    public override void _Ready()
    {
		((TurretDoor)GetNode("/root/Main/TurretDoor")).ToggleTurret += OnToggleTurret;
        anim = (AnimatedSprite2D)GetNode("AnimatedSprite2D");
    }


    public override void _PhysicsProcess(double delta)
    {
		Tracker tracker = Tracker.GetInstance();
		tracker.IncrementTracking("Time:Total", (float)delta);
		if (GlobalPosition.Y > 0)
		{
			tracker.IncrementTracking("Time:InMine", (float)delta);
		}

		float maxDepthReached = tracker.GetTracking<float>("Max:DepthReached");
		float maxWidthReached = tracker.GetTracking<float>("Max:WidthReached");
		tracker.SetTracking("Max:DepthReached", Mathf.Max(maxDepthReached, GlobalPosition.Y));
		tracker.SetTracking("Max:WidthReached", Mathf.Max(maxWidthReached, Mathf.Abs(GlobalPosition.X)));

		if (InTurret)
		{
			tracker.IncrementTracking("Time:InTurret", (float)delta);
			return;
		}

		HandleTransitions();
		
		switch (CurrentState) {
			case State.Ground:
                this.SetCollisionMaskValue(6, true);
                HandleGroundMovement((float)delta);
				tracker.IncrementTracking("Time:OnGround", (float)delta);
				break;
			case State.Air:
                this.SetCollisionMaskValue(6, true);
                HandleAirMovement((float)delta);
				tracker.IncrementTracking("Time:InAir", (float)delta);
				break;
			case State.Wire:
				this.SetCollisionMaskValue(6, false);
				HandleWireMovement((float)delta);
				tracker.IncrementTracking("Time:OnWire", (float)delta);
				break;
		}

		HandleMine((float)delta);
		HandleAnimations();

		TimeSinceMountChange += 1;
    }

	private void OnToggleTurret() {
		if (!InTurret)
		{
			Hide();
			GlobalPosition = new Vector2(229, -20);
			InTurret = true;
		} 
		else
		{
			Show();
			InTurret = false;
		}
	}

	private void HandleAnimations()
	{
		anim.FlipH = Velocity.X < 0.0;

		if (CurrentState == State.Ground)
		{
			if (Mathf.Abs(Velocity.X) == MaxSpeed)
				anim.Play("move");
			else if (Velocity.X == 0.0f)
				anim.Play("idle");
			else
				ShowSpecificFrame("acceleration", (int)(Mathf.Abs(Velocity.X) / (MaxSpeed / 5.0)));
		}

		if (CurrentState == State.Wire)
		{
			anim.Play("climb");
		}

		if (CurrentState == State.Air)
		{
			if (!JetpackActivated)
			{
				anim.Play("jetpack_fall");
			}
			else
			{
				anim.Play("jetpack_boost");
			}
		}
	}

	private void HandleTransitions()
	{
		// Early return, because the transition back to Air/Ground state, from
		// the wire state, is handled in the HandleWireMovement function
		if (CurrentState == State.Wire)
			return;

		if (IsOnFloor()) 
			CurrentState = State.Ground;
		else
			CurrentState = State.Air;

		if (Input.IsActionJustPressed("mount"))
		{
			bool isTouchingWire = false;
			float wireX = 0.0f;

			var areas = WireArea.GetOverlappingAreas();
			foreach (var area in areas) {
				if (area.IsInGroup("wire")) {
					isTouchingWire = true;
					wireX = area.GlobalPosition.X;
					break;
				}
			}

			if (isTouchingWire && TimeSinceMountChange > 1)
			{
				TimeSinceMountChange = -1;
				CurrentState = State.Wire;
				Velocity = Vector2.Zero;

				Tween tween = GetTree().CreateTween();
				tween.SetEase(Tween.EaseType.Out);
				tween.SetTrans(Tween.TransitionType.Quint);
				tween.TweenProperty(this, "global_position:x", wireX+1, 0.5f);
			}
		}
	}


	private void HandleMine(float delta)
	{
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

		if (inputDirection.Angle() % (Mathf.Pi / 2.0) == 0 && inputDirection != Vector2.Zero)
        {
			Global global = Global.GetInstance();
            Ground ground = (Ground)GetTree().GetFirstNodeInGroup("Ground");

			float miningSpeed = global.GetState<float>("MiningSpeed");

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
            float newTileHealth = tileHealth - miningSpeed * (float)delta;

            if (newTileHealth <= 0.0)
                ground.BreakTile(miningTilePosition);
            else
                ground.TileHealth[miningTilePosition] = newTileHealth;
        }

        EarlyExit:
			return;
	}


	private void HandleGroundMovement(float delta)
	{
		JetpackActivated = false;
		Vector2 velocity = Velocity;

		float sidewaysInput = Input.GetAxis("left", "right");
		float towards = sidewaysInput == 0 ? 0 : MaxSpeed * sidewaysInput;

		velocity.X = Mathf.MoveToward(velocity.X, towards, BasicAcceleration * (float)delta);
		if (Input.IsActionPressed("jump"))
		{
			velocity.Y -= JumpForce;
		}
		
		Velocity = velocity;
		MoveAndSlide();
	}


	private void HandleAirMovement(float delta)
	{
		Tracker tracker = Tracker.GetInstance();
		Vector2 velocity = Velocity;

		float sidewaysInput = Input.GetAxis("left", "right");
		float towards = sidewaysInput == 0 ? 0 : MaxSpeed * sidewaysInput;
		velocity.X = Mathf.MoveToward(velocity.X, towards, BasicAcceleration * delta);

		if (Input.IsActionPressed("jump")) {
			if (Velocity.Y > 0.0f)
			{
				JetpackActivated = true;
			}
		}
		else
		{
			JetpackActivated = false;
		}

		Global global = Global.GetInstance();
		float currentEnergy = global.GetState<float>("CurrentEnergy");
		float jetpackEfficiency = global.GetState<float>("JetpackEffeciency");
		float jetpackDrain      = global.GetState<float>("JetpackDrain");

		if (JetpackActivated && currentEnergy > 0.0f)
		{
			tracker.IncrementTracking("Time:UsingJetpack", delta);
			velocity.Y = Mathf.MoveToward(Velocity.Y, -MaxJetpackSpeed, JetpackAcceleration * delta);
			currentEnergy -= 1.0f / jetpackEfficiency * jetpackDrain * delta;
			global.SetState("CurrentEnergy", currentEnergy);
		}
		else
		{
			velocity += GetGravity() * delta;
		}

		Velocity = velocity;
		MoveAndSlide();
	}


	private void HandleWireMovement(float delta)
	{
		Vector2 velocity = Velocity;

		if (!Input.IsActionJustPressed("mount"))
		{
			float upwardsInput = Input.GetAxis("up", "down");
			float towards = upwardsInput == 0 ? 0 : MaxWireSpeed * upwardsInput;
			velocity.Y = Mathf.MoveToward(velocity.Y, towards, WireAcceleration * (float)delta);
		} 
		else if (TimeSinceMountChange > 1)
		{
			TimeSinceMountChange = -1;
			CurrentState = State.Air;
			float sidewaysInput = Input.GetAxis("left", "right");

			if (sidewaysInput != 0)
			{
				velocity += new Vector2(1.0f * sidewaysInput, -0.5f) * DismountBoost;
			}
		}

		if (GlobalPosition.Y < -15.0f)
		{
			velocity += new Vector2(1.0f, -0.5f) * DismountBoost;
			TimeSinceMountChange = -1;
			CurrentState = State.Air;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
      

	private void ShowSpecificFrame(string Animation, int frame) {
		anim.Stop();  // Stop den først
		anim.Animation = Animation;
		anim.Frame = frame;  // Sæt frame bagefter
	}
}
