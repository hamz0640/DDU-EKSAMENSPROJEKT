using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public partial class UpgradeMenu : MarginContainer
{
    [Export]
    public HBoxContainer HBoxContainer = null;
    [Export]
    public HBoxContainer ShieldHealth = null;
    [Export]
    private Label RedMineralCount = null;
    [Export]
    private Label PurpleMineralCount = null;
    [Export]
    private Label YellowMineralCount = null;
    private Random random = new();

    public override void _Ready()
    {
        Global global = Global.GetInstance();
        global.MineralCountUpdated += HandleUpdate;

        WaveManager waveManager = WaveManager.GetInstance();
        waveManager.WaveStarted += HandleShopChange;
    }


    private void HandleUpdate(Mineral.MineralType mineralType, bool _)
    {
        Global global = (Global)GetTree().Root.GetNode("Global");

        switch (mineralType)
        {
            case Mineral.MineralType.Red:
                uint redMineralCount = global.GetState<uint>("DepositedRedMineralCount");
                RedMineralCount.Text = redMineralCount.ToString();
                break;
            case Mineral.MineralType.Purple:
                uint purpleMineralCount = global.GetState<uint>("DepositedPurpleMineralCount");
                PurpleMineralCount.Text = purpleMineralCount.ToString();
                break;
            case Mineral.MineralType.Yellow:
                uint yellowMineralCount = global.GetState<uint>("DepositedYellowMineralCount");
                YellowMineralCount.Text = yellowMineralCount.ToString();
                break;
        }
    }

    public void HandleShopChange(uint waveNumber)
    {
        Global global = Global.GetInstance();
        Array<Upgrade> upgrades = global.GetState<Array<Upgrade>>("Upgrades");

        foreach (Upgrade upgrade in upgrades)
        {
            // Could be a problem, if the ShieldHealth upgrade ever changes it's
            // name (Possibly to include a space?)
            if (upgrade.UpgradeName == "Shield Health" && ShieldHealth.GetChildCount() == 0)
            {
                PackedScene scene = (PackedScene)GD.Load("res://Scenes/upgrade_icon.tscn");
                UpgradeIcon upgradeIcon = (UpgradeIcon)scene.Instantiate();

                upgradeIcon.UpgradeName.Text = upgrade.UpgradeName;
                upgradeIcon.RedMineralCount.Text    = upgrade.RedMineralAmount.ToString();
                upgradeIcon.PurpleMineralCount.Text = upgrade.PurpleMineralAmount.ToString();
                upgradeIcon.YellowMineralCount.Text = upgrade.YellowMineralAmount.ToString();
                upgradeIcon.AmountBought.Text = "0/∞";

                upgradeIcon.RelatedUpgradeResource = upgrade;
                ShieldHealth.AddChild(upgradeIcon);
            }
        }
        
        for (int i = 0; i < HBoxContainer.GetChildCount(); i++)
        {
            HBoxContainer.GetChild(i).QueueFree();
        }

        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            int upgradeIndex = random.Next(0, Mathf.Max(3, upgrades.Count));
            // Check to ensure that ShieldHealth isn't picked as one of the
            // random upgrades, as that would feel not good :c
            if (upgrades[upgradeIndex].UpgradeName == "Shield Health")
                selectedIndices.Add(upgradeIndex);

            while (selectedIndices.Contains(upgradeIndex))
            {
                upgradeIndex = random.Next(0, Mathf.Max(3, upgrades.Count));

                // Check to ensure that ShieldHealth isn't picked as one of the
                // random upgrades, as that would feel not good :c
                if (upgrades[upgradeIndex].UpgradeName == "Shield Health")
                    selectedIndices.Add(upgradeIndex);
            }
            
            selectedIndices.Add(upgradeIndex);
            Upgrade upgrade = upgrades[upgradeIndex];

            PackedScene scene = (PackedScene)GD.Load("res://Scenes/upgrade_icon.tscn");
            UpgradeIcon upgradeIcon = (UpgradeIcon)scene.Instantiate();

            upgradeIcon.UpgradeName.Text = upgrade.UpgradeName;
            upgradeIcon.RedMineralCount.Text    = upgrade.RedMineralAmount.ToString();
            upgradeIcon.PurpleMineralCount.Text = upgrade.PurpleMineralAmount.ToString();
            upgradeIcon.YellowMineralCount.Text = upgrade.YellowMineralAmount.ToString();
            if (upgrade.MaxBuyAmount == 0)
            {
                upgradeIcon.AmountBought.Text = "0/∞";
            } 
            else
            {
                upgradeIcon.AmountBought.Text = "0/" + upgrade.MaxBuyAmount;
            }

            HBoxContainer.AddChild(upgradeIcon);

            upgradeIcon.RelatedUpgradeResource = upgrade;
        }
    }
}
