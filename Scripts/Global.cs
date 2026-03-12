using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
    public static Global GetInstance() {
        return (Global)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/Global");
    }

    [Signal]
    public delegate void MineralCountUpdatedEventHandler(Mineral.MineralType mineralType, bool pickedUp);
    private Dictionary<string, Variant> Stats = new();
    private Dictionary<string, Variant> State = new();


    public override void _Ready()
    {
        // ===== Stats =====
        Stats["MaxInventorySpace"] = 10;
        Stats["MiningSpeed"] = 0.5f;

        // ===== State =====
        // Minerals
        State["RedMineralCount"]    = (uint)0;
        State["PurpleMineralCount"] = (uint)0;
        State["YellowMineralCount"] = (uint)0;
        State["TotalMineralCount"]  = (uint)0;

        // Upgrade Console
        State["DepositedRedMineralCount"]    = (uint)0;
        State["DepositedPurpleMineralCount"] = (uint)0;
        State["DepositedYellowMineralCount"] = (uint)0;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("give_op"))
        {
            SetStat("MiningSpeed", 100.0f);

            SetState("DepositedRedMineralCount",    (uint)99999);
            SetState("DepositedPurpleMineralCount", (uint)99999);
            SetState("DepositedYellowMineralCount", (uint)99999);

            SetState("RedMineralCount",    (uint)99999);
            SetState("PurpleMineralCount", (uint)99999);
            SetState("YellowMineralCount", (uint)99999);

            EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Red,    false]);
            EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Purple, false]);
            EmitSignal("MineralCountUpdated", [(int)Mineral.MineralType.Yellow, false]);
        }
    }


    public T GetStat<[MustBeVariant] T>(string stat)
    {
        return Stats[stat].As<T>();
    }

    public void SetStat<[MustBeVariant] T>(string stat, T value)
    {
        Stats[stat] = Variant.From(value);
    }

    public T GetState<[MustBeVariant] T>(string state)
    {
        return State[state].As<T>();
    }

    public void SetState<[MustBeVariant] T>(string state, T value)
    {
        State[state] = Variant.From(value);
    }
}
