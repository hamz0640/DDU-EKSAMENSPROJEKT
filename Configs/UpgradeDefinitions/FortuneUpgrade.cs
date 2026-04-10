using Godot;
using System;

[GlobalClass]
public partial class FortuneUpgrade : Upgrade
{
    public override void OnBuy(SceneTree tree)
    {
        Global global = Global.GetInstance();
        float fortune = global.GetState<float>("Fortune");
        global.SetState("Fortune", fortune + 0.1f);

        AmountBought += 1;
    }
}
