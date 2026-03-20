using Godot;
using System;
using System.Net.NetworkInformation;

public partial class MineralCounter : Node
{
    [Export]
    public TextureRect MineralImage = null;
    [Export]
    public Label NumberLabel = null;

    [Export]
    public Mineral.MineralType ReactOnPickupOf;


    public override void _Ready()
    {
        Global global = (Global)GetTree().Root.GetNode("Global");
        global.MineralCountUpdated += HandleUpdate;
    }

    private void HandleUpdate(Mineral.MineralType mineralType, bool pickedUp)
    {
        if (mineralType != ReactOnPickupOf)
            return;

        Global global = (Global)GetTree().Root.GetNode("Global");
        uint totalMineralCount = global.GetState<uint>("TotalMineralCount");
        uint maxInventorySpace = global.GetStat<uint>("MaxInventorySpace");

        if (totalMineralCount >= maxInventorySpace && pickedUp == true)
            return;

        uint mineralCount = 0;
        uint add = pickedUp ? 1u : 0u;

        switch (mineralType)
        {
            case Mineral.MineralType.Red:
                uint redMineralCount = global.GetState<uint>("RedMineralCount");
                mineralCount = redMineralCount + add;

                global.SetState("RedMineralCount",   redMineralCount   + add);
                global.SetState("TotalMineralCount", totalMineralCount + add);
                break;
            case Mineral.MineralType.Purple:
                uint purpleMineralCount = global.GetState<uint>("PurpleMineralCount");
                mineralCount = purpleMineralCount + add;
                
                global.SetState("PurpleMineralCount", purpleMineralCount + add);
                global.SetState("TotalMineralCount",  totalMineralCount  + add);
                break;
            case Mineral.MineralType.Yellow:
                uint yellowMineralCount = global.GetState<uint>("YellowMineralCount");
                mineralCount = yellowMineralCount + add;

                global.SetState("YellowMineralCount", yellowMineralCount + add);
                global.SetState("TotalMineralCount",  totalMineralCount  + add);
                break;
        }

        NumberLabel.Text =  mineralCount.ToString();

        Tween rotationTween = GetTree().CreateTween();
        rotationTween.SetParallel(false);
        rotationTween.TweenProperty(this, "rotation", 0.78, 0.1);
        rotationTween.TweenProperty(this, "rotation", -0.78, 0.1);
        rotationTween.TweenProperty(this, "rotation", 0, 0.1);
        rotationTween.SetEase(Tween.EaseType.InOut);
        rotationTween.SetTrans(Tween.TransitionType.Sine);
    }
}
