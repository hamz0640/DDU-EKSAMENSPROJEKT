using Godot;
using System;
using System.Data;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Threading;

public partial class PlayerController : CharacterBody2D 
{
	[Export] AnimatedSprite2D anim;
	[Export] ShapeCast2D shapeCast1;
	[Export] ShapeCast2D shapeCast2;
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
	private bool IsMining = false;
	private int TimeSinceMountChange = 0;
	private bool InTurret = false;
	private int KnockOffWire = 0;
	enum State { Ground, Air, Wire };
	private bool HasBeenInMine = false;
	private bool FreeDrill = false;

	AudioStreamPlayer2D drilling;
	string[] MineControls = ["right", "left", "up", "down"];
	private bool MiningAudio = false;
	public override void _Ready()
	{
		((TurretDoor)GetNode("/root/Main/TurretDoor")).ToggleTurret += OnToggleTurret;
        AddToGroup("player");
		drilling = GetNode<AudioStreamPlayer2D>("Mining");
    }

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.Y < 0.0f && HasBeenInMine && InTurret)
		{
			WaveManager waveManager = WaveManager.GetInstance();
			waveManager.StartWave();
			HasBeenInMine = false;
		}
		HasBeenInMine = HasBeenInMine || GlobalPosition.Y > 0.0f;

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

		Global global = Global.GetInstance();
		if (!global.GetState<bool>("PlayerCanMove"))
			return;
		
		HandleTransitions();
		
		switch (CurrentState) {
			case State.Ground:
				HandleGroundMovement((float)delta);
				tracker.IncrementTracking("Time:OnGround", (float)delta);
				break;
			case State.Air:
				HandleAirMovement((float)delta);
				tracker.IncrementTracking("Time:InAir", (float)delta);
				break;
			case State.Wire:
				this.SetCollisionMaskValue(6, false);
				HandleWireMovement((float)delta);
				tracker.IncrementTracking("Time:OnWire", (float)delta);
				break;
		}
		IsMining = false;
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
		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
		anim.FlipH = Velocity.X < 0.0;
		if (CurrentState == State.Ground)
		{
			if (Mathf.Abs(Velocity.X) == MaxSpeed)
				anim.Play("move");
            else if (Velocity.X == 0.0f)
            {
                // Check for mining inputs FIRST
                if (Math.Round(GlobalPosition.Y) != -12 && inputDirection != Vector2.Zero && IsMining)
                {
                    if (inputDirection.Y == 1)
                    {
						HandleAudio();
                        anim.Play("mine_down");
                        anim.FlipH = false;
                        
                    }
                    else if (inputDirection.Y == -1 && Math.Round(GlobalPosition.Y) != -11)
                    {
                        HandleAudio();
                        anim.Play("mine_up");
                        anim.FlipH = false;
                        
                    }
                    else if (inputDirection.X != 0)
                    {
                        HandleAudio();
                        anim.Play("mine_side");
                        anim.FlipH = inputDirection.X < 0;
						
                    }
                }
                else
                    anim.Play("idle");
            }
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
	}

	private void HandleMine(float delta)
	{
        Global global = Global.GetInstance();
		if (!global.GetState<bool>("FreeDrill"))
			goto EarlyExit;

		Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");

		if (inputDirection.Angle() % (Mathf.Pi / 2.0) == 0 && inputDirection != Vector2.Zero)
		{
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
                blocked = (shapeCast1.IsColliding() && shapeCast2.IsColliding()) ? true : blocked;

            if (!blocked)
				goto EarlyExit;

			float currentEnergy = global.GetState<float>("CurrentEnergy");
			float miningDrain = global.GetState<float>("MiningDrain");

			if (currentEnergy <= 0.0f)
				goto EarlyExit;

			// Drain energy while mining
			currentEnergy -= miningDrain * delta;
			global.SetState("CurrentEnergy", currentEnergy);	

			TileData tileData = ground.GroundLayer.GetCellTileData(miningTilePosition);
			if (tileData == null)
				goto EarlyExit;

			float tileHealth = ground.TileHealth[miningTilePosition];
			float newTileHealth = tileHealth - miningSpeed * (float)delta;

			if (newTileHealth <= 0.0)
                ground.BreakTile(miningTilePosition);
			else
				ground.TileHealth[miningTilePosition] = newTileHealth;

            if (ground.IsBreakable(miningTilePosition))
                IsMining = true; // Til ANIMATIONS!
				MiningAudio = false;
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

		bool isTouchingWire = false;
		float wireX = 0;

		var areas = WireArea.GetOverlappingAreas();
		foreach (var area in areas)
		{
			if (area.IsInGroup("wire"))
			{
				isTouchingWire = true;
				wireX = area.GlobalPosition.X;
				break;
			}
		}

		if (Input.IsActionPressed("jump"))
		{
			if(!isTouchingWire || Math.Round(GlobalPosition.Y) !< -11)
				velocity.Y -= JumpForce;
		}
		if (Input.IsActionJustReleased("jump") && isTouchingWire)
		{
			velocity.Y = 0;
			Velocity = Vector2.Zero;
			velocity = Vector2.Zero;
			MountWire(wireX);
		}

		// Down through gate
		if (Input.GetAxis("down", "jump") == -1)
			this.SetCollisionMaskValue(6, false);
		else
			this.SetCollisionMaskValue(6, true);
		Velocity = velocity;
		MoveAndSlide();
	}

	private void MountWire(float wireX)
	{
		// Remember to set velocity and Velocity to 0 beforehand
		CurrentState = State.Wire;
		Tween tween = GetTree().CreateTween();
		tween.SetEase(Tween.EaseType.Out);
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(this, "global_position:x", wireX + 1, 0.5f);
	}

	private Vector2 DismountWire(Vector2 velocity, float Amplifier)
	{
		float sidewaysInput = Input.GetAxis("left", "right");
		TimeSinceMountChange = -1;
		this.SetCollisionMaskValue(6, true);
		CurrentState = State.Air;
		velocity += new Vector2(1.0f * sidewaysInput, -0.8f) * DismountBoost*Amplifier;
		return velocity;
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
				JetpackActivated = true;
		}
		
		if (Input.IsActionJustReleased("jump")) {
			JetpackActivated = false;
			
			bool isTouchingWire = false;
			float wireX = 0.0f;

			var areas = WireArea.GetOverlappingAreas();
			foreach (var area in areas)
			{
				if (area.IsInGroup("wire"))
				{
					isTouchingWire = true;
					wireX = area.GlobalPosition.X;
					break;
				}
			}
			if(isTouchingWire)
			{
				Velocity = Vector2.Zero;
				velocity = Vector2.Zero;
				MountWire(wireX);
			}
			
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

		float upwardsInput = Input.GetAxis("up", "down");
		float towards = upwardsInput == 0 ? 0 : MaxWireSpeed * upwardsInput;
		velocity.Y = Mathf.MoveToward(velocity.Y, towards, WireAcceleration * (float)delta);
		if (Input.IsActionJustReleased("jump") && !JetpackActivated) // Dismount
		{
			velocity = DismountWire(velocity,1);
			if (velocity.X == 0)
				velocity.Y = 0;
		}

		if (upwardsInput == 1 && Velocity.Y == 0)
		{
			KnockOffWire++;
			GD.Print(KnockOffWire);
			if(KnockOffWire >= 2)
			{
				velocity = DismountWire(velocity, 0.4f);
			}
		}
		else
		{
			KnockOffWire = 0;
		}

		if (GlobalPosition.Y < -24.5f) // Dismount at top
		{
			velocity += new Vector2(1.0f, -0.5f) * 150.0f;
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

	private void HandleAudio()
	{
		bool ButtonIsHeld = false;
		foreach(string action in MineControls)
		{
			if (Input.IsActionPressed(action))
			{
				ButtonIsHeld = true;
				break;
			}
		}
        
        if (IsMining && ButtonIsHeld)
        {
            // Tjek om lyden allerede spiller
            if (!drilling.Playing)
            {
                drilling.Play();
            }
        }
        else
        {
            if (drilling.Playing)
            {
                drilling.Stop();
            }
        }
    }
}
