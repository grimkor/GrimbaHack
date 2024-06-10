using GrimbaHack.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules;

public class FontAssetManager
{
    public static FontAssetManager Instance = new();
    private bool _loaded;
    public Font DefaultFont;
    public Font SuperFont { get; private set; }
    public Font OverlayFont { get; private set; }

    static FontAssetManager()
    {
    }

    public static void Init()
    {
        if (Instance._loaded) return;
        OnEnterMainMenuActionHandler.Instance.AddCallback(() =>
        {
            if (Instance._loaded) return;
            foreach (var font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font.name.ToLower() == "mgs76")
                {
                    Instance.SuperFont = font;
                }

                if (font.name.ToLower() == "route159-semibold")
                {
                    Instance.OverlayFont = font;
                }
            }

            var go = new GameObject("grimui_temp_super_font");
            var text = go.AddComponent<Text>();
            text.AssignDefaultFont();
            Instance.DefaultFont = text.font;
            Object.Destroy(go);
            if (Instance.SuperFont == null)
            {
                Instance.SuperFont = Instance.DefaultFont;
            }

            if (Instance.OverlayFont == null)
            {
                Instance.SuperFont = Instance.DefaultFont;
            }

            Instance._loaded = true;
        });
    }
}
