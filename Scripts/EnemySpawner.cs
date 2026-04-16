using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    private uint CurrentWave = 0;
    private SceneTreeTimer TimeUntilNextEnemy = null;
    float TimeBetweenSpawns = 1.0f;
    Random rnd = new Random();
    private int spawnCount = 0;
    public override void _Ready()
    {
        TimeUntilNextEnemy = GetTree().CreateTimer(TimeBetweenSpawns, false);
        WaveManager waveManager = (WaveManager)GetTree().Root.GetNode("WaveManager");
        waveManager.WaveStarted += OnWaveStarted;
    }

    public override void _PhysicsProcess(double delta)
    {

        WaveManager waveManager = WaveManager.GetInstance();
        if (waveManager.CurrentWave == null)
        {
            return;
        }

        if (TimeUntilNextEnemy.TimeLeft <= 0.0 && waveManager.CurrentWave.Enemies.Count > 0)
        {
            TimeUntilNextEnemy = GetTree().CreateTimer(TimeBetweenSpawns, false);
            Enemy enemy = (Enemy)waveManager.CurrentWave.Enemies[0].Instantiate();
            waveManager.CurrentWave.Enemies.RemoveAt(0);

            bool spawnRight = spawnCount % 2 == 0;
            enemy.SetSpawnSide(spawnRight);

            spawnCount++; 

            AddChild(enemy);
            if (rnd.Next(0,5)==4)
            {
                var scene = GD.Load<PackedScene>("res://Scenes/asteroid.tscn");
                Asteroid asteroid = scene.Instantiate<Asteroid>();
                AddChild(asteroid);
                GD.Print("Asteroid spawned");
            }

            if (!spawnRight)
            {
                enemy.GlobalPosition = new Vector2(
                    -500,
                    enemy.GlobalPosition.Y
                );
            }
        }
    }

    public void OnWaveStarted(uint waveNumber)
    {
        CurrentWave = waveNumber;
        spawnCount = 0;
    }
}
