using Godot;
using System;

public partial class TurretDoor : Node2D
{
    [Export]
    Interactable Interactable = null;
    public override void _Ready()
    {
        AddUserSignal("ToggleTurret");
        Interactable.Interact += () => {
            EmitSignal("ToggleTurret");
        };
    }
}
