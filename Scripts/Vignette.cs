using Godot;
using System;

public partial class Vignette : Sprite2D
{
    [Export]
    public float BlinkSpeed = 20;
    public override void _Process(double delta)
    {
        Global global = Global.GetInstance();
        float currentEnergy = global.GetState<float>("CurrentEnergy");
        float maxEnergy     = global.GetState<float>("MaxEnergy");

        float fraction = currentEnergy / maxEnergy;

        if (fraction > 0.5f)
        {
            Modulate = new Color(0f, 0f, 0f, 0f);
        }
        else if (fraction < 0.5f && fraction > 0.25f)
        {
            Modulate = new Color(1, 0.75f, 0.75f, 0.5f * Mathf.Sin(Engine.GetPhysicsFrames() / BlinkSpeed));
        }
        else if (fraction < 0.25f && fraction > 0.1f)
        {
            Modulate = new Color(1, 0.75f, 0.75f, 1.0f * Mathf.Sin(Engine.GetPhysicsFrames() / BlinkSpeed));
        }
        else if (fraction < 0.1f && fraction > 0.0f)
        {
            Modulate = new Color(1, 0.75f, 0.75f, 2.0f * Mathf.Sin(Engine.GetPhysicsFrames() / BlinkSpeed));
        }
    }
}
