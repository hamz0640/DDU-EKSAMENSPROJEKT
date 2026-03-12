using Godot;
using System;

[GlobalClass]
public partial class MaxInventorySpace : Upgrade
{
    public override void OnBuy()
    {
        Global global = Global.GetInstance();
        AmountBought += 1;

        uint currentMaxInventorySpace = global.GetStat<uint>("MaxInventorySpace");
        uint newMaxInventorySpace = currentMaxInventorySpace + 1;

        global.SetStat("MaxInventorySpace", newMaxInventorySpace);
    }
}
