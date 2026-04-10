using Godot;
using System;

[GlobalClass]
public partial class ShieldRegenUpgrade : Upgrade
{
    public override void OnBuy(SceneTree tree)
    {
        WaveManager waveManager = WaveManager.GetInstance();
        waveManager.WaveStarted += (_) =>
        {
            Global global = Global.GetInstance();
            float ShieldHealth = global.GetState<float>("ShieldHealth");

            ShieldHealth += 100f;
        };
    }
}
