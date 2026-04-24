using Godot;
using Godot.NativeInterop;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;

public partial class WaveManager : Node
{
    [Signal]
    public delegate void WaveStartedEventHandler();
    [Signal]
    public delegate void WaveEndedEventHandler();
    public uint WaveNumber { get; private set; } = 0;

    public bool IsInWave { get; private set; } = false;

    public static WaveManager GetInstance() {
        return (WaveManager)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/WaveManager");
    }

    public override void _PhysicsProcess(double delta)
    {
        Tracker tracker = Tracker.GetInstance();

        int EnemiesInWorld = GetTree().GetNodeCountInGroup("Enemy");

        if (EnemiesInWorld > 0)
        {
            tracker.IncrementTracking("Time:InWaves", (float)delta);
        }

        if (EnemiesInWorld <= 0)
        {
            IsInWave = false;
            EmitSignal("WaveEnded");
        }
    }

    public void StartWave()
    {
        WaveNumber += 1;
        IsInWave = true;
        EmitSignal("WaveStarted");
    }
}
