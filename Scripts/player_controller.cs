using Godot;
using System;
using System.Security.AccessControl;
using System.Threading;

public partial class player_controller : CharacterBody2D
{
	[Export] public float Speed = 220.0f;
	[Export] public float Acceleration = 400.0f;
    [Export] public float Deceleration = 600.0f;
	[Export] public float RopeSpeed = 150.0f;
	[Export] private AnimatedSprite2D animationPlayer;
    private bool isMounted = false;
    private bool IsWantedFlip = false;

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
                if(Mathf.Abs(velocity.X) < Speed)
                    velocity.X -= Acceleration * (float)delta;
                if(velocity.X != 0)
                    IsWantedFlip = true;
            }
            else
                isMounted = false;
		}
		else if (inputDirection == Vector2.Right)
		{
            if (IsOnFloor() && !isMounted)
            {
                if (velocity.X < Speed)
                    velocity.X += Acceleration * (float)delta;
                if (velocity.X != 0)
                    IsWantedFlip = false;
            }
            else
                isMounted = false;
        }
		else if (inputDirection == Vector2.Up && isMounted)
		{
			animationPlayer.FlipH = false;
			velocity.Y = -RopeSpeed;
			animationPlayer.Play("rope");
		}
		else if (inputDirection == Vector2.Down && isMounted)
		{
			animationPlayer.FlipH = false;
			velocity.Y = RopeSpeed;
			animationPlayer.Play("rope");
		}

        //Mathf.MoveToward();

        if (Math.Abs(velocity.X) < Speed) // Animation
        {
            if (Math.Abs(velocity.X) != 0 && Math.Abs(velocity.X) < Speed / 5)
            {
                ShowSpecificFrame(animationPlayer, "acceleration", 0);
                animationPlayer.FlipH = IsWantedFlip;
            }
            else if (Math.Abs(velocity.X) >= Speed / 5 && Math.Abs(velocity.X) < Speed / 4)
            {
                ShowSpecificFrame(animationPlayer, "acceleration", 1);
            }
            else if (Math.Abs(velocity.X) >= Speed / 4 && Math.Abs(velocity.X) < Speed / 3)
            {
                ShowSpecificFrame(animationPlayer, "acceleration", 2);
            }
            else if (Math.Abs(velocity.X) >= Speed / 3 && Math.Abs(velocity.X) < Speed / 2)
            {
                ShowSpecificFrame(animationPlayer, "acceleration", 2);
            }
            else if (Math.Abs(velocity.X) > Speed / 2 && Math.Abs(velocity.X) < Speed)
                ShowSpecificFrame(animationPlayer, "acceleration", 3);
        }
        else if (Math.Abs(velocity.X) >= Speed)
        {
            animationPlayer.Play("move");
        } 
        
        if (velocity.X == 0)
        {
            animationPlayer.Play("idle");
            animationPlayer.FlipH = false;
        }
        if (inputDirection == Vector2.Zero && velocity.X != 0)
        {
            if(velocity.X < 0)
                velocity.X += Deceleration * (float)delta;
            if (velocity.X > 0)
                velocity.X -= Deceleration * (float)delta;
        }

        Velocity = velocity;
		MoveAndSlide();
	}
    private static void ShowSpecificFrame(AnimatedSprite2D animationPlayer, string animation, int frame)
    {
        animationPlayer.Stop(); // Stop den først
        animationPlayer.Animation = animation;
        animationPlayer.Frame = frame; // Sæt frame bagefter

        animationPlayer.FrameProgress = 0.0f;
    }
}
