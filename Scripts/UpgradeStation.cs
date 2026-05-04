using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public partial class UpgradeStation : Node2D
{
    [Export]
    public Interactable InputChute = null;
    [Export]
    public Interactable UpgradeConsole = null;
    [Export]
    public UpgradeMenu UpgradeMenu = null;
    AudioStreamPlayer2D sfx;

    public override void _Ready()
    {
        AddToGroup("UpgradeStation");
        sfx = GetNode<AudioStreamPlayer2D>("Suck");
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

    async private void DepositMinerals()
    {
        Global global = Global.GetInstance();
        var swooop = async () =>
        {
            while (true)
            {
                await Task.Delay(100);
                uint redMineralCount = global.GetState<uint>("RedMineralCount");
                uint purpleMineralCount = global.GetState<uint>("PurpleMineralCount");
                uint yellowMineralCount = global.GetState<uint>("YellowMineralCount");

                if (redMineralCount + purpleMineralCount + yellowMineralCount == 0)
                    return;
                
                if (redMineralCount > 0)
                {
                    global.ModifyState("RedMineralCount", -1);
                    SwoopMineral(Mineral.MineralType.Red);
                    global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Red, false]);
                    global.ModifyState("DepositedRedMineralCount", 1);
                }
                else if (purpleMineralCount > 0)
                {
                    global.ModifyState("PurpleMineralCount", -1);
                    SwoopMineral(Mineral.MineralType.Purple);
                    global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Purple, false]);
                    global.ModifyState("DepositedPurpleMineralCount", 1);
                }
                else if (yellowMineralCount > 0)
                {
                    SwoopMineral(Mineral.MineralType.Yellow);
                    global.ModifyState("YellowMineralCount", -1);
                    global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Yellow, false]);
                    global.ModifyState("DepositedYellowMineralCount", 1);
                }
            }
        };

        swooop();
    }

    private void SwoopMineral(Mineral.MineralType mineralType)
    {
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");

        SwoopMineral swoopMineral = new SwoopMineral();
        
        swoopMineral.SetMineralType(mineralType);
        swoopMineral.GlobalPosition = camera.GlobalPosition;
        swoopMineral.Velocity = InputChute.GlobalPosition - camera.GlobalPosition;
        swoopMineral.ZIndex = 1000;

        GetTree().Root.AddChild(swoopMineral);
        sfx.Play();
    }
}
