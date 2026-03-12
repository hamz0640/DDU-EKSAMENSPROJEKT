using Godot;

public partial class ChargingZone : Area2D
{
    [Export] public float ChargeRate = 5f; 

    private PlayerController Player = null;
    private EnergyBar EnergyBar = null;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        EnergyBar = GetTree().GetFirstNodeInGroup("energy_bar") as EnergyBar;

        if (EnergyBar == null)
            GD.PrintErr("ChargingZone shi not found in group ");
    }

    public override void _Process(double delta)
    {
        if (Player != null && EnergyBar != null)
        {
            EnergyBar.AddEnergy(ChargeRate * (float)delta);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController player)
            Player = player;
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is PlayerController player && Player == player)
            Player = null;
    }
}