using System;
using ArenaProtocol;
using GrimbaHack.UI;
using GrimbaHack.UI.Twitch;
using GrimbaHack.Utility;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using MatchType = epoch.db.MatchType;

namespace GrimbaHack.Modules.Twitch;

public class MatchPrediction : ModuleBase
{
    public static MatchPrediction Instance { get; private set; }
    private bool _enabled;
    public static Toggle MatchPredictionToggle { get; set; }
    public static Text matchPredictionEnableToggleLabel { get; set; }
    private int playerOneWins = 0;
    private int playerTwoWins = 0;
    private int winsNeeded = 0;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            SetUIColours();
        }
    }

    static MatchPrediction()
    {
        Instance = new();
        OnCombatConditionMatchEndActionHandler.Instance.AddCallback((matchEndInfo) =>
        {
            if (!Instance._enabled)
            {
                return;
            }

            if (matchEndInfo == MatchResult.PLAYER1_WIN)
            {
                Instance.playerOneWins++;
            }

            if (matchEndInfo == MatchResult.PLAYER2_WIN)
            {
                Instance.playerTwoWins++;
            }

            if (Instance.playerOneWins >= Instance.winsNeeded || Instance.playerTwoWins >= Instance.winsNeeded)
            {
                TwitchApi.Instance.SetPredictionWinner(matchEndInfo == MatchResult.PLAYER1_WIN);
            }
        });
        OnMatchManagerActionHandler.Instance.AddCallback(match =>
        {
            if (!Instance._enabled)
            {
                return;
            }

            
            if (match.GetMatchType() == MatchType.RANKED || match.GetMatchType() == MatchType.LOBBY)
            {
                Instance.winsNeeded = match.matchRule.minWinCount;
                Instance.playerOneWins = 0;
                Instance.playerTwoWins = 0;
                    TwitchApi.Instance.StartPrediction(match.GetMatchUserList()[0].userName,
                        match.GetMatchUserList()[1].userName);
            }
        });
    }

    private static void SetUIColours()
    {
            matchPredictionEnableToggleLabel.color = Instance.Enabled ? Color.white : Color.grey;
    }
    public static void CreateUIControls(GameObject contentRoot)
    {
        
        var matchPredictionViewerGroup = UIFactory.CreateUIObject("MatchPredictionBoxViewerGroup", contentRoot);
        UIFactory.SetLayoutElement(matchPredictionViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(matchPredictionViewerGroup, false, false, true, true, padLeft: 25,
            spacing: 20, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(matchPredictionViewerGroup, "MatchPredictionToggle",
            out var toggle, out var label);
        MatchPredictionToggle = toggle;
        matchPredictionEnableToggleLabel = label;
        matchPredictionEnableToggleLabel.text = "Enable Match Prediction";
        MatchPredictionToggle.isOn = false;
        MatchPredictionToggle.onValueChanged.AddListener(new Action<bool>((value) =>
        {
            Instance.Enabled = value;
        }));
        var predictionMessageButton =
            UIFactory.CreateButton(matchPredictionViewerGroup, "predictionMessageButton", "Set Message");
        predictionMessageButton.OnClick += () =>
        {
            var panel = new TwitchBotPredictionMessagePanel(UIManager.UIBase);
            panel.Toggle();
        };
        
        UIFactory.SetLayoutElement(MatchPredictionToggle.gameObject, minHeight: 25, minWidth: 220);
        UIFactory.SetLayoutElement(predictionMessageButton.GameObject, minHeight: 25, minWidth: 100);
        SetUIColours();
    }

}