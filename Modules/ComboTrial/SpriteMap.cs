using System.Collections.Generic;
using UnityEngine;

namespace GrimbaHack.module;

public class SpriteMap
{
    private SpriteMap()
    {
    }

    public static SpriteMap Instance = new();
    private bool _loaded;

    private readonly Dictionary<string, Sprite> Map = new()
    {
        { "L", new() },
        { "M", new() },
        { "H", new() },
        { "S", new() },
        { "1", new() },
        { "2", new() },
        { "3", new() },
        { "4", new() },
        { "5", new() },
        { "6", new() },
        { "7", new() },
        { "8", new() },
        { "9", new() },
        { "+", new() },
        { ">", new() },
        { "A1", new() },
        { "A2", new() },
        {"panel_streak", new()},
        {"bolt", new()}
    };


    public Sprite GetMapping(string key)
    {
        if (Instance.Map.TryGetValue(key, out var value))
        {
            return value;
        }

        return new();
    }

    public void SetMapping(string key, Sprite sprite)
    {
        if (Instance.Map.ContainsKey(key))
        {
            Instance.Map.Remove(key);
        }

        Instance.Map.Add(key, sprite);
    }

    public void GenerateSpriteMap()
    {
        if (_loaded) return;
        var obj = Resources.FindObjectsOfTypeAll<Sprite>();

        foreach (var sprite in obj)
        {
            if (sprite.name.Contains("button_pc_light"))
            {
                SpriteMap.Instance.SetMapping("L", sprite);
            }

            if (sprite.name.Contains("button_pc_medium"))
            {
                SpriteMap.Instance.SetMapping("M", sprite);
            }

            if (sprite.name.Contains("button_pc_heavy"))
            {
                SpriteMap.Instance.SetMapping("H", sprite);
            }

            if (sprite.name.Contains("button_pc_special"))
            {
                SpriteMap.Instance.SetMapping("S", sprite);
            }

            if (sprite.name.Contains("button_pc_assist_1"))
            {
                SpriteMap.Instance.SetMapping("A1", sprite);
            }

            if (sprite.name.Contains("button_pc_assist_2"))
            {
                SpriteMap.Instance.SetMapping("A2", sprite);
            }

            if (sprite.name == "arrow_toggle")
            {
                SpriteMap.Instance.SetMapping(">", sprite);
            }

            if (sprite.name == "button_xbox_dpad_down_left")
            {
                SpriteMap.Instance.SetMapping("1", sprite);
            }

            if (sprite.name == "button_xbox_dpad_down")
            {
                SpriteMap.Instance.SetMapping("2", sprite);
            }

            if (sprite.name == "button_xbox_dpad_down_right")
            {
                SpriteMap.Instance.SetMapping("3", sprite);
            }

            if (sprite.name == "button_xbox_dpad_left")
            {
                SpriteMap.Instance.SetMapping("4", sprite);
            }

            if (sprite.name == "button_xbox_dpad_right")
            {
                SpriteMap.Instance.SetMapping("6", sprite);
            }

            if (sprite.name == "button_xbox_dpad_up_left")
            {
                SpriteMap.Instance.SetMapping("7", sprite);
            }

            if (sprite.name == "button_xbox_dpad_up")
            {
                SpriteMap.Instance.SetMapping("8", sprite);
            }

            if (sprite.name == "button_xbox_dpad_up_right")
            {
                SpriteMap.Instance.SetMapping("9", sprite);
            }

            if (sprite.name == "icon_plus")
            {
                SpriteMap.Instance.SetMapping("+", sprite);
            }

            if (sprite.name == "panel_streak")
            {
                SpriteMap.Instance.SetMapping("panel_streak", sprite);
            }
            if (sprite.name == "bolt")
            {
                SpriteMap.Instance.SetMapping("bolt", sprite);
            }
        }

        _loaded = true;
    }
}
