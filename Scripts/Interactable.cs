using Godot;
using Godot.Collections;
using System;


[GlobalClass]
public partial class Interactable : Area2D
{
    [Signal]
    public delegate void InteractEventHandler();
    [Export]
    string InputActionName = "interact";
    private bool IsPlayerInInteractableArea = false;


    public override void _Ready()
    {
        BodyEntered += (body) =>
        {
            if (body is PlayerController) IsPlayerInInteractableArea = true;
        };

        BodyExited += (body) =>
        {
            if (body is PlayerController) IsPlayerInInteractableArea = false;
        };
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!(Input.IsActionJustPressed(InputActionName) && IsPlayerInInteractableArea))
            return;
        
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");
        Vector2 cameraPosition = camera.GlobalPosition;
        
        Array<Node> interactables = GetTree().GetNodesInGroup("Interactable");
        Node2D closestInteractable = this;

        for (int i = 0; i < interactables.Count; i++)
        {
            Node2D currentInteractable = (Node2D)interactables[i];
            float currentInteractableDistance = cameraPosition.DistanceTo(currentInteractable.GlobalPosition);
            float closestInteractableDistance = cameraPosition.DistanceTo(closestInteractable.GlobalPosition);

            if (currentInteractableDistance < closestInteractableDistance)
                closestInteractable = currentInteractable;
        }

        if (closestInteractable == this)
            EmitSignal(SignalName.Interact);
    }
}
