using Godot;
using System;

public partial class UpgradeStation : Node2D
{
    [Export]
    public Area2D InputChute = null;
    [Export]
    public Area2D UpgradeConsole = null;


    public override void _PhysicsProcess(double delta)
    {
        Camera2D camera = (Camera2D)GetTree().GetFirstNodeInGroup("Camera");
        CharacterBody2D player = (CharacterBody2D)camera.GetParent();
        
        Vector2 cameraPosition = camera.GlobalPosition;
        Vector2 InputChutePosition = InputChute.GlobalPosition;
        Vector2 UpgradeConsolePosition = UpgradeConsole.GlobalPosition;

        float distanceToInputChute = cameraPosition.DistanceTo(InputChutePosition);
        float distanceToUpgradeConsole = cameraPosition.DistanceTo(UpgradeConsolePosition);

        bool closerToInputChute = distanceToInputChute < distanceToUpgradeConsole;
        
        bool canDepositMinerals    = InputChute.GetOverlappingBodies().Contains(player) && closerToInputChute;
        bool canOpenUpgradeConsole = UpgradeConsole.GetOverlappingBodies().Contains(player) && !closerToInputChute;

        if (canDepositMinerals && Input.IsActionJustPressed("interact"))
            DepositMinerals();

        if (canOpenUpgradeConsole && Input.IsActionJustPressed("interact"))
        {
            
        }
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

        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Red,    false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Purple, false]);
        global.EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Yellow, false]);
    }
}
