using System.Linq;
using GrimbaHack.Utility;
using UnityEngine;

namespace GrimbaHack.Modules;

static class CommandHistoryFix
{
    private static readonly ButtonSpriteMapping SpriteMapping = new();

    public static void Init()
    {
    }

    static CommandHistoryFix()
    {
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() =>
        {
            var obj = Object.FindObjectsOfType<Sprite>();

            foreach (var sprite in obj)
            {
                if (sprite.name.Contains("button_pc_light"))
                {
                    SpriteMapping.light = sprite;
                }

                if (sprite.name.Contains("button_pc_medium"))
                {
                    SpriteMapping.medium = sprite;
                }

                if (sprite.name.Contains("button_pc_heavy"))
                {
                    SpriteMapping.heavy = sprite;
                }

                if (sprite.name.Contains("button_pc_special"))
                {
                    SpriteMapping.special = sprite;
                }

                if (sprite.name.Contains("button_pc_special"))
                {
                    SpriteMapping.special = sprite;
                }

                if (sprite.name.Contains("button_pc_assist_1"))
                {
                    SpriteMapping.assist1 = sprite;
                }

                if (sprite.name.Contains("button_pc_assist_2"))
                {
                    SpriteMapping.assist2 = sprite;
                }
            }
        });

        OnCommandHistoryItemUpdateActionButton.Instance.AddPostfix((button, mask, _, _, _) =>
            {
                var obj = Object.FindObjectsOfType<Sprite>();
                obj.ToList().First(x =>
                {
                    if ((mask & PlayerButton.Fire1) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.light;
                        button.image.sprite = SpriteMapping.light;
                        return x;
                    }

                    if ((mask & PlayerButton.Fire2) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.medium;
                        button.image.sprite = SpriteMapping.medium;
                        return x;
                    }

                    if ((mask & PlayerButton.Fire3) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.heavy;
                        button.image.sprite = SpriteMapping.heavy;
                        return x;
                    }

                    if ((mask & PlayerButton.Ability1) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.special;
                        button.image.sprite = SpriteMapping.special;
                        return x;
                    }

                    if ((mask & PlayerButton.Assist1) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.assist1;
                        button.image.sprite = SpriteMapping.assist1;
                        return x;
                    }

                    if ((mask & PlayerButton.Assist2) != 0)
                    {
                        button.image.overrideSprite = SpriteMapping.assist2;
                        button.image.sprite = SpriteMapping.assist2;
                        return x;
                    }

                    return false;
                });
            }
        );
    }

    private class ButtonSpriteMapping
    {
        public Sprite light;
        public Sprite medium;
        public Sprite heavy;
        public Sprite special;
        public Sprite assist1;
        public Sprite assist2;
    }
}