using GrimbaHack.Modules.ComboRecorder;
using nway.ui;
using UnityEngine.Events;

namespace GrimbaHack.UI.Popup.TrainingSettings.Elements;

static class ComboRecorderExportSubmit
{
    public static void Generate(UIPage menuPage)
    {
        var button =
            menuPage.AddItem<MenuSubmit>("comboRecorderExportSubmit");
        button.Text = "Export Combo";
        button.SetOnSubmit((UnityAction<ILayeredEventData>)((ILayeredEventData data) =>
        {
            data.Use();
            ComboRecorderManager.Instance.ExportCombo();
        }));
    }
}
