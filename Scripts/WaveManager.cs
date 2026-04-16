using Godot;
using Godot.NativeInterop;
using System;
using System.Linq;

public partial class WaveManager : Node
{
    [Signal]
    public delegate void WaveStartedEventHandler(uint waveNumber);
    private SceneTreeTimer WaveTimer;
    [Export]
    public float MiningTime = 60.0f;
    public uint waveNumber { get; private set; } = 0;
    public Wave CurrentWave = null;
    private DirAccess WavesDir = null;
    private string[] waves = null;

    public static WaveManager GetInstance() {
        return (WaveManager)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/WaveManager");
    }

    public override void _Ready()
    {   
        WavesDir = DirAccess.Open("res://Configs/Waves/");
        waves = WavesDir.GetFiles();
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
            WaveTimer = GetTree().CreateTimer(MiningTime, false);
            WaveTimer.Timeout += () =>
            {
                string next = waves[waveNumber];
                Wave wave = GD.Load<Wave>("res://Configs/Waves/" + next);
                CurrentWave = wave;

                EmitSignal("WaveStarted", waveNumber);
                GD.Print("Wave " + waveNumber + " started");
                
                waveNumber += 1;
                waveNumber = Math.Clamp(waveNumber, 0, (uint)waves.Length - 1);
            };
        }
    }


    public float TimeUntilNextWave()
    {
        return (float)WaveTimer.TimeLeft;
    }

    public void StartWaves()
    {
        waveNumber = 0;
        StartNextWaveTimer();
    }

    public void StartNextWaveTimer()
    {
        WaveTimer = GetTree().CreateTimer(MiningTime, false);
        WaveTimer.Timeout += OnWaveTimerTimeout;
    }
    private void OnWaveTimerTimeout()
    {
        string next = waves[waveNumber];
        Wave wave = GD.Load<Wave>("res://Configs/Waves/" + next);
        CurrentWave = wave;

        EmitSignal("WaveStarted", waveNumber);
        GD.Print("Wave " + waveNumber + " started");
        
        waveNumber += 1;
        waveNumber = Math.Clamp(waveNumber, 0, (uint)waves.Length - 1);
    }
}
