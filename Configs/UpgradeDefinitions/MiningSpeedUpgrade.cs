using Godot;


[GlobalClass]
public partial class MiningSpeedUpgrade : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        AmountBought += 1;

        float currentMiningSpeed = global.GetState<float>("MiningSpeed");
        float newMiningSpeed = currentMiningSpeed * 1.15f;

        global.SetState("MiningSpeed", newMiningSpeed);
        RedMineralAmount = RedMineralAmount+2;
        YellowMineralAmount++;
        PurpleMineralAmount++;

    }
}
