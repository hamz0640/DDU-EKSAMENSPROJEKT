using Godot;
using System;

public partial class Wavecounter : Label
{
    public override void _Ready()
    {
        var waveManager = WaveManager.GetInstance();

        // Connect to the signal
        waveManager.WaveStarted += OnWaveStarted;

        // Set initial text
        Text = "Wave: " + waveManager.WaveNumber;
    }

    private void OnWaveStarted()
    {
        var waveManager = WaveManager.GetInstance();
        Text = "Wave: " + waveManager.WaveNumber;
    }
}