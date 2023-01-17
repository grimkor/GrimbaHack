using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;

namespace GrimbaHack.Modules;

public class SimulationSpeed
{
    public static SimulationSpeed Instance { get; private set; }

    static SimulationSpeed()
    {
        Instance = new();
    }

    private bool _enabled;

    public static bool Enabled
    {
        get => Instance._enabled;
        set
        {
            Instance._enabled = value;

            if (!value)
            {
                SetSpeed(100);
            }
        }
    }


    public static void SetSpeed(float speed)
    {
        if (Enabled && MatchManager.instance.match != null)
        {
            Table
        }
    }

    [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetupGamePlay))]
    public class PatchSetupGamePlay
    {
        public static void Postfix(Match match,
            string pid,
            PlayerControllerMapping controllerMapping)
        {
            if (match.IsOnlineMatch() || !Instance._enabled)
            {
                Enabled = false;
            }
        }
    }
}