using Godot;
using System;

[GlobalClass]
public partial class ShieldRegenUpgrade : Upgrade
{
    Global global = Global.GetInstance();
    public override void OnBuy(SceneTree tree)
    {
        /*WaveManager waveManager = WaveManager.GetInstance();
        waveManager.WaveStarted += () =>
        {
            float shieldHealth = global.GetState<float>("CurrentShieldHealth");
            shieldHealth += 100f;
            global.SetState("CurrentShieldHealth", shieldHealth);
        };
        */
        int a = global.GetState<int>("RegenAmount")+1;
        global.SetState<int>("RegenAmount", a);
        AmountBought += 1;
    }
}
