using System;
using System.Collections.Generic;
using GrimbaHack.Utility;
using nway.collision;
using nway.gameplay;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules;

public class CollisionBoxViewer : ModuleBase
{
    public static CollisionBoxViewer Instance { get; private set; }
    private static List<Toggle> toggleInstances = new();

    static CollisionBoxViewer()
    {
        Instance = new();
        OnEnterTrainingMatchActionHandler.Instance.AddPostfix(() => Instance.Enabled = Instance._enabled);
        OnEnterPremadeMatchActionHandler.Instance.AddCallback(() => Instance.Enabled = Instance._enabled);
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => CollisionBoxViewerController.Enabled = false);
    }

    private static bool _cameraInitialised;

    private bool _enabled;

    public bool Enabled
    {
        get => Instance._enabled;
        set
        {
            Instance._enabled = value;
            CollisionBoxViewerController.Enabled = value;
            toggleInstances.ForEach((toggle => toggle.isOn = value));
        }
    }


    public static void CreateUIControls(GameObject contentRoot)
    {
        var collisionBoxViewerGroup = UIFactory.CreateUIObject("CollisionBoxViewerGroup", contentRoot);
        UIFactory.SetLayoutElement(collisionBoxViewerGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(collisionBoxViewerGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);
        UIFactory.CreateToggle(collisionBoxViewerGroup, "CollisionBoxViewerToggle",
            out var collisionBoxViewerToggle,
            out var collisionBoxViewerToggleLabel);
        collisionBoxViewerToggle.isOn = Instance._enabled;
        collisionBoxViewerToggleLabel.text = "Enable Collision Box Viewer";
        collisionBoxViewerToggle.onValueChanged.AddListener(new Action<bool>((value) => { Instance.Enabled = value; }));
        toggleInstances.Add(collisionBoxViewerToggle);
        UIFactory.SetLayoutElement(collisionBoxViewerToggle.gameObject, minHeight: 25, minWidth: 50);
    }
}

public static class CollisionBoxViewerController
{
    static CollisionBoxViewerController()
    {
        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (Enabled)
            {
                InitCamera();
                MapColliders();
            }
            else
            {
                ClearColliders();
            }
        });

        OnMatchManagerSetupGamePlay.Instance.AddCallback((match, pc, x) =>
        {
            _cameraInitialised = false;
            if (Enabled)
                InitCamera();
        });
    }

    private static bool _enabled;

    public static bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            if (value)
            {
                InitCamera();
                SetCamera(true);
                MapColliders();
            }
            else
            {
                SetCamera(false);
                ClearColliders();
            }
        }
    }

    public static bool _cameraInitialised;
    private static ColliderRenderer colliderRenderer { get; set; }

    public static void InitCamera()
    {
        if (!_cameraInitialised)
        {
            var stackCamera = Object.FindObjectOfType<StackCamera>();
            if (stackCamera == null) return;
            colliderRenderer = stackCamera.gameObject.AddComponent<ColliderRenderer>();
            colliderRenderer.isGlobalSleep = true;
            _cameraInitialised = true;
        }
    }

    public static void SetCamera(bool enable)
    {
        if (colliderRenderer)
        {
            colliderRenderer.enabled = enable;
        }
    }

    public static void ClearColliders()
    {
        if (SceneStartup.instance == null) return;
        SceneStartup.instance.GamePlay._playerList[0].characterTeam.members
            .ForEach(new Action<Character>(member => { member.renderColliderList.Clear(); }));
    }

    public static void MapColliders()
    {
        ClearColliders();
        SceneStartup.instance.GamePlay._playerList[0].renderColliderList = ColliderManager.instance.core.colliders;
    }
}
