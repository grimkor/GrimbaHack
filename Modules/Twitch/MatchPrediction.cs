using System;
using System.Linq;
using ArenaProtocol;
using GrimbaHack.UI;
using GrimbaHack.UI.Twitch;
using GrimbaHack.Utility;
using Il2CppSystem.Collections.Generic;
using nway.gameplay;
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
    private PlayerInfo playerOne;
    private PlayerInfo playerTwo;
    private string currentMatchId;
    private List<string> previousMatches = new List<string>();
    private bool ongoingMatch;

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
            try
            {
                if (!Instance._enabled)
                {
                    return;
                }

                if (Instance.previousMatches.Contains(Instance.currentMatchId))
                {
                    return;
                }
                Instance.previousMatches.Add(Instance.currentMatchId);
            
                if (matchEndInfo == MatchResult.PLAYER1_WIN)
                {
                    Instance.playerOneWins++;
                }

                if (matchEndInfo == MatchResult.PLAYER2_WIN)
                {
                    Instance.playerTwoWins++;
                }

            
                if (MatchHasFinished())
                {
                    TwitchApi.Instance.SetPredictionWinner(matchEndInfo == MatchResult.PLAYER1_WIN);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogInfo(e); 
            }
        });
        OnMatchManagerActionHandler.Instance.AddCallback(match =>
        {
            try
            {
                if (!Instance._enabled)
                {
                    return;
                }

                Instance.currentMatchId = match.GetMatchId();
            
                var players = match.GetMatchUserList();
                var playerIds = players.ToArray().Select(info => info.pid).ToArray();
                if (Plugin.TwitchPredictionTournamentMode.Value && !MatchHasFinished() && playerIds.Contains(Instance.playerOne?.pid) &&
                    playerIds.Contains(Instance.playerTwo?.pid))
                {
                    return;
                }
            
                Plugin.Log.LogInfo(match.GetType());
                if (match.GetMatchType() == MatchType.RANKED || match.GetMatchType() == MatchType.LOBBY)
                {
                    Instance.Reset();
                    Instance.winsNeeded = Plugin.TwitchPredictionTournamentMode.Value ? Plugin.TwitchPredictionWinsRequired.Value : match.matchRule.minWinCount;
                    Instance.playerOne = players[0];
                    Instance.playerTwo = players[1];
                
                    TwitchApi.Instance.StartPrediction(Instance.playerOne.userName, Instance.playerTwo.userName);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError(e);
            }
        });
    }

    private static bool MatchHasFinished()
    {
        return Instance.playerOneWins >= Instance.winsNeeded || Instance.playerTwoWins >= Instance.winsNeeded;
    }

    private void Reset()
    {
        Instance.playerOne = null;
        Instance.playerTwo = null;
        Instance.playerOneWins = 0;
        Instance.playerTwoWins = 0;
        Instance.previousMatches.Clear();
    }

    private static void SetUIColours()
    {
        matchPredictionEnableToggleLabel.color = Instance.Enabled ? Color.white : Color.grey;
    }
    public static void CreateUIControls(GameObject contentRoot)
    {
        try
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
                Instance.Reset();
            }));
            var predictionMessageButton =
                UIFactory.CreateButton(matchPredictionViewerGroup, "predictionMessageButton", "Prediction Settings");
            predictionMessageButton.OnClick += () =>
            {
                var panel = new TwitchBotPredictionMessagePanel(UIManager.UIBase);
                panel.Toggle();
            };
            
            UIFactory.SetLayoutElement(MatchPredictionToggle.gameObject, minHeight: 25, minWidth: 220);
            UIFactory.SetLayoutElement(predictionMessageButton.GameObject, minHeight: 25, minWidth: 100);
            SetUIColours();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

}