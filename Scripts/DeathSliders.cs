using Godot;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

public partial class DeathSliders : Control 
{
    [Export]
    private ColorRect TopDeathSlider;
    [Export]
    private ColorRect BottomDeathSlider;

    private bool IsDead = false;
    private void OnQuitPressed()
        {
            string exePath = OS.GetExecutablePath();

            Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            UseShellExecute = true
        });

            GetTree().Quit();
        }
    public override void _Process(double delta)
    {
        Global global = Global.GetInstance();
        if (global.GetState<float>("CurrentEnergy") <= 0.0f || global.GetState<float>("CurrentShipHealth") <= 0.0f && IsDead == false)
        {
            Tween tween = GetTree().CreateTween();

            TopDeathSlider.Show();
            BottomDeathSlider.Show();

            tween.SetParallel(true);
            tween.TweenProperty(TopDeathSlider, "position", new Vector2(0, 0), 0.5);
            tween.TweenProperty(BottomDeathSlider, "position", new Vector2(0, 540), 0.5);

            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);

            IsDead = true;

            tween.Finished += () =>
            {
                GetTree().CreateTimer(0.5).Timeout += () =>
                {
                   OnQuitPressed();    
                };
            };

            


        }
    }
}
