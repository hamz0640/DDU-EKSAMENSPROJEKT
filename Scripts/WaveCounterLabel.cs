using Godot;
using System;

public partial class WaveCounterLabel : Label
{
    public override void _Process(double delta)
    {
        WaveManager waveManager = WaveManager.GetInstance();
        Text = ((int)waveManager.TimeUntilNextWave()).ToString();
    }
}
