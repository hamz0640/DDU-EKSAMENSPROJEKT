using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
    public static Global GetInstance() {
        return (Global)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/Global");
    }

    [Signal]
    public delegate void MineralPickedUpEventHandler(Mineral.MineralType mineralType);
    [Signal]
    public delegate void MineralDroppedEventHandler(Mineral.MineralType mineralType);

    public Dictionary<Mineral.MineralType, uint> MineralCount = new();
    private Dictionary<string, Variant> Stats = new();
    public uint TotalMineralCount = 0;


    public override void _Ready()
    {
        MineralCount[Mineral.MineralType.Red]    = 0;
        MineralCount[Mineral.MineralType.Purple] = 0;
        MineralCount[Mineral.MineralType.Yellow] = 0;

        Stats["MaxInventorySpace"] = 10;
        Stats["MiningSpeed"] = 0.5f;
    }


    public T GetStat<[MustBeVariant] T>(string stat)
    {
        return Stats[stat].As<T>();
    }

    public void SetStat<[MustBeVariant] T>(string stat, T value)
    {
        Stats[stat] = Variant.From<T>(value);
    }
}
