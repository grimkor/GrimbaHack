using System;
using HarmonyLib;
using nway.collision;
using nway.gameplay;
using nway.gameplay.match;
using nway.gameplay.simulation;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public class CollisionBoxViewer : ModuleBase
{
    private static ColliderRenderer colliderRenderer { get; set; }

    public static CollisionBoxViewer Instance { get; private set; }

    static CollisionBoxViewer()
    {
        Instance = new CollisionBoxViewer();
    }

    private static bool _cameraInitialised;

    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (colliderRenderer)
            {
                colliderRenderer.enabled = value;
            }
        }
    }

    [HarmonyPatch(typeof(SimulationManager), nameof(SimulationManager.Initialize))]
    public class PatchCollisionShit
    {
        public static void Postfix()
        {
            if (!Instance.Enabled) return;
            if (!_cameraInitialised)
            {
                var stackCamera = GameObject.FindObjectOfType<StackCamera>();
                if (stackCamera == null) return;
                colliderRenderer = stackCamera.gameObject.AddComponent<ColliderRenderer>();
                colliderRenderer.isGlobalSleep = true;
                _cameraInitialised = true;
            }

            SceneStartup.instance.GamePlay._playerList[0].characterTeam.members
                .ForEach(new Action<Character>(new Action<Character>(member =>
                {
                    member.renderColliderList.Clear();
                })));
            SceneStartup.instance.GamePlay._playerList[0].renderColliderList = ColliderManager.instance.core.colliders;
        }
    }

    [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetupGamePlay))]
    public class PatchSetupGamePlay
    {
        public static void Postfix()
        {
            _cameraInitialised = false;
        }
    }

    public static void CreateUIControls(GameObject contentRoot)
    {
        var collisionBoxViewerGroup = UIFactory.CreateUIObject("CollisionBoxViewerGroup", contentRoot);
        UIFactory.SetLayoutElement(collisionBoxViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(collisionBoxViewerGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(collisionBoxViewerGroup, "CollisionBoxViewerToggle", out var collisionBoxViewerToggle,
            out var collisionBoxViewerToggleLabel);
        collisionBoxViewerToggle.isOn = false;
        collisionBoxViewerToggleLabel.text = "Enable Collision Box Viewer";
        collisionBoxViewerToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));

        UIFactory.SetLayoutElement(collisionBoxViewerToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}