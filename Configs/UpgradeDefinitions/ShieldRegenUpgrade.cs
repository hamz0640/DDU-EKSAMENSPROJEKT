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
            float ShieldHealth = global.GetState<float>("CurrentShieldHealth");

            ShieldHealth += 100f;
        };

        AmountBought += 1;
    }
}
