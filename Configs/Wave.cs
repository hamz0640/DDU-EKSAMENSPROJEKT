using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Wave : Resource
{
    [Export]
    public Array<PackedScene> Enemies;
}
