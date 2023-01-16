using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GrimbaHack.Data;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace GrimbaHack.Modules;

public sealed class BGMSelector : ModuleBase
{
    private BGMSelector()
    {
    }

    public static BGMSelector Instance { get; private set; }

    static BGMSelector()
    {
        Instance = new BGMSelector();
    }

    public static bool Enabled { get; set; } = false;
    public static List<string> BGMList { get; set; } = Global.BGMTracks.ToArray().Select(x => x.Value).ToList();
    public static string selectedBGM { get; set; } = BGMList[0];
    static Dropdown _bgmDropdown;
    static Stopwatch _stopwatch = new();
    static float trackLength;


    public static void CreateUIControls(GameObject contentRoot, out Action updateCallback)
    {
        var bgmParentVerticalGroup = UIFactory.CreateUIObject("BGMSelectorGroup", contentRoot);
        UIFactory.SetLayoutElement(bgmParentVerticalGroup);
        UIFactory.SetLayoutGroup<VerticalLayoutGroup>(bgmParentVerticalGroup, false, false, true, true, padLeft: 0,
            spacing: 10, childAlignment: TextAnchor.UpperCenter);

        // BUTTON HORIZONTAL GROUP
        var bgmParentHorizontalGroup = UIFactory.CreateUIObject("BGMSelectorGroup", bgmParentVerticalGroup);
        UIFactory.SetLayoutElement(bgmParentHorizontalGroup);
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(bgmParentHorizontalGroup, false, false, true, true, padLeft: 0,
            spacing: 10, childAlignment: TextAnchor.UpperCenter);

        // REWIND
        var rewindButton = UIFactory.CreateButton(bgmParentHorizontalGroup, "BgmsRewindButton", "<<");
        rewindButton.OnClick += PreviousTrackOrRewind;
        UIFactory.SetLayoutElement(rewindButton.GameObject, minWidth: 25, minHeight: 25);
        // PAUSE
        var pauseButton = UIFactory.CreateButton(bgmParentHorizontalGroup, "BgmsRewindButton", "Pause");
        pauseButton.OnClick += () =>
        {
            var audioManagerInstance = AudioManager.instance;
            if (audioManagerInstance != null)
            {
                audioManagerInstance.PauseBGM();
            }
        };
        UIFactory.SetLayoutElement(pauseButton.GameObject, minWidth: 75, minHeight: 25);

        // PLAY
        var playButton = UIFactory.CreateButton(bgmParentHorizontalGroup, "BgmsRewindButton", "Play");
        playButton.OnClick += PlayBGM;
        UIFactory.SetLayoutElement(playButton.GameObject, minWidth: 75, minHeight: 25);

        // NEXT
        var nextButton = UIFactory.CreateButton(bgmParentHorizontalGroup, "BgmsRewindButton", ">>");

        nextButton.OnClick += NextTrack;
        UIFactory.SetLayoutElement(nextButton.GameObject, minWidth: 25, minHeight: 25);

        // DROPDOWN
        UIFactory.CreateDropdown(bgmParentVerticalGroup, "BGMDropdown",
            out _bgmDropdown,
            selectedBGM,
            14,
            i =>
            {
                Plugin.Log.LogInfo($"i: {i}");
                Plugin.Log.LogInfo($"BGMList[i]: {BGMList?[i]}");
                SelectBGM(BGMList?[i]);
            },
            Global.BGMTracks.ToArray().Select(x => x.Label).ToArray()
        );
        UIFactory.SetLayoutElement(_bgmDropdown.gameObject, minHeight: 25, minWidth: 200);

        updateCallback = () =>
        {
            if (_stopwatch.IsRunning)
            {
                if (_stopwatch.Elapsed.TotalSeconds >= trackLength)
                {
                    NextTrack();
                }
            }
            else
            {
                try
                {
                    var trackTime = AudioManager.instance.bgmManager.bgmAssetList[$"BGM/{selectedBGM}"].audioClip
                        .length;
                    trackLength = trackTime;
                    if (trackTime > 0)
                    {
                        _stopwatch.Start();
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        };
    }

    private static void SelectBGM(string trackName)
    {
        selectedBGM = trackName;
    }

    private static void PlayBGM()
    {
        Plugin.Log.LogInfo("PlayBGM");
        
        AudioManager.instance.StopBGM();
        AudioManager.instance.PlayBGM(selectedBGM);

        _stopwatch.Stop();
        _stopwatch.Reset();
    }

    private static void NextTrack()
    {
        var audioManagerInstance = AudioManager.instance;
        if (audioManagerInstance != null)
        {
            var currentBGMIndex = BGMList.IndexOf(selectedBGM);
            if (currentBGMIndex == BGMList.Count - 1)
            {
                currentBGMIndex = 0;
            }
            else
            {
                currentBGMIndex++;
            }

            selectedBGM = BGMList[currentBGMIndex];
            _bgmDropdown.value = currentBGMIndex;
            SelectBGM(selectedBGM);
            PlayBGM();
        }
    }

    private static void PreviousTrackOrRewind()
    {
        var audioManagerInstance = AudioManager.instance;
        if (audioManagerInstance != null)
            if (trackLength > 0 && _stopwatch.Elapsed.TotalSeconds > 5)
            {
                _stopwatch.Stop();
                _stopwatch.Reset();
                PlayBGM();
            }
            else
            {
                var currentBGMIndex = BGMList.IndexOf(selectedBGM);
                if (currentBGMIndex == 0)
                {
                    currentBGMIndex = BGMList.Count - 1;
                }
                else
                {
                    currentBGMIndex--;
                }

                selectedBGM = BGMList[currentBGMIndex];
                _bgmDropdown.value = currentBGMIndex;
                SelectBGM(selectedBGM);
                PlayBGM();
            }
    }
}