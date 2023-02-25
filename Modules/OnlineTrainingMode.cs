using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public class OnlineTrainingMode : CheatPrevention
{
    public static OnlineTrainingMode Instance { get; private set; }
    private OnlineTrainingModeBehaviour _behaviour;
    private Toggle _onlineTrainingModeToggle;

    static OnlineTrainingMode()
    {
        Instance = new OnlineTrainingMode();
        ClassInjector.RegisterTypeInIl2Cpp<OnlineTrainingModeBehaviour>();
        var go = new GameObject("OnlineTrainingModeBehaviour");
        go.hideFlags = HideFlags.HideAndDontSave;
        GameObject.DontDestroyOnLoad(go);
        Instance._behaviour = go.AddComponent<OnlineTrainingModeBehaviour>();
        OnEnterPremadeMatchActionHandler.Instance.AddCallback(() =>
            Instance._onlineTrainingModeToggle.isOn = false
        );
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => { Instance.SetEnabled(false); });
        Instance.SetEnabled(false);
    }


    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
        Instance._behaviour.enabled = enabled;

        if (SceneStartup.instance != null)
            SceneStartup.instance.GamePlay.SetGameStateTimer(enabled ? -1 : 5);
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var onlineTrainingModeGroup = UIFactory.CreateUIObject("OnlineTrainingModeGroup", contentRoot);
        UIFactory.SetLayoutElement(onlineTrainingModeGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(onlineTrainingModeGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(onlineTrainingModeGroup, "OnlineTrainingModeToggle",
            out Instance._onlineTrainingModeToggle,
            out var onlineTrainingModeToggleLabel);
        Instance._onlineTrainingModeToggle.isOn = false;
        onlineTrainingModeToggleLabel.text = "Enable Online Training Mode";
        Instance._onlineTrainingModeToggle.onValueChanged.AddListener(new Action<bool>((value) =>
            Instance.SetEnabled(value)));
        UIFactory.SetLayoutElement(Instance._onlineTrainingModeToggle.gameObject, minHeight: 25, minWidth: 100);

        var textTemplate = new List<string>()
        {
            "*** WARNING ***",
            "Enabling 'Online Training Mode' will cause a desync unless your opponent does the same. To get out of this mode the only option is to ALT+F4."
        };
        var warningTextGroup = UIFactory.CreateUIObject("OnlineTrainingModeGroup", contentRoot);
        UIFactory.SetLayoutElement(warningTextGroup);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(warningTextGroup, false, false, true, true, padLeft: 25,
            padRight: 25,
            spacing: 5, childAlignment: TextAnchor.MiddleLeft);

        foreach (var line in textTemplate)
        {
            var text = UIFactory.CreateLabel(warningTextGroup, "OnlineTrainingModeWarningText", line);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            UIFactory.SetLayoutElement(text.gameObject, minHeight: 25, minWidth: 25);
        }
    }
}

public class OnlineTrainingModeBehaviour : MonoBehaviour
{
    void Update()
    {
        if (!enabled) return;

        if (SceneStartup.instance)
        {
            foreach (var character in SceneStartup.instance.GamePlay._playerList)
            {
                character.characterTeam.superMeter.current = 3000;
                character.characterTeam.mzMeter.current = 6000;
                foreach (var teamMember in character.characterTeam.members)
                {
                    teamMember.health.current = teamMember.health.max;
                }
            }
        }
    }
}