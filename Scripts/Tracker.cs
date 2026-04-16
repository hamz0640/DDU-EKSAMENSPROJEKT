using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Tracker : Node
{
    private FileAccess TrackingFile;
    private Dictionary<string, Variant> Trackings = new();

    public static Tracker GetInstance() {
        return (Tracker)((SceneTree)Engine.GetMainLoop()).Root.GetNode("/root/Tracker");
    }
    public override void _EnterTree()
    {
        // All tracks are added here, at the start of the program
        // They can also be dynamically registered, if that is more desired
        Trackings["Time:Total"]        = 0f; // Check
        Trackings["Time:OnWire"]       = 0f; // Check
        Trackings["Time:InAir"]        = 0f; // Check
        Trackings["Time:UsingJetpack"] = 0f; // Check
        Trackings["Time:OnGround"]     = 0f; // Check
        Trackings["Time:InTurret"]     = 0f; // Check
        Trackings["Time:InMine"]       = 0f; // Check
        Trackings["Time:InWaves"]      = 0f; // Check

        Trackings["UpgradesBought:Total"] = 0u;     // Check
        // Each upgrade registers their own tracker // Check

        Trackings["Max:DepthReached"] = 0f; // Check
        Trackings["Max:WidthReached"] = 0f; // Check
        Trackings["Max:WaveReached"]  = 0u; // Check

        Trackings["Mine:TotalMined"] = 0u; // Check
        Trackings["Mine:DirtMined"]  = 0u; // Check
        Trackings["Mine:MineralsMined:Red"]    = 0u; // Check
        Trackings["Mine:MineralsMined:Purple"] = 0u; // Check
        Trackings["Mine:MineralsMined:Yellow"] = 0u; // Check
        Trackings["Mine:MineralsSpawned:Red"]     = 0u; // Check
        Trackings["Mine:MineralsSpawned:Purple"]  = 0u; // Check
        Trackings["Mine:MineralsSpawned:Yellow"]  = 0u; // Check

        Trackings["Wave:TimesShot"] = 0u;          // Check
        Trackings["Wave:AsteroidsShot"] = 0u;      // Check
        Trackings["Wave:AsteroidsHitShield"] = 0u; // Check
        Trackings["Wave:AsteroidsSpawned"] = 0u;   // Check
        Trackings["Wave:ShieldDamageTaken"] = 0f;  // Check

        Godot.Collections.Dictionary time = Time.GetDatetimeDictFromSystem();
        string year = time["year"].ToString();
        string month = time["month"].ToString();
        string day = time["day"].ToString();

        string hour = time["hour"].ToString();
        string minute = time["minute"].ToString();
        string second = time["second"].ToString();

        string fileName = day + "-" + month + "-" + year + "   " + hour + "-" + minute + "-" + second; 

        TrackingFile = FileAccess.Open("res://" + fileName + ".txt", FileAccess.ModeFlags.Write);
        if (TrackingFile == null)
        {
            GD.PrintErr("Failed to open tracking file");
        }
    }

    public override void _ExitTree()
    {
        List<KeyValuePair<string, Variant>> finalTrackings = Trackings.ToList();
        finalTrackings.Sort( (a, b) => string.CompareOrdinal(a.Key, b.Key) );
        foreach (KeyValuePair<string, Variant> track in finalTrackings)
        {
            TrackingFile.StoreLine(track.Key + ": " + track.Value.ToString());
        }

        TrackingFile.Flush();
        TrackingFile.Close();
    }

    public T GetTracking<[MustBeVariant] T>(string tracking)
    {
        return Trackings[tracking].As<T>();
    }

    public void SetTracking<[MustBeVariant] T>(string tracking, T value)
    {
        if (!Trackings.ContainsKey(tracking))
        {
            Trackings.Add(tracking, Variant.From(value));
        } 
        else
        {
            Trackings[tracking] = Variant.From(value);
        }
    }

    public void IncrementTracking(string tracking, uint increment)
    {
        if (!Trackings.ContainsKey(tracking))
        {
            Trackings.Add(tracking, Variant.From(0u));
        } 

        uint previousValue = Trackings[tracking].As<uint>();
        Trackings[tracking] = Variant.From(previousValue + increment);
    }

    public void IncrementTracking(string tracking, float increment)
    {
        if (!Trackings.ContainsKey(tracking))
        {
            Trackings.Add(tracking, Variant.From(0f));
        } 

        float previousValue = Trackings[tracking].As<float>();
        Trackings[tracking] = Variant.From(previousValue + increment);
    }
}
