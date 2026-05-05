using Godot;
using System;

[GlobalClass]
public partial class ShieldRegenUpgrade : Upgrade
{
    public override void OnBuy(SceneTree tree)
    {
        WaveManager waveManager = WaveManager.GetInstance();
        waveManager.WaveStarted += () =>
        {
            Global global = Global.GetInstance();
            float shieldHealth = global.GetState<float>("CurrentShieldHealth");
            shieldHealth += 100f;
            global.SetState("CurrentShieldHealth", shieldHealth);
        };

        AmountBought += 1;
    }
}
