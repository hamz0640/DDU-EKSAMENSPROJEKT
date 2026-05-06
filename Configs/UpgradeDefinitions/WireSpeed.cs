using Godot;
using System;

[GlobalClass]
public partial class WireSpeed : Upgrade
{
    public override void OnBuy(SceneTree tree)
    {
        Global global = Global.GetInstance();
        float currentWireSpeed = global.GetState<float>("WireSpeed");
        global.SetState("WireSpeed", currentWireSpeed * 1.35);
        AmountBought += 1;
    }
}
