using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class UpgradeStation : Node2D
{
    [Export]
    public Interactable InputChute = null;
    [Export]
    public Interactable UpgradeConsole = null;
    [Export]
    public UpgradeMenu UpgradeMenu = null;


    public override void _Ready()
    {
        AddToGroup("UpgradeStation");
        InputChute.Interact += DepositMinerals;
        UpgradeConsole.Interact += () =>
        {
            ToggleUpgradeConsole(!UpgradeMenu.Visible);
            Global global = Global.GetInstance();
            global.SetState("PlayerCanMove", !UpgradeMenu.Visible);
        };
    }


    public override void _PhysicsProcess(double delta)
    {
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");
        Vector2 cameraPosition = camera.GlobalPosition;

        if (UpgradeConsole.GlobalPosition.DistanceTo(cameraPosition) > 23.0)
        {
            ToggleUpgradeConsole(false);
        }
    }

    public void ToggleUpgradeConsole(bool visible)
    {
        CanvasLayer UI = (CanvasLayer)GetTree().GetFirstNodeInGroup("UI");

        UI.Visible = !visible;
        UpgradeMenu.Visible = visible;
        UpgradeMenu.SelectedIndex = 0;
    }

    private void DepositMinerals()
    {
        Global global = Global.GetInstance();

        uint redMineralCount    = global.GetState<uint>("RedMineralCount");
        uint purpleMineralCount = global.GetState<uint>("PurpleMineralCount");
        uint yellowMineralCount = global.GetState<uint>("YellowMineralCount");

        uint depositedRedMineralCount    = global.GetState<uint>("DepositedRedMineralCount");
        uint depositedPurpleMineralCount = global.GetState<uint>("DepositedPurpleMineralCount");
        uint depositedYellowMineralCount = global.GetState<uint>("DepositedYellowMineralCount");

        global.SetState("DepositedRedMineralCount",    depositedRedMineralCount    + redMineralCount);
        global.SetState("DepositedPurpleMineralCount", depositedPurpleMineralCount + purpleMineralCount);
        global.SetState("DepositedYellowMineralCount", depositedYellowMineralCount + yellowMineralCount);

        global.SetState<uint>("RedMineralCount", 0);
        global.SetState<uint>("PurpleMineralCount", 0);
        global.SetState<uint>("YellowMineralCount", 0);

        global.SetState<uint>("TotalMineralCount", 0);

        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Red,    false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Purple, false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Yellow, false]);
    }
}
