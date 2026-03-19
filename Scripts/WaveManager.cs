using Godot;
using System;

public partial class WaveManager : Node
{
    [Signal]
    public delegate void WaveStartedEventHandler(uint waveNumber);
    private SceneTreeTimer WaveTimer;
    [Export]
    public float MiningTime = 60.0f;
    private uint waveNumber = 0;

    public override void _Ready()
    {
        WaveTimer = GetTree().CreateTimer(MiningTime, false);
        WaveTimer.Timeout += () =>
        {
            EmitSignal("WaveStarted", waveNumber);
            GD.Print("New wave has begun");
            waveNumber += 1;
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (WaveTimer.TimeLeft <= 0.0 && GetTree().GetNodesInGroup("Enemy").Count == 0)
        {
            GD.Print("Mining time has started");
            WaveTimer = GetTree().CreateTimer(MiningTime, false);

            WaveTimer.Timeout += () =>
            {
                EmitSignal("WaveStarted", waveNumber);
                GD.Print("New wave has begun");
                waveNumber += 1;
            };
        }
    }


    public float TimeUntilNextWave()
    {
        return (float)WaveTimer.TimeLeft;
    }

    public float WaveNumber()
    {
        return waveNumber;
    }
}
