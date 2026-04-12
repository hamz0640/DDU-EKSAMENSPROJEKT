using Godot;
using System;

public partial class TurretDoor : Node2D
{
    [Export]
    Interactable Interactable = null;
    [Signal]
    public delegate void ToggleTurretEventHandler();
    public override void _Ready()
    {
        Interactable.Interact += () => {
            EmitSignal(SignalName.ToggleTurret);
        };
    }
}
