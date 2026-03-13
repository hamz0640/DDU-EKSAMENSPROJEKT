using Godot;
using System;

public partial class UpgradeIcon : MarginContainer
{
    [Export]
    public Label RedMineralCount = null;
    [Export]
    public Label PurpleMineralCount = null;
    [Export]
    public Label YellowMineralCount = null;
    [Export]
    public Label UpgradeName = null;
    [Export]
    public TextureRect Icon = null;
    public Upgrade RelatedUpgradeResource = null; 


    private void _OnButtonPressed()
    {
        GD.Print("Attempt buy");
        Global global = Global.GetInstance();
        
        uint DepositedRedMineralCount    = global.GetState<uint>("DepositedRedMineralCount");
        uint DepositedPurpleMineralCount = global.GetState<uint>("DepositedPurpleMineralCount");
        uint DepositedYellowMineralCount = global.GetState<uint>("DepositedYellowMineralCount");

        uint RequiredRedMineralCount    = RelatedUpgradeResource.RedMineralAmount;
        uint RequiredPurpleMineralCount = RelatedUpgradeResource.PurpleMineralAmount;
        uint RequiredYellowMineralCount = RelatedUpgradeResource.YellowMineralAmount;

        if (DepositedRedMineralCount < RequiredRedMineralCount)
            return;
        
        if (DepositedPurpleMineralCount < RequiredPurpleMineralCount)
            return;

        if (DepositedYellowMineralCount < RequiredYellowMineralCount)
            return;
        
        GD.Print("Bought " + UpgradeName.Text);

        uint NewDepositedRedMineralCount    = DepositedRedMineralCount    - RequiredRedMineralCount;
        uint NewDepositedPurpleMineralCount = DepositedPurpleMineralCount - RequiredPurpleMineralCount;
        uint NewDepositedYellowMineralCount = DepositedYellowMineralCount - RequiredYellowMineralCount; 

        global.SetState("DepositedRedMineralCount",    NewDepositedRedMineralCount);
        global.SetState("DepositedPurpleMineralCount", NewDepositedPurpleMineralCount);
        global.SetState("DepositedYellowMineralCount", NewDepositedYellowMineralCount); 

        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Red,    false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Purple, false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Yellow, false]);
    }
}
