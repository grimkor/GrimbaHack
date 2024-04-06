using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using GrimbaHack.UI;
using GrimbaHack.UI.Twitch;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Predictions.CreatePrediction;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules.Twitch;

public class TwitchApi : ModuleBase
{
    public static TwitchApi Instance { get; private set; }

    static TwitchApi()
    {
        Instance = new TwitchApi();
    }

    private TwitchClient _client;
    private TwitchAPI _api;
    private string _predictionId;
    private List<string> _outcomes;
    private bool _enabled;
    public static Toggle TwitchEnableToggle { get; set; }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            MatchPrediction.Instance.Enabled = value;
            if (value)
            {
                Instance.Initialize();
            }
            else
            {
                Instance.Disconnect();
            }
        }
    }

    private void Initialize()
    {
        try
        {
            if (Plugin.TwitchClientID.Value.IsNullOrWhiteSpace() || Plugin.TwitchAccessToken.Value.IsNullOrWhiteSpace())
            {
                TwitchEnableToggle.isOn = false;
                Plugin.Log.LogError("Twitch Access Key or Client ID is missing, cannot initialize API.");
                return;
            }

            InitializeApi();
            GetUserDetails();
            InitializeClient();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
        }
    }

    private void Disconnect()
    {
        _client?.Disconnect();
    }

    private async void GetUserDetails()
    {
        try
        {
            if (_api == null)
            {
                TwitchEnableToggle.isOn = false;
                Plugin.Log.LogError("Cannot get user details as the API is not initialized");
                return;
            }

            var response = await _api.Helix.Users.GetUsersAsync();
            Plugin.TwitchBroadcasterID.Value = response.Users[0].Id;
            Plugin.TwitchUsername.Value = response.Users[0].Login;
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"ERROR IN GetUsers: {e}");
        }
    }

    private void InitializeApi()
    {
        if (Plugin.TwitchClientID.Value.IsNullOrWhiteSpace() || Plugin.TwitchAccessToken.Value.IsNullOrWhiteSpace())
        {
            TwitchEnableToggle.isOn = false;
            Plugin.Log.LogError("Twitch Access Key or Client ID is missing, cannot initialize API.");
            return;
        }

        _api = new TwitchAPI()
        {
            Settings =
            {
                ClientId = Plugin.TwitchClientID.Value,
                AccessToken = Plugin.TwitchAccessToken.Value,
            }
        };
    }

    private void InitializeClient()
    {
        try
        {
            if (Plugin.TwitchUsername.Value == "")
            {
                TwitchEnableToggle.isOn = false;
                Plugin.Log.LogError("Cannot Initialize Twitch Client, missing OAuth token or username");
                return;
            }

            var credentials =
                new ConnectionCredentials(Plugin.TwitchUsername.Value, Plugin.TwitchAccessToken.Value);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(credentials, Plugin.TwitchUsername.Value);
            var x = _client.Connect();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Twitch InitializeClient Error: {e}");
        }
    }

    public async void StartPrediction(string choiceOne, string choiceTwo)
    {
        if (!_enabled)
        {
            return;
        }

        try
        {
            _client.SendMessage(Plugin.TwitchUsername.Value,
                Plugin.TwitchPredictionMessage.Value.Replace("{P1}", choiceOne).Replace("{P2}", choiceTwo));
            var pred = new CreatePredictionRequest
            {
                Title = Plugin.TwitchPredictionTitle.Value.Replace("{P1}", choiceOne).Replace("{P2}", choiceTwo),
                Outcomes = new[] { new Outcome() { Title = choiceOne }, new Outcome() { Title = choiceTwo } },
                PredictionWindowSeconds = 30,
                BroadcasterId = Plugin.TwitchBroadcasterID.Value
            };

            var response = await _api.Helix.Predictions.GetPredictionsAsync(Plugin.TwitchBroadcasterID.Value);
            if (response.Data.Length > 0 && response.Data[0].EndedAt == null)
            {
                await _api.Helix.Predictions.EndPredictionAsync(Plugin.TwitchBroadcasterID.Value, response.Data[0].Id,
                    PredictionEndStatus.CANCELED);
            }

            var req = await _api.Helix.Predictions.CreatePredictionAsync(pred);
            _predictionId = req.Data[0].Id;
            _outcomes = req.Data[0].Outcomes.Select(outcome => outcome.Id).ToList();
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error in StartPrediction: {e}");
        }
    }

    public async void SetPredictionWinner(bool isFirstChoice)
    {
        if (!_enabled)
        {
            return;
        }

        try
        {
            if (_outcomes == null)
            {
                return;
            }

            var response = await _api.Helix.Predictions.EndPredictionAsync(Plugin.TwitchBroadcasterID.Value,
                _predictionId,
                PredictionEndStatus.RESOLVED, _outcomes.ElementAt(isFirstChoice ? 0 : 1));
            _outcomes = null;
        }
        catch (Exception e)
        {
            Plugin.Log.LogError($"Error in SetPredictionWinner: {e}");
        }
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var twitchViewerGroup = UIFactory.CreateUIObject("TwitchBoxViewerGroup", contentRoot);
        UIFactory.SetLayoutElement(twitchViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(twitchViewerGroup, false, false, true, true, padLeft: 25,
            spacing: 20, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(twitchViewerGroup, "TwitchEnableToggle",
            out var toggle, out var twitchEnableToggleLabel);
        TwitchEnableToggle = toggle;
        twitchEnableToggleLabel.text = "Enable Twitch Integration";
        TwitchEnableToggle.isOn = false;
        TwitchEnableToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));

        var button = UIFactory.CreateButton(twitchViewerGroup, "TwitchLoginButton", "Login");
        var loginWindow = new TwitchLoginPanel(UIManager.UIBase);
        button.OnClick += () => { loginWindow.Toggle(); };
        UIFactory.SetLayoutElement(TwitchEnableToggle.gameObject, minHeight: 25, minWidth: 220);
        UIFactory.SetLayoutElement(button.GameObject, minHeight: 25, minWidth: 100);
    }
}
