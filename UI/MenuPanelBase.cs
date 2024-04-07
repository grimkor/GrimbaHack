using GrimbaHack.Data;
using GrimbaHack.UI.Global;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI;

public abstract class MenuPanelBase : PanelBase
{
    protected MenuPanelBase(UIBase owner) : base(owner)
    {
    }

    public abstract PanelTypes PanelType { get; }
    public ButtonRef MenuButton;
    public LayoutElement layoutElement { get; set; }

    public void SetButtonVisible(bool visible)
    {
        layoutElement.enabled = visible;
        MenuButton.ButtonText.enabled = visible;
        if (!visible)
        {
            UIRoot.SetActive(false);
            LegacyUIManager.RefreshUI();
        }
    }

    public override void ConstructUI()
    {
        base.ConstructUI();

        MenuButton = UIFactory.CreateButton(Toolbar.ButtonContainer, $"{Name}_Button", Name);
        Toolbar.Instance._panels.Add(PanelType, this);
        MenuButton.OnClick += () => { LegacyUIManager.TogglePanel(PanelType); };
        layoutElement =
            UIFactory.SetLayoutElement(MenuButton.GameObject, minHeight: 25, minWidth: 50, preferredWidth: 150);
        MenuButton.Enabled = true;
    }
}