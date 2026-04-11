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

	enum State { Ground, Air, Wire };

    public override void _PhysicsProcess(double delta)
    {
		HandleTransitions();
		
		switch (CurrentState) {
			case State.Ground:
				HandleGroundMovement((float)delta);
				break;
			case State.Air:
				HandleAirMovement((float)delta);
				break;
			case State.Wire:
				HandleWireMovement((float)delta);
				break;
		}

		TimeSinceMountChange += 1;
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
				tween.TweenProperty(this, "global_position:x", wireX, 0.5f);
			}
		}
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
			velocity.Y = Mathf.MoveToward(Velocity.Y, -MaxJetpackSpeed, JetpackAcceleration * delta);
			currentEnergy -= (1.0f / jetpackEfficiency) * jetpackDrain * delta;
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

		Velocity = velocity;
		MoveAndSlide();
	}
      

	private static void ShowSpecificFrame(AnimatedSprite2D AnimationPlayer, string Animation, int frame) {
		AnimationPlayer.Stop();  // Stop den først
		AnimationPlayer.Animation = Animation;
		AnimationPlayer.Frame = frame;  // Sæt frame bagefter
	}
}
