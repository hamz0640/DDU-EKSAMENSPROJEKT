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
        // Change to your main game scene
        GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
    }

    private void OnQuitPressed()
    {
        string exePath = OS.GetExecutablePath();

    	Process.Start(new ProcessStartInfo
    {
        FileName = exePath,
        UseShellExecute = true
    });

    	GetTree().Quit();
    }
}