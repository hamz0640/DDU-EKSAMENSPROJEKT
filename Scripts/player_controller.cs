using Godot;
using System;
using System.Threading;

public partial class player_controller : CharacterBody2D
{
	[Export] public float Speed = 300.0f;
	[Export] public float Acceleration = 10.0f;
	[Export] public float RopeSpeed = 150f;
	[Export] private AnimatedSprite2D animationPlayer;
	private bool isMounted = false;

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}// Gravity

		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (IsOnFloor())
			{
                isMounted = true;
				GD.Print("Player mounted!");
            }
			else
			{
				isMounted = false;
                GD.Print("Player unmounted");
            }
		}// Mounting and unmounting the rope

		Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (inputDirection == Vector2.Left)
		{
			if (IsOnFloor() && !isMounted)
			{
				animationPlayer.FlipH = true;
				if (Math.Abs(velocity.X) < Speed)
				{
                    velocity.X += Acceleration;
                    if(Math.Abs(velocity.X) < Speed / 5)
                        ShowSpecificFrame(animationPlayer, "acceleration", 0);
                    if (Math.Abs(velocity.X) < Speed / 4)
                        ShowSpecificFrame(animationPlayer, "acceleration", 1);
                    if (Math.Abs(velocity.X) < Speed / 3)
                        ShowSpecificFrame(animationPlayer, "acceleration", 2);
                    if (Math.Abs(velocity.X) < Speed / 2)
                        ShowSpecificFrame(animationPlayer, "acceleration", 3);
                    if (Math.Abs(velocity.X) < Speed / 1)
                        ShowSpecificFrame(animationPlayer, "acceleration", 1);
                }
				else
				{
                    animationPlayer.Play("move");
				}
            }
		}
		else if (inputDirection == Vector2.Right)
		{
            if (IsOnFloor() && !isMounted)
            {
                animationPlayer.FlipH = false;
                if (Math.Abs(velocity.X) < Speed)
                {
                    velocity.X += Acceleration;
                    if (Math.Abs(velocity.X) < Speed / 5)
                        ShowSpecificFrame(animationPlayer, "acceleration", 0);
                    if (Math.Abs(velocity.X) < Speed / 4)
                        ShowSpecificFrame(animationPlayer, "acceleration", 1);
                    if (Math.Abs(velocity.X) < Speed / 3)
                        ShowSpecificFrame(animationPlayer, "acceleration", 2);
                    if (Math.Abs(velocity.X) < Speed / 2)
                        ShowSpecificFrame(animationPlayer, "acceleration", 3);
                    if (Math.Abs(velocity.X) < Speed / 1)
                        ShowSpecificFrame(animationPlayer, "acceleration", 1);
                }
                else
                {
                    animationPlayer.Play("move");
                }
            }
        }
		else if (inputDirection == Vector2.Up && isMounted)
		{
			animationPlayer.FlipH=false;
			velocity.Y = RopeSpeed;
			animationPlayer.Play("rope");
		}
		else if (inputDirection == Vector2.Down && isMounted)
		{
			animationPlayer.FlipH = false;
			velocity.Y = -RopeSpeed;
			animationPlayer.Play("rope");
		}
		else
		{
            if (IsOnFloor())
            {
                if (Math.Abs(velocity.X) < Speed)
                {
                    velocity.X += Acceleration;
                    if (Math.Abs(velocity.X) < Speed / 5)
                        ShowSpecificFrame(animationPlayer, "acceleration", 0);
                    if (Math.Abs(velocity.X) < Speed / 4)
                        ShowSpecificFrame(animationPlayer, "acceleration", 1);
                    if (Math.Abs(velocity.X) < Speed / 3)
                        ShowSpecificFrame(animationPlayer, "acceleration", 2);
                    if (Math.Abs(velocity.X) < Speed / 2)
                        ShowSpecificFrame(animationPlayer, "acceleration", 3);
                    if (Math.Abs(velocity.X) < Speed / 1)
                        ShowSpecificFrame(animationPlayer, "acceleration",1);
                }
                velocity.X += -Acceleration;
                animationPlayer.Play("move");
            }
        } // Deacelerate

		if (inputDirection != Vector2.Zero)
		{
			velocity.X = inputDirection.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            animationPlayer.Play("idle");
		}

        Thread.Sleep(1);
		Velocity = velocity;
		MoveAndSlide();
	}
    private static void ShowSpecificFrame(AnimatedSprite2D animationPlayer, string animation, int frame)
    {
        animationPlayer.Animation = animation;
        animationPlayer.Frame = frame;

        animationPlayer.FrameProgress = 0.0f;
        animationPlayer.Stop();
    }
}
