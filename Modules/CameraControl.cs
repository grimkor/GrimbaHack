﻿using System;
using GrimbaHack.Modules;
using GrimbaHack.UI;
using Il2CppInterop.Runtime.Injection;
using nway.ui;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniverseLib.UI;
using Object = UnityEngine.Object;

namespace GrimbaHack;

public sealed class CameraControl : ModuleBase
{
    private CameraControl()
    {
    }

    public static CameraControl Instance { get; private set; }
    private CameraControlBehaviour Behaviour;

    static CameraControl()
    {
        Instance = new CameraControl();
        ClassInjector.RegisterTypeInIl2Cpp<CameraControlBehaviour>();
        GameObject go = new GameObject("CameraControlBehaviour");
        Object.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        Instance.Behaviour = go.AddComponent<CameraControlBehaviour>();
        Instance.Enabled = false;
    }


    public bool Enabled
    {
        get => Behaviour.enabled;
        set => Behaviour.SetEnabled(value);
    }

    public static CameraControlHelpWindow HelpWindow;

    public static void CreateUIControls(GameObject contentRoot)
    {
        HelpWindow = new CameraControlHelpWindow(UIManager.UIBase);
        var cameraControlGroup = UIFactory.CreateUIObject("CameraControlGroup", contentRoot);
        UIFactory.SetLayoutElement(cameraControlGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(cameraControlGroup, false, false, true, true, padLeft: 25,
            spacing: 10, childAlignment: TextAnchor.MiddleLeft);

        // Enable Toggle
        UIFactory.CreateToggle(cameraControlGroup, "CameraControlToggle", out Toggle CameraControlToggle,
            out Text CameraControlToggleText, checkHeight: 20, checkWidth: 20);
        CameraControlToggle.enabled = true;
        CameraControlToggle.isOn = false;
        CameraControlToggle.onValueChanged.AddListener(new Action<bool>(enabled => { Instance.Enabled = enabled; }));

        // Toggle Text
        CameraControlToggleText.text = "Enable Freecam";

        // Help Button
        var button = UIFactory.CreateButton(cameraControlGroup, "CameraControlHelpButton", "?");
        button.OnClick += () => { HelpWindow.Toggle(); };

        // Layout elements
        UIFactory.SetLayoutElement(CameraControlToggle.gameObject);
        UIFactory.SetLayoutElement(button.GameObject, minHeight: 25, minWidth: 25);
    }
}

public class CameraControlBehaviour : MonoBehaviour
{
    static Camera _camera;
    internal static CameraControlBehaviour Instance { get; private set; }

    public void SetEnabled(bool enable)
    {
        enabled = enable;
        if (!enable && _camera != null)
        {
            _camera.enabled = false;
        }
    }


    void Update()
    {
        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            if (!_camera)
            {
                var go = new GameObject();
                _camera = go.AddComponent<Camera>();
                _camera.depth = 100;
                _camera.enabled = true;
                _camera.backgroundColor = Color.green;
            }
            else
            {
                _camera.enabled = true;
            }

            _camera.transform.position = new Vector3(0, 0, 0);
            _camera.transform.rotation = new Quaternion(0, 0, 0, 0);
        }

        if (Keyboard.current.f10Key.wasPressedThisFrame)
        {
            if (_camera)
            {
                _camera.enabled = false;
            }
        }

        {
            if (_camera)
            {
                // forward
                if (Keyboard.current.yKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.forward * (Time.deltaTime * 4));
                }

                // back
                if (Keyboard.current.hKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.back * (Time.deltaTime * 4));
                }

                // left
                if (Keyboard.current.gKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.left * (Time.deltaTime * 4));
                }

                // right
                if (Keyboard.current.jKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.right * (Time.deltaTime * 4));
                }

                // up
                if (Keyboard.current.tKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.up * (Time.deltaTime * 4));
                }

                // down
                if (Keyboard.current.uKey.isPressed)
                {
                    _camera.transform.Translate(Vector3.down * (Time.deltaTime * 4));
                }

                // look up
                if (Keyboard.current.oKey.isPressed)
                {
                    _camera.transform.Rotate(new Vector3(-1, 0, 0));
                }

                // look down
                if (Keyboard.current.lKey.isPressed)
                {
                    _camera.transform.Rotate(new Vector3(1, 0, 0));
                }

                // look left
                if (Keyboard.current.kKey.isPressed)
                {
                    _camera.transform.Rotate(new Vector3(0, -1, 0));
                }

                // look right
                if (Keyboard.current.semicolonKey.isPressed)
                {
                    _camera.transform.Rotate(new Vector3(0, 1, 0));
                }
            }
        }
    }
}