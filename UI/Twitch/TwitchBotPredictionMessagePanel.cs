using System;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI.Twitch;

public class TwitchBotPredictionMessagePanel : PanelBase
{
    public bool tournamentModeValue { get; set; } = Plugin.TwitchPredictionTournamentMode.Value;
    public int winsNeededValue { get; set; } = Plugin.TwitchPredictionWinsRequired.Value;
    public GameObject winsNeededLayout { get; set; }

    public TwitchBotPredictionMessagePanel(UIBase owner) : base(owner)
    {
    }

    protected override void ConstructPanelContent()
    {
        Enabled = false;
        var mainVerticalGroup = UIFactory.CreateUIObject("MainVerticalGroup", ContentRoot);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(mainVerticalGroup, false, false, true, true, padLeft: 25,
            padTop: 15, padBottom: 15, childAlignment: TextAnchor.MiddleLeft);

        var predictionMessageLabel = UIFactory.CreateLabel(mainVerticalGroup, "Text",
            "Bot Message for when a Prediction starts. {P1} and {P2} will be replaced with the player names.");
        predictionMessageLabel.horizontalOverflow = HorizontalWrapMode.Wrap;

        var titleLayout = UIFactory.CreateUIObject("titleLayout", mainVerticalGroup);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(titleLayout, false, false, true, true, spacing: 1);
        var titleLabel = UIFactory.CreateLabel(titleLayout, "titleLabel", "Title", TextAnchor.LowerLeft);
        var predictionTitleField = UIFactory.CreateInputField(titleLayout, "PredictionTitleInput", "Title Message");
        predictionTitleField.Text = Plugin.TwitchPredictionTitle.Value;

        var messageLayout = UIFactory.CreateUIObject("messageLayout", mainVerticalGroup);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(messageLayout, false, false, true, true, padBottom: 15, spacing: 1);
        var messageLabel = UIFactory.CreateLabel(messageLayout, "messageLabel", "Message (leave empty for no message)", TextAnchor.LowerLeft);
        var predictionMessageField = UIFactory.CreateInputField(messageLayout, "PredictionMessageInput", "Bot Message");
        predictionMessageField.Text = Plugin.TwitchPredictionMessage.Value;

        var tournamentLayout = UIFactory.CreateUIObject("tournament layout", mainVerticalGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(tournamentLayout, false, false, true, true, spacing: 10, padBottom: 15);
        UIFactory.CreateToggle(tournamentLayout, "tournamentModeToggle", out var tournamentModeToggle,
            out var tournamentModeLabel);
        tournamentModeToggle.isOn = Plugin.TwitchPredictionTournamentMode.Value;
        tournamentModeLabel.text = "Tournament Mode";
        tournamentModeToggle.onValueChanged.AddListener(new Action<bool>((value) =>
        {
            tournamentModeValue = value;
            winsNeededLayout.active = value;
        }));

        winsNeededLayout = UIFactory.CreateUIObject("winsNeededLayout", tournamentLayout);
        winsNeededLayout.active = Plugin.TwitchPredictionTournamentMode.Value;
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(winsNeededLayout, false, true, true, true,  spacing: 5); 
        var winsNeeded = UIFactory.CreateInputField(winsNeededLayout, "winsNeededInput", "");
        winsNeeded.Text = Plugin.TwitchPredictionWinsRequired.Value.ToString();
        winsNeeded.OnValueChanged += s =>
        {
            if (int.TryParse(s, out var value))
            {
                winsNeededValue = value;
                winsNeeded.Text = value.ToString();
            }
            else
            {
                winsNeeded.Text = winsNeededValue.ToString();
            }
        };
        var winsNeededLabel = UIFactory.CreateLabel(winsNeededLayout, "winsNeededLabel", "Wins");


        var saveButton = UIFactory.CreateButton(mainVerticalGroup, "SaveMessagesButton", "Save"); 
        saveButton.OnClick = () =>
        {
            Plugin.TwitchPredictionTitle.Value = predictionTitleField.Text;
            Plugin.TwitchPredictionMessage.Value = predictionMessageField.Text;
            Plugin.TwitchPredictionTournamentMode.Value = tournamentModeValue;
            Plugin.TwitchPredictionWinsRequired.Value = winsNeededValue;
            
            Enabled = false;
        };
        
        UIFactory.SetLayoutElement(predictionMessageLabel.gameObject, minHeight: 30, minWidth: 90);
        UIFactory.SetLayoutElement(titleLabel.gameObject, minHeight: 30, minWidth: 90);
        UIFactory.SetLayoutElement(predictionTitleField.GameObject, minHeight: 30, minWidth: 450);
        UIFactory.SetLayoutElement(messageLabel.gameObject, minHeight: 30, minWidth: 90);
        UIFactory.SetLayoutElement(predictionMessageField.GameObject, minHeight: 30, minWidth: 450);
        UIFactory.SetLayoutElement(tournamentModeToggle.gameObject, minHeight: 30);
        UIFactory.SetLayoutElement(winsNeeded.GameObject, minWidth: 30);
        UIFactory.SetLayoutElement(winsNeededLabel.gameObject);
        UIFactory.SetLayoutElement(saveButton.GameObject, minHeight: 30, minWidth: 100);
    }

    public override string Name { get; } = "Match Prediction Bot Message";
    public override int MinWidth { get; } = 500;
    public override int MinHeight { get; } = 300;
    public override Vector2 DefaultAnchorMin { get; } = new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax { get; } = new Vector2(0.5f, 0.5f);
}