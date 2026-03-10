using Godot;
using System;

public partial class Global : Node
{
    [Signal]
    public delegate void MineralPickedUpEventHandler(Mineral.MineralType mineralType);
    [Signal]
    public delegate void MineralDroppedEventHandler(Mineral.MineralType mineralType);
}
