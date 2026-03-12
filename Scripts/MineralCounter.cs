using Godot;
using System;

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
        global.MineralPickedUp += HandlePickup;
    }

    private void HandlePickup(Mineral.MineralType mineralType)
    {
        if (mineralType != ReactOnPickupOf)
            return;

        Global global = (Global)GetTree().Root.GetNode("Global");
        if (global.TotalMineralCount < global.MaxInventorySpace)
        {
            global.MineralCount[mineralType] += 1;
            global.TotalMineralCount += 1;

            int itemCount = Convert.ToInt32(NumberLabel.Text);
            itemCount += 1;
            NumberLabel.Text =  itemCount.ToString();

            Tween rotationTween = GetTree().CreateTween();
            rotationTween.SetParallel(false);
            rotationTween.TweenProperty(this, "rotation", 0.78, 0.1);
            rotationTween.TweenProperty(this, "rotation", -0.78, 0.1);
            rotationTween.TweenProperty(this, "rotation", 0, 0.1);
            rotationTween.SetEase(Tween.EaseType.InOut);
            rotationTween.SetTrans(Tween.TransitionType.Sine);
        } 
        else
        {
            
        }
    }
}
