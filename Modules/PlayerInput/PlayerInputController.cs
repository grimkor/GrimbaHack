using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GrimbaHack.Modules.Combo;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace GrimbaHack.Modules.PlayerInput;

public class ComboExport
{
    public List<string> Combo;
    public List<uint> Inputs;
}

public sealed class PlayerInputController : ModuleBase
{
    private PlayerInputBehaviour Behaviour;
    private static GameObject playerInputButtonGroup;
    private static ButtonRef recordButton;
    private static ButtonRef playbackButton;
    private static ButtonRef exportButton;

    private PlayerInputController()
    {
    }

    public static PlayerInputController Instance { get; set; }

    static PlayerInputController()
    {
        Instance = new PlayerInputController();
        ClassInjector.RegisterTypeInIl2Cpp<PlayerInputBehaviour>();
        GameObject go = new GameObject("PlayerInputBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(go);
        Instance.Behaviour = go.AddComponent<PlayerInputBehaviour>();
        Instance.Behaviour.SetEnable(false);
        OnEnterTrainingMatchActionHandler.Instance.AddCallback(() => { Instance.Behaviour.SetEnable(true); });
        OnSimulationInitializeActionHandler.Instance.AddCallback(() => { Instance.Behaviour.Setup(); });
        OnUIComboCounterOnBreakComboCallbackHandler.Instance.AddCallback((playerId, comboCount) =>
        {
            if (Instance.Behaviour.enabled)
            {
                Instance.Behaviour.Reset();
            }
        });
    }

    private void ExportCombo()
    {
        var combo = ComboTracker.Instance.GetCombo();
        var inputs = Instance.Behaviour._inputs;
        var exportClass = new ComboExport()
        {
            Combo = combo,
            Inputs = inputs,
        };
        var options = new JsonSerializerOptions { IncludeFields = true };
        System.IO.File.WriteAllText(Path.Join(BepInEx.Paths.GameRootPath, "output", "combo.json"),
            System.Text.Json.JsonSerializer.Serialize(exportClass, options));
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        playerInputButtonGroup = UIFactory.CreateUIObject("PlayerInputButtonGroup", contentRoot);
        UIFactory.SetLayoutElement(playerInputButtonGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(playerInputButtonGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        recordButton = UIFactory.CreateButton(playerInputButtonGroup, "PlayerButtonRecordButton", "Record");
        recordButton.OnClick += () =>
        {
            Instance.Behaviour.Record();
            ComboTracker.Instance.SetState(ComboTrackerState.Recording);
        };
        playbackButton = UIFactory.CreateButton(playerInputButtonGroup, "PlayerButtonPlaybackButton", "Playback");
        playbackButton.OnClick += () => { Instance.Behaviour.Playback(); };
        exportButton = UIFactory.CreateButton(playerInputButtonGroup, "PlayerButtonExport", "Export");
        exportButton.OnClick += () => { Instance.ExportCombo(); };
        UIFactory.SetLayoutElement(playerInputButtonGroup.gameObject, minHeight: 25, minWidth: 50);
        UIFactory.SetLayoutElement(recordButton.GameObject, minHeight: 25, minWidth: 75);
        UIFactory.SetLayoutElement(playbackButton.GameObject, minHeight: 25, minWidth: 50);
        UIFactory.SetLayoutElement(exportButton.GameObject, minHeight: 25, minWidth: 50);
    }
}