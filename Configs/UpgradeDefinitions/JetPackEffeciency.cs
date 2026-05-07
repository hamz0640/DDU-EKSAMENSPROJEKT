using Godot;
using System;

[GlobalClass]
public partial class JetPackEffeciency : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        float newvalue = global.GetState<float>("JetpackEffeciency") * 1.15f;
        global.SetState("JetpackEffeciency", newvalue);
        AmountBought += 1;
    }
}
