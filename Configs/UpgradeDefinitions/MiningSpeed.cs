using Godot;


[GlobalClass]
public partial class MiningSpeed : Upgrade
{
    public override void OnBuy()
    {
        Global global = Global.GetInstance();
        AmountBought += 1;

        float currentMiningSpeed = global.GetStat<float>("MiningSpeed");
        float newMiningSpeed = currentMiningSpeed * 1.15f;

        global.SetStat("MiningSpeed", newMiningSpeed);
    }
}
