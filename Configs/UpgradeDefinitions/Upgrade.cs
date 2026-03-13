using Godot;
using System;

[GlobalClass]
public abstract partial class Upgrade : Resource
{

    [Export] public uint RedMineralAmount;
    [Export] public uint PurpleMineralAmount;
    [Export] public uint YellowMineralAmount;
    [Export] public uint MaxBuyAmount;
    public uint AmountBought = 0;

    public abstract void OnBuy();
}
