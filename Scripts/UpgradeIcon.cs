using Godot;
using System;

public partial class UpgradeIcon : MarginContainer
{
    [Export]
    public Label RedMineralCount = null;
    [Export]
    public Label PurpleMineralCount = null;
    [Export]
    public Label YellowMineralCount = null;
    [Export]
    public Label UpgradeName = null;
    [Export]
    public TextureRect Icon = null;
}
