using Godot;
using System;
using System.Diagnostics;

public partial class StartScreen : Control
{
    public override void _Ready()
    {
        var startButton = GetNode<Button>("CenterContainer/VBoxContainer/StartButton");
        var quitButton = GetNode<Button>("CenterContainer/VBoxContainer/QuitButton");

        startButton.Pressed += OnStartPressed;
        quitButton.Pressed += OnQuitPressed;
    }

    private void OnStartPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
        WaveManager.GetInstance().StartWave();
    }

    private void OnQuitPressed()
    {
    	GetTree().Quit();
    }
}