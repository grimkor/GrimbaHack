using System;
using epoch.db;
using HarmonyLib;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace GrimbaHack.Modules;

public class SimulationSpeed
{
    public static SimulationSpeed Instance { get; private set; }

    static SimulationSpeed()
    {
        Instance = new();
    }

    private int _speed = 100;

    private int Speed
    {
        get => _speed;
        set
        {
            simulationSpeedValue.Text = value.ToString();
            _speed = value;
        }
    }

    public static void SetSpeed(int speed)
    {
        if (!MatchManager.instance.MatchIsOnline())
        {
            SceneStartup.gamePlay._simulationManager.currentTimeScalePercent = speed;
        }
        else
        {
            SceneStartup.gamePlay._simulationManager.currentTimeScalePercent = 100;
            simulationSpeedValue.Text = "100";
        }
    }

    [HarmonyPatch(typeof(SceneStartup), nameof(SceneStartup.OnDestroy))]
    private class PatchOnDestroy
    {
        private static void Prefix()
        {
            if (MatchManager.instance.MatchIsOnline()) return;

            Instance.Speed = 100;
            SetSpeed(100);
            GameManager.instance.InitializeSimulation();
        }
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var simulationSpeedGroup = UIFactory.CreateUIObject("SimulationSpeedGroup", contentRoot);
        UIFactory.SetLayoutElement(simulationSpeedGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(simulationSpeedGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        var simulationSpeedButton = UIFactory.CreateButton(simulationSpeedGroup, "SimulationSpeedToggle", "Set Speed");
        simulationSpeedButton.OnClick = delegate { SetSpeed(Instance.Speed); };

        simulationSpeedValue =
            UIFactory.CreateInputField(simulationSpeedGroup, "SimulationSpeedValue", Instance.ToString());
        simulationSpeedValue.Text = Instance.Speed.ToString();
        simulationSpeedValue.OnValueChanged += s =>
        {
            if (s.Length > 0)
            {
                Instance.Speed = Convert.ToInt32(s);
            }
            else
            {
                Instance.Speed = 100;
            }
        };

        var minusButton = UIFactory.CreateButton(simulationSpeedGroup, "simulationSpeedMinusButton", "-");
        minusButton.OnClick = delegate
        {
            Instance.Speed -= 5;
            simulationSpeedValue.Text = Instance.Speed.ToString();
        };

        var plusButton = UIFactory.CreateButton(simulationSpeedGroup, "simulationSpeedPlusButton", "+");
        plusButton.OnClick = delegate
        {
            Instance.Speed += 5;
            simulationSpeedValue.Text = Instance.Speed.ToString();
        };

        UIFactory.SetLayoutElement(simulationSpeedButton.GameObject, minHeight: 25, minWidth: 50);
        UIFactory.SetLayoutElement(minusButton.GameObject, minHeight: 25, minWidth: 50);
        UIFactory.SetLayoutElement(simulationSpeedValue.GameObject, minHeight: 25, minWidth: 50);
        UIFactory.SetLayoutElement(plusButton.GameObject, minHeight: 25, minWidth: 50);
    }

    public static InputFieldRef simulationSpeedValue { get; set; }
}