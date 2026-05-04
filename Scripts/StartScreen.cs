using Godot;
using System;
using System.Diagnostics;

public partial class StartScreen : Control
{
    AudioStreamPlayer2D select;
    bool AudioDelay = true;
    public override void _Ready()
    {
        select = GetNode<AudioStreamPlayer2D>("Select");
        var startButton = GetNode<Button>("CenterContainer/VBoxContainer/StartButton");
        var quitButton = GetNode<Button>("CenterContainer/VBoxContainer/QuitButton");

        startButton.Pressed += OnStartPressed;
        quitButton.Pressed += OnQuitPressed;
        
    }

    private void OnStartPressed()
    {
        select.Play();
        select.Connect(AudioStreamPlayer2D.SignalName.Finished, 
            Callable.From(() => { GetTree().ChangeSceneToFile("res://Scenes/main.tscn"); }), 
            (uint)ConnectFlags.OneShot);
        
        
    }

    private void OnQuitPressed()
    {
        select.Play();
        select.Connect(AudioStreamPlayer2D.SignalName.Finished,
           Callable.From(() => { GetTree().Quit(); }),
           (uint)ConnectFlags.OneShot);
        
    }

}