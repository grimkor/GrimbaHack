using GrimbaHack.Utility;
using nway.gameplay.match;

namespace GrimbaHack.Modules;

public class SimulationSpeed
{
    public static SimulationSpeed Instance { get; private set; }

    static SimulationSpeed()
    {
        Instance = new();
        OnSceneStartupOnDestroyActionHandler.Instance.AddCallback(() =>
        {
            if (MatchManager.instance.MatchIsOnline()) return;

            Instance.SetSpeed(100);
            GameManager.instance.InitializeSimulation();
        });
    }

    private int _speed = 100;

    public void SetSpeed(int speed)
    {
        if (!MatchManager.instance.MatchIsOnline())
        {
            Instance._speed = speed;
            SceneStartup.gamePlay._simulationManager.currentTimeScalePercent = speed;
        }
        else
        {
            Plugin.Log.LogInfo("Match is online, forcing speed to 100%");
            SceneStartup.gamePlay._simulationManager.currentTimeScalePercent = 100;
        }
    }


    public static int GetSpeed()
    {
        return SceneStartup.gamePlay._simulationManager.currentTimeScalePercent;
    }
}
