using Godot;
using System;

[GlobalClass]
public partial class FreeDrill : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        AmountBought += 1;

        global.SetState("FreeDrill", true);
    }
}
