using Godot;

public partial class ChargingZone : Area2D
{
    [Export] public float ChargeRate = 5f; 
    
    private bool IsInChargingZone = false; 
    private PlayerController Player = null;
    Sprite2D sprite;
    float MostRecentShieldEnergy;

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

        if (shieldEnergy > 1)
        {
            currentEnergy += ChargeRate * (float)delta;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            global.SetState("CurrentEnergy", currentEnergy);
        }


        if (shieldEnergy < MostRecentShieldEnergy && shieldEnergy > 40) // Hvis ramt, pulsér
            PulseShield(sprite.Modulate.A);

        switch (shieldEnergy) // Ranges from 0 - 200
        {
            case >= 50:
                // Gradvis synlighed basseret på dens energi
                sprite.Visible = true;
                Color tempColor = sprite.Modulate;
                tempColor.A = (float)((((float)shieldEnergy / 2) / 100));
                if (tempColor.A > 1)
                    tempColor.A = 1;
                sprite.Modulate = tempColor;
                break;
            case >= 1:
                // Pulsering for at indikere danger eller sådan noget
                sprite.Visible = true;
                Color tempColorT = sprite.Modulate;
                float rawSin = Mathf.Sin(Time.GetTicksMsec() * 0.01f);
                tempColorT.A = Mathf.Remap(rawSin, -1.0f, 1.0f, 0.2f, 0.3f);
                sprite.Modulate = tempColorT;
                break;
            case <= 0:
                sprite.Visible = false;
                break;
        }
        MostRecentShieldEnergy = shieldEnergy;
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

    public void PulseShield(float OriginalAValue)
    {
        Tween tween = GetTree().CreateTween();

        // Stærk farve ændring
        sprite.Modulate = new Color(1.5f, 1.5f, 1.5f, OriginalAValue);
        // Lav en overgang tilbage til normal
        tween.TweenProperty(sprite, "modulate", new Color(1, 1, 1, OriginalAValue), 0.1f)
             .SetTrans(Tween.TransitionType.Expo)
             .SetEase(Tween.EaseType.Out);
    }
}