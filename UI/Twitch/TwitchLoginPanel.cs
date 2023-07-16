using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI.Twitch;

public class TwitchLoginPanel : PanelBase
{
    public TwitchLoginPanel(UIBase owner) : base(owner)
    {
    }

    protected override void ConstructPanelContent()
    {
        Enabled = false;
        var twitchLoginViewerGroup = UIFactory.CreateUIObject("TwitchLoginBoxViewerGroup", ContentRoot);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(twitchLoginViewerGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);

        var labelGroup = UIFactory.CreateUIObject("TwitchLoginLabelViewerGroup", twitchLoginViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(labelGroup, false, false, true, true, spacing: 10, 
            childAlignment: TextAnchor.MiddleLeft);
        var Text = UIFactory.CreateLabel(labelGroup, "Text",
            "Click button to generate codes: ");
        var linkButton = UIFactory.CreateButton(labelGroup, "URLLink", "twitchtokengenerator.com");
        linkButton.OnClick += () =>
        {
            Application.OpenURL("https://twitchtokengenerator.com/quick/L4hCq2DV6Q");
        };

        
        var clientIdLabelGroup = UIFactory.CreateUIObject("TwitchClientIDLabelViewerGroup", twitchLoginViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(clientIdLabelGroup, false, false, true, true, spacing: 10, 
            childAlignment: TextAnchor.MiddleLeft);
        var clientIdText = UIFactory.CreateLabel(clientIdLabelGroup, "Text", "Client ID");
        var clientIdField = UIFactory.CreateInputField(clientIdLabelGroup, "TwitchClientIdField", "Client ID");
        
        var accessCodeLabelGroup = UIFactory.CreateUIObject("TwitchAccessTokenLabelViewerGroup", twitchLoginViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(accessCodeLabelGroup, false, false, true, true, spacing: 10, 
            childAlignment: TextAnchor.MiddleLeft);
        var accessTokenText = UIFactory.CreateLabel(accessCodeLabelGroup, "Text",
            "Access Token");
        var accessTokenField = UIFactory.CreateInputField(accessCodeLabelGroup, "TwitchTokenCode", "Access Token");

        var saveButton = UIFactory.CreateButton(twitchLoginViewerGroup, "TwitchSaveCodesButton", "Save Details");
        saveButton.OnClick = () =>
        {
            Plugin.TwitchAccessToken.Value = accessTokenField.Text != "" ? accessTokenField.Text : Plugin.TwitchAccessToken.Value;
            Plugin.TwitchClientID.Value = clientIdField.Text != "" ? clientIdField.Text : Plugin.TwitchClientID.Value;
            accessTokenField.Text = "";
            clientIdField.Text = "";
            Enabled = false;
        };
        
        
        
        UIFactory.SetLayoutElement(Text.gameObject, minHeight: 30, minWidth: 50);
        UIFactory.SetLayoutElement(linkButton.GameObject, minHeight: 30, minWidth: 200);
        UIFactory.SetLayoutElement(clientIdText.gameObject, minHeight: 30, minWidth: 90);
        UIFactory.SetLayoutElement(clientIdField.GameObject, minHeight: 30, minWidth: 250);
        UIFactory.SetLayoutElement(accessTokenText.gameObject, minHeight: 30, minWidth: 90);
        UIFactory.SetLayoutElement(accessTokenField.GameObject, minHeight: 30, minWidth: 250);
        UIFactory.SetLayoutElement(saveButton.GameObject, minHeight: 30, minWidth: 60);
    }

    public override string Name { get; } = "Twitch Login";
    public override int MinWidth { get; } = 500;
    public override int MinHeight { get; } = 200;
    public override Vector2 DefaultAnchorMin { get; } = new Vector2(0.5f, 0.5f);
    public override Vector2 DefaultAnchorMax { get; } = new Vector2(0.5f, 0.5f);
}