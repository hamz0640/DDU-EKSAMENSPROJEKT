using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class WaveManagerConfig : Resource
{
    [Export]
    public Dictionary<PackedScene, float> Weights { get; private set; } = null;
}
