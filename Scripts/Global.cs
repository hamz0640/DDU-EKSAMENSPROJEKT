using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
    // TODO: Implement get caching (i.e. store the global in a private class
    // variable, instead of getting it each time)
    public static Global GetInstance() {
        return (Global)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/Global");
    }

    [Signal]
    public delegate void MineralCountUpdatedEventHandler(Mineral.MineralType mineralType, bool pickedUp);
    private System.Collections.Generic.Dictionary<string, Variant> State = new();


    public override void _Ready()
    {
        // Jetpack?
        State["CurrentEnergy"] = 100f;
        State["MaxEnergy"]     = 100f;
        State["JetpackEffeciency"] = 1.0f;
        State["JetpackDrain"] = 15.0f;

        // Mining 
        State["MiningDrain"] = 5.0f;


        // ===== Stats =====
        State["MaxInventorySpace"] = 10;
        State["MiningSpeed"] = 0.5f;
        State["Fortune"] = 1.0f;

        // Minerals
        State["RedMineralCount"]    = (uint)0;
        State["PurpleMineralCount"] = (uint)0;
        State["YellowMineralCount"] = (uint)0;
        State["TotalMineralCount"]  = (uint)0;

        // Upgrade Console
        State["DepositedRedMineralCount"]    = (uint)0;
        State["DepositedPurpleMineralCount"] = (uint)0;
        State["DepositedYellowMineralCount"] = (uint)0;
        State["Upgrades"] = new Array<Upgrade>();

        // Wave
        State["ShieldHealth"] = 200.0f;

        // Ship
        State["ShipHealth"] = 100.0f;
        State["CurrentShipHealth"] = 100.0f;
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("give_op"))
        {
            SetState("MiningSpeed", 100.0f);

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


    public T GetState<[MustBeVariant] T>(string state)
    {
        return State[state].As<T>();
    }

    public void SetState<[MustBeVariant] T>(string state, T value)
    {
        if (!State.ContainsKey(state)) 
            State.Add(state, Variant.From(value));
        else
            State[state] = Variant.From(value);
    }

    public void ModifyState(string state, int amount)
    {
        if (!State.ContainsKey(state)) State[state] = 0;

        int currentState = State[state].As<int>();
        State[state] = currentState + amount;
    }

    public void ModifyState(string state, uint amount)
    {
        if (!State.ContainsKey(state)) State[state] = 0u;

        uint currentState = State[state].As<uint>();
        State[state] = currentState + amount;
    }

    public void ModifyState(string state, float amount)
    {
        if (!State.ContainsKey(state)) State[state] = 0.0f;

        float currentState = State[state].As<float>();
        State[state] = currentState + amount;
    }

    public void ModifyState(string state, double amount)
    {
        if (!State.ContainsKey(state)) State[state] = 0.0d;

        double currentState = State[state].As<double>();
        State[state] = currentState + amount;
    }
}
