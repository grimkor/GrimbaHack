using System;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using GrimbaHack.Modules;
using GrimbaHack.UI;
using HarmonyLib;
using nway.collision;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UniverseLib.UI;
using UniverseLib.UI.Panels;

namespace GrimbaHack;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    public static UIBase UiBase { get; private set; }
    private static PanelBase _panel;

    public override void Load()
    {
        Log = base.Log;
        UISetup.Init(this);
        var harmony = new Harmony("Base.Grimbakor.Mod");
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        CameraControl.Init();
        // AddComponent<PatchCollisionShit>();
    }
    //
    // [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    // public class PatchCollisionShit
    // {
    //     private static bool _cameraInitialised;
    //
    //     public static ColliderRenderer colliderRenderer { get; set; }
    //
    //     public static void Postfix()
    //     {
    //         if (!_cameraInitialised)
    //         {
    //             var stackCamera = GameObject.FindObjectOfType<StackCamera>();
    //             if (stackCamera == null) return;
    //             colliderRenderer = stackCamera.gameObject.AddComponent<ColliderRenderer>();
    //             colliderRenderer.isGlobalSleep = true;
    //             _cameraInitialised = true;
    //         }
    //         
    //         SceneStartup.instance.GamePlay._playerList[0].characterTeam.members
    //             .ForEach(new Action<Character>(member => { member.renderColliderList.Clear(); }));
    //         // ColliderManager.instance.core.colliders.ForEach(new Action<Collider>(x =>
    //         //     SceneStartup.instance.GamePlay._playerList[0].renderColliderList.Add(x)));
    //         
    //         SceneStartup.instance.GamePlay._playerList[0].renderColliderList = ColliderManager.instance.core.colliders;
    //     }

    // public static bool _boxesInitialised
    // {
    //     get { throw new System.NotImplementedException(); }
    //     set { throw new System.NotImplementedException(); }
    // }
    //
    //
    // // void Update()
    // // {
    // //     if (colliderRenderer == null || !_isInitialized) return;
    // //
    // //     foreach (var collider in ColliderManager.instance.core.colliders)
    // //     {
    // //         var t = typeof(CharacterHurtBox);
    // //         if (collider.owner.GetType() == typeof(CharacterHurtBox))
    // //         {
    // //             collider.owner.armor.ownerCharacter.renderColliderList.Add(collider);
    // //         }
    // //         
    // //         // Plugin.Log.LogInfo($"collider: {collider.owner.armor.ownerCharacter.attackerColliders}");
    // //         // colliderRenderer.Draw(collider.owner.armor.ownerCharacter.attackerColliders);
    // //         // collider.owner.armor.armorOwner.
    // //         // collider.owner.armor.ownerCharacter.RenderCollider(colliderRenderer);
    // //     }
    // // }
    // }
}