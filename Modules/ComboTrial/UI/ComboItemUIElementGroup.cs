using System.Collections.Generic;
using GrimbaHack.Modules.Combo;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules.ComboTrial.UI;

public class ComboItemUIElementGroup : ComboItem
{
    private readonly List<ComboItemUIElement> _uiElements;
    private readonly List<ComboItemUIElement> _uiBrackets = new();
    private ComboItemUIElement _uiRepeatCounter;
    private int _currentComboPoint;
    private int _currentRepeatCount = 1;
    private readonly Transform _parentTransform;
    private GameObject _groupContainer;

    public ComboItemUIElementGroup(ComboItem comboItem, Transform parentObject)
    {
        Items = comboItem.Items;
        Repeat = comboItem.Repeat;
        _uiElements = new();
        _parentTransform = parentObject;

        SetupLayoutComponents();
        GenerateIcons();
    }

    private void SetupLayoutComponents()
    {
        _groupContainer = new GameObject($"grimui_button_container");
        _groupContainer.transform.SetParent(_parentTransform);

        var layout = _groupContainer.AddComponent<HorizontalLayoutGroup>();
        layout.childControlHeight = true;
        layout.childControlWidth = false;
        layout.childScaleHeight = false;
        layout.childScaleWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 0;
        layout.padding = new(0, 0, 0, 0);

        var sizeFitter = _groupContainer.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        var containerRect = _groupContainer.GetComponent<RectTransform>();
        {
            containerRect.sizeDelta = new() { y = 40 };
            containerRect.localScale = new(1, 1, 1);
            containerRect.position = new(1, 1, 1);
        }
    }

    private void GenerateIcons()
    {
        if (Repeat > 1)
        {
            _uiBrackets.Add(new ComboItemUIElement(_groupContainer, "("));
        }

        foreach (var item in GetNotation())
        {
            _uiElements.Add(new ComboItemUIElement(_groupContainer, item));
        }

        if (Repeat > 1)
        {
            _uiBrackets.Add(new ComboItemUIElement(_groupContainer, ")"));
            _uiRepeatCounter = new ComboItemUIElement(_groupContainer, $"x");
            _uiRepeatCounter.SetText($"x{Repeat}");
        }
    }

    public bool CheckAttack(string attackName)
    {
        if (GetIds()[_currentComboPoint] != ComboQuickConverter.SanitiseAttackName(attackName)) return false;
        Next();
        // example Ids.Count = 3 - 1 == 2
        // 0 < 2 return false, more to match
        // 1 < 2 return false, more to match
        // 2 < 2 nothing more to match, see if done or repeats
        if (_currentComboPoint++ < GetIds().Count - 1) return false;

        if (_currentRepeatCount < Repeat)
        {
            _currentComboPoint = 0;
            ResetNotationStyles();
            _uiRepeatCounter?.SetText($"x{Repeat - _currentRepeatCount}");
            _currentRepeatCount++;
            return false;
        }

        _uiRepeatCounter?.SetText($"x{Repeat - _currentRepeatCount}");
        _uiBrackets.ForEach(bracket => bracket.SetComplete(true, false));
        _uiRepeatCounter?.SetComplete(true);
        return true;
    }

    private void Next()
    {
        _uiElements[_currentComboPoint].SetComplete(true);
    }

    public void Reset()
    {
        _currentComboPoint = 0;
        _currentRepeatCount = 1;
        _uiRepeatCounter?.SetText($"x{Repeat}");
        ResetNotationStyles();
    }

    private void ResetNotationStyles()
    {
        _uiElements.ForEach(item => item.SetComplete(false));
        _uiBrackets.ForEach(bracket => bracket.SetComplete(false));
        _uiRepeatCounter?.SetComplete(false);
    }

    private Il2CppArrayBase<CanvasRenderer> GetAllCanvasRenderers()
    {
        return _parentTransform.GetComponents<CanvasRenderer>();
    }

    public void Destroy()
    {
        Object.Destroy(_groupContainer);
    }
}
