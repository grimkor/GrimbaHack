using System.Linq;
using GrimbaHack.module;
using GrimbaHack.Utility;
using UnityEngine;

namespace GrimbaHack.Modules;

static class CommandHistoryFix
{
    // private static readonly ButtonSpriteMapping SpriteMapping = new();
    private static bool _loaded;

    public static void Init()
    {
    }

    static CommandHistoryFix()
    {
        OnCommandHistoryItemUpdateActionButton.Instance.AddPostfix((button, mask, _, _, _) =>
        {
            if ((mask & PlayerButton.Fire1) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("L");
                button.image.sprite = SpriteMap.Instance.GetMapping("L");
                return;
            }

            if ((mask & PlayerButton.Fire2) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("M");
                button.image.sprite = SpriteMap.Instance.GetMapping("M");
                return;
            }

            if ((mask & PlayerButton.Fire3) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("H");
                button.image.sprite = SpriteMap.Instance.GetMapping("H");
                return;
            }

            if ((mask & PlayerButton.Ability1) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("S");
                button.image.sprite = SpriteMap.Instance.GetMapping("S");
                return;
            }

            if ((mask & PlayerButton.Assist1) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("A1");
                button.image.sprite = SpriteMap.Instance.GetMapping("A1");
                return;
            }

            if ((mask & PlayerButton.Assist2) != 0)
            {
                button.image.overrideSprite = SpriteMap.Instance.GetMapping("A2");
                button.image.sprite = SpriteMap.Instance.GetMapping("A2");
            }
        });
    }
}
