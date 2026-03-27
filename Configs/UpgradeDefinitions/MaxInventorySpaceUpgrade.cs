using Godot;
using System;

[GlobalClass]
public partial class MaxInventorySpaceUpgrade : Upgrade
{
    public override void OnBuy(SceneTree _tree)
    {
        Global global = Global.GetInstance();
        AmountBought += 1;

        uint currentMaxInventorySpace = global.GetState<uint>("MaxInventorySpace");
        uint newMaxInventorySpace = currentMaxInventorySpace + 1;

        global.SetState("MaxInventorySpace", newMaxInventorySpace);
    }
}
