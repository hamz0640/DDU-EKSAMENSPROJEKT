using Godot;
using System;

[GlobalClass]
public abstract partial class Upgrade : Resource
{

    [Export] public uint RedCrystalAmount;
    [Export] public uint PurpleCrystalAmount;
    [Export] public uint YellowCrystalAmount;
    [Export] public uint MaxBuyAmount;
    public uint AmountBought = 0;

    public abstract void OnBuy();
}
