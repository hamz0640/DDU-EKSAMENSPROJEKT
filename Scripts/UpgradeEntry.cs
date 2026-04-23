using Godot;
using System;

public partial class UpgradeEntry : PanelContainer
{
    [Export] public Label RedMineralCount = null;
    [Export] public Label PurpleMineralCount = null;
    [Export] public Label YellowMineralCount = null;
    [Export] public TextureRect RedMineralIcon = null;
    [Export] public TextureRect PurpleMineralIcon = null;
    [Export] public TextureRect YellowMineralIcon = null;

    [Export]
    public Label UpgradeName = null;
    [Export]
    public TextureRect Icon = null;
    [Export]
    public Label AmountBought = null;
    public Upgrade RelatedUpgradeResource = null; 


    private void _OnButtonPressed()
    {
        if (!RelatedUpgradeResource.CanBuy(GetTree()))
        {
            GD.Print("Attempted to buy " + UpgradeName.Text + " but it was already maxxed");
            return;
        }

        Global global = Global.GetInstance();
        
        uint DepositedRedMineralCount    = global.GetState<uint>("DepositedRedMineralCount");
        uint DepositedPurpleMineralCount = global.GetState<uint>("DepositedPurpleMineralCount");
        uint DepositedYellowMineralCount = global.GetState<uint>("DepositedYellowMineralCount");

        uint RequiredRedMineralCount    = RelatedUpgradeResource.RedMineralAmount;
        uint RequiredPurpleMineralCount = RelatedUpgradeResource.PurpleMineralAmount;
        uint RequiredYellowMineralCount = RelatedUpgradeResource.YellowMineralAmount;

        if (DepositedRedMineralCount < RequiredRedMineralCount)
        {
            GD.Print("Attempted to buy " + UpgradeName.Text + " but didn't have enough red minerals");
            return;
        }
            
        
        if (DepositedPurpleMineralCount < RequiredPurpleMineralCount)
        {
            GD.Print("Attempted to buy " + UpgradeName.Text + " but didn't have enough purple minerals");
            return;
        }

        if (DepositedYellowMineralCount < RequiredYellowMineralCount)
        {
            GD.Print("Attempted to buy " + UpgradeName.Text + " but didn't have enough yellow minerals");
            return;
        }
        
        GD.Print("Bought " + UpgradeName.Text);
        Tracker tracker = Tracker.GetInstance();
        tracker.IncrementTracking("UpgradesBought:Total", 1u);
        tracker.IncrementTracking("UpgradesBought:" + UpgradeName.Text, 1u);

        RelatedUpgradeResource.OnBuy(GetTree());
        if (RelatedUpgradeResource.MaxBuyAmount == 0)
        {
            AmountBought.Text = RelatedUpgradeResource.AmountBought.ToString() + "/∞";
        } 
        else
        {
            string boughtAmount = RelatedUpgradeResource.AmountBought.ToString();
            string maxBuyAmount = RelatedUpgradeResource.MaxBuyAmount.ToString();
            AmountBought.Text = boughtAmount + "/" + maxBuyAmount;
        }

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
