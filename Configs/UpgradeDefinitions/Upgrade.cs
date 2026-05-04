using Godot;
using System;

[GlobalClass]
public abstract partial class Upgrade : Resource
{

    [Export] public uint RedMineralAmount;
    [Export] public uint PurpleMineralAmount;
    [Export] public uint YellowMineralAmount;
    [Export] public uint MaxBuyAmount;
    [Export] public string Description = "Not Set";
    [Export] public string BuyCondition = "None";
    [Export] public Texture2D UpgradeIcon = null;

    public uint AmountBought = 0;
    public string UpgradeName = "Not Set";

    public abstract void OnBuy(SceneTree tree);

    public virtual bool CanBuy(SceneTree tree)
    {
        return AmountBought < MaxBuyAmount || MaxBuyAmount == 0;
    }
}
