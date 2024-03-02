using System;
using System.Collections.Generic;
using HarmonyLib;

namespace GrimbaHack.Utility;

[HarmonyPatch(typeof(Armor), nameof(Armor.TakeDamage))]
public class OnArmorTakeDamageCallbackHandler
{
    private OnArmorTakeDamageCallbackHandler()
    {
    }

    static OnArmorTakeDamageCallbackHandler()
    {
        Instance = new OnArmorTakeDamageCallbackHandler();
    }
    public static OnArmorTakeDamageCallbackHandler Instance { get; set; }
    private List<Action<DamageInfo>> callbacks = new();

    public void AddCallback(Action<DamageInfo> callback)
    {
        Instance.callbacks.Add(callback);
    }
    
    public static void Prefix(DamageInfo damageInfo)
    {
        foreach (var callback in Instance.callbacks)
        {
            callback(damageInfo);
        }
    }
}