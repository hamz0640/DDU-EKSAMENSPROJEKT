using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class UpgradeEntry : PanelContainer
{
    [Export] public Label RedMineralCount = null;
    [Export] public Label PurpleMineralCount = null;
    [Export] public Label YellowMineralCount = null;
    [Export] public TextureRect RedMineralIcon = null;
    [Export] public TextureRect PurpleMineralIcon = null;
    [Export] public TextureRect YellowMineralIcon = null;
    [Export] public Button ClickDetect = null;


    [Export]
    public Label UpgradeName = null;
    [Export]
    public TextureRect Icon = null;
    [Export]
    public Label AmountBought = null;
    public Upgrade RelatedUpgradeResource = null; 
    public bool IsLocked {get; private set; } = true;

    public void Lock()
    {
        Texture2D lockedImage = GD.Load<Texture2D>("res://Assets/Icons/LockedPadlock.png");
        Icon.Texture = lockedImage;
        Icon.Modulate = new Color(0.8f, 0.8f, 0.8f);
    }

    public void Unlock()
    {
        Texture2D unlockedImage = GD.Load<Texture2D>("res://Assets/Icons/UnlockedPadlock.png");
        Icon.Texture = unlockedImage;
        Icon.Modulate = new Color(1.2f, 1.2f, 1.2f);
    }
}
