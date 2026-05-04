using Godot;
using System;

public partial class SwoopMineral : CharacterBody2D
{
    private bool ShouldDisappear = false;
    public override void _PhysicsProcess(double delta)
    {
        Rotation += 4 * Mathf.Pi * (float)delta;
        MoveAndSlide();

        if (GlobalPosition.DistanceTo(new Vector2(156.0f, -25.0f)) < 15.0f)
        {
            ShouldDisappear = true;
        }

        if (ShouldDisappear)
        {
            ((Sprite2D)GetChild(0)).Scale *= 0.85f;;;;;
            Global global = Global.GetInstance();
        }

        if (((Sprite2D)GetChild(0)).Scale.Length() < 0.001f)
            QueueFree(); 
    }

    public void SetMineralType(Mineral.MineralType mineralType)
    {
        Sprite2D sprite = new Sprite2D();
        sprite.Position += new Vector2(5, 5);
        sprite.Scale = new Vector2(0.25f, 0.25f);
        AddChild(sprite);

        if (mineralType == Mineral.MineralType.Red)
            sprite.Texture = GD.Load<Texture2D>("res://Assets/Icons/block-out-red-mineral.png");
        if (mineralType == Mineral.MineralType.Purple)
            sprite.Texture = GD.Load<Texture2D>("res://Assets/Icons/block-out-purple-mineral.png");
        if (mineralType == Mineral.MineralType.Yellow)
            sprite.Texture = GD.Load<Texture2D>("res://Assets/Icons/block-out-yellow-mineral.png");
    }
}
