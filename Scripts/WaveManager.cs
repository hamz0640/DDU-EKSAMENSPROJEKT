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
    public Wave CurrentWave = null;
    private DirAccess WavesDir = null;

    public static WaveManager GetInstance() {
        return (WaveManager)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/WaveManager");
    }

    public override void _Ready()
    {
        WavesDir = DirAccess.Open("res://Configs/Waves/");
        WavesDir.ListDirBegin();

        WaveTimer = GetTree().CreateTimer(MiningTime, false);
        WaveTimer.Timeout += () =>
        {
            string next = WavesDir.GetNext();
            Wave wave = GD.Load<Wave>("res://Configs/Waves/" + next);
            CurrentWave = wave;

            EmitSignal("WaveStarted", waveNumber);
            GD.Print("New wave has begun");
            waveNumber += 1;
        };
    }

    public override void _PhysicsProcess(double delta)
    {
        if (CurrentWave == null)
        {
            return;
        }

        int EnemiesInWorld = GetTree().GetNodeCountInGroup("Enemy");
        if (WaveTimer.TimeLeft <= 0.0 && CurrentWave.Enemies.Count <= 0 && EnemiesInWorld <= 0)
        {
            GD.Print("Mining time has started");
            WaveTimer = GetTree().CreateTimer(MiningTime, false);

            WaveTimer.Timeout += () =>
            {
                Wave wave = GD.Load<Wave>("res://Configs/Waves/" + WavesDir.GetNext());
                CurrentWave = wave;

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
