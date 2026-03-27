using Godot;

public partial class ChargingZone : Area2D
{
    [Export] public float ChargeRate = 5f; 
    
    private bool IsInChargingZone = false; 
    private PlayerController Player = null;
    Sprite2D sprite;

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite2D");
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
        float shieldEnergy = global.GetState<float>("ShieldHealth");
        
        currentEnergy += ChargeRate * (float)delta;
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

        global.SetState("CurrentEnergy", currentEnergy);

        if (shieldEnergy <= 0)
        {
            sprite.Visible = false;
        }

        else
        {
            sprite.Visible = true;
        }
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