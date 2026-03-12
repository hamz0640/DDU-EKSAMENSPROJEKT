using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
    [Signal]
    public delegate void MineralPickedUpEventHandler(Mineral.MineralType mineralType);
    [Signal]
    public delegate void MineralDroppedEventHandler(Mineral.MineralType mineralType);

    public Dictionary<Mineral.MineralType, uint> MineralCount = new();
    public uint TotalMineralCount = 0;
    public uint MaxInventorySpace = 10;


    public override void _Ready()
    {
        MineralCount[Mineral.MineralType.Red]    = 0;
        MineralCount[Mineral.MineralType.Purple] = 0;
        MineralCount[Mineral.MineralType.Yellow] = 0;
    }
}
