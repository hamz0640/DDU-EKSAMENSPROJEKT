using Godot;

public partial class ChargingZone : Area2D
{
    [Export] public float ChargeRate = 5f; 
    
    private bool IsInChargingZone = false; 
    private PlayerController Player = null;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (!IsInChargingZone)
        {
            return;
        }

        Global global = Global.GetInstance();
        
        float currentEnergy = global.GetState<float>("CurrentEnergy"); 
        float maxEnergy     = global.GetState<float>("MaxEnergy");
        
        currentEnergy += ChargeRate * (float)delta;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        global.SetState("CurrentEnergy", currentEnergy);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is PlayerController)
        {
            IsInChargingZone = true;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is PlayerController) {
            IsInChargingZone = false;
        }
    }
}