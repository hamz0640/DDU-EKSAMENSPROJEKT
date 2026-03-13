using Godot;
using System;

public partial class UpgradeMenu : MarginContainer
{
    [Export]
    public HFlowContainer HFlowContainer = null;
    [Export]
    private Label RedMineralCount = null;
    [Export]
    private Label PurpleMineralCount = null;
    [Export]
    private Label YellowMineralCount = null;

    public override void _Ready()
    {
        Global global = (Global)GetTree().Root.GetNode("Global");
        global.MineralCountUpdated += HandleUpdate;
    }


    private void HandleUpdate(Mineral.MineralType mineralType, bool _)
    {
        Global global = (Global)GetTree().Root.GetNode("Global");

        switch (mineralType)
        {
            case Mineral.MineralType.Red:
                uint redMineralCount = global.GetState<uint>("DepositedRedMineralCount");
                RedMineralCount.Text = redMineralCount.ToString();
                break;
            case Mineral.MineralType.Purple:
                uint purpleMineralCount = global.GetState<uint>("DepositedPurpleMineralCount");
                PurpleMineralCount.Text = purpleMineralCount.ToString();
                break;
            case Mineral.MineralType.Yellow:
                uint yellowMineralCount = global.GetState<uint>("DepositedYellowMineralCount");
                YellowMineralCount.Text = yellowMineralCount.ToString();
                break;
        }
    }
}
