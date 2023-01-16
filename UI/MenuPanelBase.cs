using GrimbaHack.Data;
using GrimbaHack.UI.Global;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack.UI;

public abstract class MenuPanelBase : PanelBase
{
    protected MenuPanelBase(UIBase owner) : base(owner)
    {
    }

    public abstract PanelTypes PanelType { get; }

    public override void ConstructUI()
    {
        base.ConstructUI();

        var button = UIFactory.CreateButton(Toolbar.ButtonContainer, $"{Name}_Button", Name);
        button.OnClick += () => { UIManager.TogglePanel(PanelType); };
        UIFactory.SetLayoutElement(button.GameObject, minHeight: 25, minWidth: 50, preferredWidth: 150);
    }
}