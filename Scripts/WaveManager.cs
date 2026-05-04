using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WaveManager : Node
{
    [Signal]
    public delegate void WaveStartedEventHandler();
    [Signal]
    public delegate void WaveEndedEventHandler();
    public uint WaveNumber { get; private set; } = 0;

    public bool IsInWave { get; private set; } = false;

    private SceneTreeTimer SpawnTimer = null;
    private uint SpawnIndex = 0;
    private float TimeBetweenSpawns = 1.0f;
    [Export]
    private Godot.Collections.Dictionary<PackedScene, float> Weights = null;

	private Random rand = new Random(0);

    public static WaveManager GetInstance() {
        return (WaveManager)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/WaveManager");
    }


    public override void _Ready()
    {
        WaveManagerConfig config = GD.Load<WaveManagerConfig>("res://configs/WaveManagerConfig.tres");
		Weights = config.Weights;
    }


    public override void _PhysicsProcess(double delta)
    {
        Tracker tracker = Tracker.GetInstance();

        int EnemiesInWorld = GetTree().GetNodeCountInGroup("Enemy");

        if (EnemiesInWorld > 0)
        {
            tracker.IncrementTracking("Time:InWaves", (float)delta);
        }

        if (EnemiesInWorld <= 0 && IsInWave)
        {
            IsInWave = false;
            EmitSignal("WaveEnded");
			GD.Print("Wave Ended");
        }
    }

    public void StartWave()
    {
        if (IsInWave) return;

        WaveNumber += 1;
        IsInWave = true;
        SpawnIndex = 0;
        EmitSignal("WaveStarted");

		GD.Print("Wave " + WaveNumber +  " Started");
        SpawnTimer = GetTree().CreateTimer(TimeBetweenSpawns);
		SpawnTimer.Timeout += SpawnNextEnemy;
        SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        uint maxEnemies = (uint)Mathf.Pow(WaveNumber, 1.3f);
        if (SpawnIndex > maxEnemies)
		{
			return;
		}
        else
		{
			SpawnIndex += 1;
			SpawnTimer = GetTree().CreateTimer(TimeBetweenSpawns);
			SpawnTimer.Timeout += SpawnNextEnemy;
		}
        
		float summedWeights = 0.0f;
        foreach (float weight in Weights.Values)
            summedWeights += weight;

        float value = rand.NextSingle() * summedWeights;

		float total = 0.0f;
		foreach ((PackedScene scene, float weight) in Weights)
		{
			if (value > (total + weight))
			{
				total += weight;
				continue;
			}

			SpawnEnemy(scene);
			break;
		}
    }

	private void SpawnEnemy(PackedScene scene)
	{
		Node node = scene.Instantiate();
		if (node is Enemy)
		{
			Enemy enemy = (Enemy)GD.Load<PackedScene>("res://Scenes/enemy.tscn").Instantiate();
			GetTree().Root.AddChild(enemy);
            if (SpawnIndex % 2 == 0)
            {
                enemy.GlobalPosition = new Vector2(1000, -15);
                enemy.SetSpawnSide(true);
            }
            else
                enemy.GlobalPosition = new Vector2(-500, -15);
		}
		
		if (node is Asteroid)
		{
			GetTree().Root.AddChild(scene.Instantiate());
		}
	}
}
