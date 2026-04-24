using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class UpgradeMenu : MarginContainer
{
    [Export]
    public VBoxContainer UpgradeList = null;
    [Export]
    private Label RedMineralCount = null;
    [Export]
    private Label PurpleMineralCount = null;
    [Export]
    private Label YellowMineralCount = null;
	[Export]
	private Label UpgradeDescription = null;
	[Export]
	private Label UpgradeBuyCondition = null;
	[Export]
	private Label UpgradeNameTitle = null;
    [Export]
    private Button BuyButton = null;

	public int SelectedIndex = 0;


    public override void _Ready()
    {
        Global global = Global.GetInstance();
        Array<Upgrade> upgrades = global.GetState<Array<Upgrade>>("Upgrades");

        string upgradePath = "res://Configs/Upgrades";
        DirAccess upgradesDir = DirAccess.Open(upgradePath);
        foreach (string localUpgradePath in upgradesDir.GetFiles())
        {
            Upgrade upgrade = (Upgrade)GD.Load(upgradePath + "/" + localUpgradePath);
            upgrade.UpgradeName = localUpgradePath.GetBaseName();

            upgrades.Add(upgrade);
        }

        global.SetState("Upgrades", upgrades);

        ShowUpgrades();
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustReleased("up")) SelectedIndex -= 1;
		if (Input.IsActionJustReleased("down")) SelectedIndex += 1;

		SelectedIndex = Mathf.Clamp(SelectedIndex, 0, UpgradeList.GetChildCount() - 1);

		foreach (Node u in UpgradeList.GetChildren())
		{
			UpgradeEntry upgradeEntry = (UpgradeEntry)u;
			upgradeEntry.UpgradeName.AddThemeFontSizeOverride("font_size", 40);
			upgradeEntry.Modulate = new Color(0.8f, 0.8f, 0.8f);
            upgradeEntry.Lock();
		}

		UpgradeEntry selectedUpgrade = (UpgradeEntry)UpgradeList.GetChild(SelectedIndex);
		selectedUpgrade.UpgradeName.AddThemeFontSizeOverride("font_size", 45);
		selectedUpgrade.Modulate = new Color(1, 1, 1);
        selectedUpgrade.Unlock();

		UpgradeNameTitle.Text = selectedUpgrade.RelatedUpgradeResource.UpgradeName;
		UpgradeDescription.Text = selectedUpgrade.RelatedUpgradeResource.Description;
		UpgradeBuyCondition.Text = selectedUpgrade.RelatedUpgradeResource.BuyCondition;

        Global global = Global.GetInstance();

        bool canBuy = selectedUpgrade.RelatedUpgradeResource.CanBuy(GetTree());
        canBuy = !selectedUpgrade.IsLocked;

        uint RedMineralCost = selectedUpgrade.RelatedUpgradeResource.RedMineralAmount;
        uint PurpleMineralCost = selectedUpgrade.RelatedUpgradeResource.PurpleMineralAmount;
        uint YellowMineralCost = selectedUpgrade.RelatedUpgradeResource.YellowMineralAmount;

        if (RedMineralCost > global.GetState<uint>("DepositedRedMineralCount")) canBuy = false;
        if (PurpleMineralCost > global.GetState<uint>("DepositedPurpleMineralCount")) canBuy = false;
        if (YellowMineralCost > global.GetState<uint>("DepositedYellowMineralCount")) canBuy = false;

        if (!canBuy)
            BuyButton.Modulate = new Color(0.2f, 0.2f, 0.2f);
        else
            BuyButton.Modulate = new Color(1.0f, 1.0f, 1.0f);
        
        if (Input.IsActionJustPressed("jump") && canBuy)
        {
            global.ModifyState("DepositedRedMineralCount", -RedMineralCost);
            global.ModifyState("DepositedPurpleMineralCount", -PurpleMineralCost);
            global.ModifyState("DepositedYellowMineralCount", -YellowMineralCost);

            selectedUpgrade.RelatedUpgradeResource.OnBuy(GetTree());
        }

        RedMineralCount.Text = "x" + global.GetState<uint>("DepositedRedMineralCount").ToString();
        PurpleMineralCount.Text = "x" + global.GetState<uint>("DepositedPurpleMineralCount").ToString();
        YellowMineralCount.Text = "x" + global.GetState<uint>("DepositedYellowMineralCount").ToString();
    }


    public void ShowUpgrades()
    {
        Global global = Global.GetInstance();
        Array<Upgrade> upgrades = global.GetState<Array<Upgrade>>("Upgrades");

        foreach (Upgrade upgrade in upgrades)
        {
            PackedScene scene = (PackedScene)GD.Load("res://Scenes/upgrade_entry.tscn");
            UpgradeEntry upgradeEntry = (UpgradeEntry)scene.Instantiate();

            upgradeEntry.UpgradeName.Text = upgrade.UpgradeName;
            if (upgrade.RedMineralAmount == 0) { upgradeEntry.RedMineralIcon.Hide(); } 
            else {upgradeEntry.RedMineralCount.Text = upgrade.RedMineralAmount.ToString(); }
            
            if (upgrade.PurpleMineralAmount == 0) { upgradeEntry.PurpleMineralIcon.Hide(); } 
            else {upgradeEntry.PurpleMineralCount.Text = upgrade.PurpleMineralAmount.ToString(); } 

            if (upgrade.YellowMineralAmount == 0) { upgradeEntry.YellowMineralIcon.Hide(); } 
            else {upgradeEntry.YellowMineralCount.Text = upgrade.YellowMineralAmount.ToString(); }

            UpgradeList.AddChild(upgradeEntry);
            upgradeEntry.RelatedUpgradeResource = upgrade;
        }
    }

    
    public void DecideAvailableStock(uint waveNumber)
    {
        
    }
}
