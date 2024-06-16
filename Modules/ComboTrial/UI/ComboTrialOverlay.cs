using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using epoch.db;
using GrimbaHack.Utility;
using nway.gameplay.match;
using UnityEngine;
using UnityEngine.UI;
using Action = System.Action;
using Object = UnityEngine.Object;

namespace GrimbaHack.Modules.ComboTrial.UI;

public class ComboTrialOverlay
{
    public static readonly ComboTrialOverlay Instance = new();
    private List<ComboItemUIElementGroup> _icons = new();
    private GameObject _rootGameObject;
    private int _comboLocation;
    private List<Transform> buttonRows = new();
    private GameObject _overlayObject;
    private GameObject _animationGo;
    private Action _completeAnimation;
    private List<List<ComboItem>> _combo;

    static ComboTrialOverlay()
    {
        OnEnterMainMenuActionHandler.Instance.AddCallback(() => { Instance.Teardown(); });

        OnSimulationInitializeActionHandler.Instance.AddPostfix(() =>
        {
            if (!ComboTrialManager.Instance.IsComboTrial) return;
            Instance.Restart();
        });
    }

    public void Teardown()
    {
        Instance._comboLocation = 0;
        if (Instance._overlayObject != null)
        {
            Object.DestroyObject(Instance._overlayObject);
            Instance._overlayObject = null;
        }

        Instance._icons.Clear();
    }

    public void Init(List<List<ComboItem>> combo)
    {
        Instance.Teardown();
        Instance._comboLocation = 0;
        var filtered = new List<List<ComboItem>>();

        foreach (var row in combo)
        {
            var filteredRow = new List<ComboItem>();
            foreach (var comboItem in row)
            {
                var filteredItems = comboItem.Items.Where(item => { return item[1] != ""; }).ToList();
                if (filteredItems.Count > 0)
                {
                    filteredRow.Add(new()
                    {
                        Items = filteredItems,
                        Repeat = comboItem.Repeat
                    });
                }
            }

            if (filteredRow.Count > 0)
            {
                filtered.Add(filteredRow);
            }
        }

        Instance._combo = filtered;
        Instance.RenderRootObject();
    }


    public void Complete()
    {
        Instance._completeAnimation();
    }

    public void Restart()
    {
        if (!ComboTrialManager.Instance.IsComboTrial || Instance._overlayObject == null) return;
        Instance._comboLocation = 0;
        Instance._icons.ForEach(item => item.Reset());
        Instance._animationGo.SetActive(false);
    }


    public void Show()
    {
        if (!ComboTrialManager.Instance.IsComboTrial) return;
        if (GameManager.instance.appStateManager.state != AppState.Combat &&
            MatchManager.instance.matchType != MatchType.Training) return;

        Instance._overlayObject.SetActive(true);
    }

    private void RenderRootObject()
    {
        Instance._rootGameObject = new("grimui_combotrial_rootGO");
        Object.DontDestroyOnLoad(Instance._rootGameObject);
        var canvas = Instance._rootGameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        Instance._rootGameObject.AddComponent<CanvasScaler>();

        Instance._overlayObject = new GameObject("grimui_combotrial_overlay");
        Instance._overlayObject.transform.SetParent(Instance._rootGameObject.transform);

        var overlayLayout = Instance._overlayObject.AddComponent<VerticalLayoutGroup>();
        overlayLayout.childControlHeight = true;
        overlayLayout.childControlWidth = true;
        overlayLayout.childForceExpandHeight = false;
        overlayLayout.childForceExpandWidth = false;
        overlayLayout.childScaleHeight = false;
        overlayLayout.childScaleWidth = false;
        overlayLayout.childAlignment = TextAnchor.MiddleLeft;
        overlayLayout.spacing = 16;

        var rootSizeFitter = Instance._overlayObject.AddComponent<ContentSizeFitter>();
        rootSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        rootSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var background = new GameObject("grimui_overlay_background");
        background.transform.SetParent(Instance._overlayObject.transform);
        var layoutElement = background.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        var bgRect = background.GetComponent<RectTransform>();
        if (bgRect != null)
        {
            bgRect.anchorMin = new(0.5f, 0);
            bgRect.anchorMax = new(0.5f, 1);
            bgRect.localPosition = new(0, 0, 0);
        }

        var leftBg = new GameObject("grimui_overlay_bg_left");
        leftBg.transform.SetParent(background.transform);
        var leftBgImage = leftBg.AddComponent<Image>();
        leftBgImage.sprite = SpriteMap.Instance.GetMapping("panel_streak");
        leftBgImage.type = Image.Type.Sliced;
        leftBgImage.color = new(1, 1, 1, 0.9f);
        var leftBgRect = leftBg.GetComponent<RectTransform>();
        if (leftBgRect != null)
        {
            leftBgRect.pivot = new(0, 0.5f);
            leftBgRect.anchorMin = new(0.5f, 0);
            leftBgRect.anchorMax = new(0.5f, 1);
            leftBgRect.offsetMin = new(0, 20);
            leftBgRect.offsetMax = new(800, -20);
            leftBgRect.localScale = new(-1, 1, 1);
        }

        var rightBg = new GameObject("grimui_overlay_bg_right");
        rightBg.transform.SetParent(background.transform);
        var rightBgImage = rightBg.AddComponent<Image>();
        rightBgImage.sprite = SpriteMap.Instance.GetMapping("panel_streak");
        rightBgImage.type = Image.Type.Sliced;
        rightBgImage.color = new(1, 1, 1, 0.9f);
        var rightBgRect = rightBg.GetComponent<RectTransform>();
        if (rightBgRect != null)
        {
            rightBgRect.pivot = new(0, 0.5f);
            rightBgRect.anchorMin = new(0.5f, 0);
            rightBgRect.anchorMax = new(0.5f, 1);
            rightBgRect.offsetMin = new(0, 20);
            rightBgRect.offsetMax = new(800, -20);
            rightBgRect.localScale = new(1, 1, 1);
        }

        Instance.buttonRows = new();
        for (int rowCount = 0; rowCount < Instance._combo.Count; rowCount++)
        {
            var row = new GameObject($"grimui_inputList_{rowCount}");
            row.transform.SetParent(Instance._overlayObject.transform);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;
            layout.childScaleHeight = false;
            layout.childScaleWidth = false;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.spacing = 16;
            Instance.buttonRows.Add(row.transform);
        }

        var rowIndex = 0;
        foreach (var row in Instance._combo)
        {
            foreach (var comboItem in row)
            {
                var comboIcon = new ComboItemUIElementGroup(comboItem, buttonRows[rowIndex]);
                Instance._icons.Add(comboIcon);
            }

            rowIndex++;
        }

        var rect = Instance._overlayObject.GetComponent<RectTransform>();
        if (rect is not null)
        {
            rect.localPosition = new(0, 0, 0);
            rect.pivot = new(0.5f, 0);
            rect.anchoredPosition = new(0, 95);
            rect.anchorMin = new(0.5f, 0);
        }

        Instance._animationGo = new GameObject("grimui_complete_animation");
        Instance._animationGo.transform.SetParent(Instance._rootGameObject.transform);
        Instance._animationGo.AddComponent<LayoutElement>().ignoreLayout = true;
        var animationRect = Instance._animationGo.transform.GetComponent<RectTransform>();
        if (animationRect is not null)
        {
            animationRect.localPosition = Vector3.zero;
            animationRect.anchoredPosition = new(0, 0);
        }

        var boltSpriteGo = new GameObject("grimui_complete_animation_bolt");
        boltSpriteGo.transform.SetParent(Instance._animationGo.transform);
        var boltSprite = boltSpriteGo.AddComponent<Image>();
        boltSprite.sprite = SpriteMap.Instance.GetMapping("bolt");
        boltSprite.preserveAspect = true;
        var boltRect = boltSpriteGo.GetComponent<RectTransform>();
        if (boltRect is not null)
        {
            boltRect.localPosition = Vector3.zero;
            boltRect.anchorMin = Vector2.zero;
            boltRect.anchorMax = Vector2.one;
        }

        var textGo = new GameObject("grimui_complete_animation_text");
        textGo.transform.SetParent(Instance._animationGo.transform);
        var textSizeFitter = textGo.AddComponent<ContentSizeFitter>();
        textSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        var text = textGo.AddComponent<Text>();
        text.text = "TRIAL COMPLETE!";
        text.font = FontAssetManager.Instance.SuperFont;
        text.fontSize = 72;

        text.transform.localScale = new(1.25f, 1.25f, 1.25f);

        textGo.AddComponent<Outline>();

        var tRect = textGo.GetComponent<RectTransform>();
        if (tRect is not null)
        {
            tRect.anchoredPosition = Vector2.zero;
            tRect.anchorMin = new(0.5f, 0.5f);
            tRect.anchorMax = new(0.5f, 0.5f);
        }

        Instance._completeAnimation = () =>
        {
            Instance._animationGo.SetActive(true);
            var boltSequence = DOTween.Sequence();
            textGo.transform.localPosition = new(2000, 0, 0);
            boltSpriteGo.transform.localScale = new Vector3(20, 20, 20);
            boltSprite.color = new(1, 1, 1, 1);
            text.color = new(1, 1, 1, 1);
            boltSequence.Append(boltSpriteGo.transform.DOScale(new Vector3(1.75f, 1.75f, 1.75f), .45f)
                .SetEase(Ease.OutQuart));
            boltSequence.Insert(0.35f, textGo.transform.DOLocalMove(new Vector3(0, 0, 0), .4f).SetEase(Ease.OutBack));
            boltSequence.AppendInterval(2);
            var fadeoutSequence = DOTween.Sequence();
            fadeoutSequence.Append(text.DOColor(new(1, 1, 1, 0), 0.5f));
            fadeoutSequence.Insert(0, boltSprite.DOColor(new(1, 1, 1, 0), 0.5f));
            boltSequence.Append(fadeoutSequence);
        };

        Instance._animationGo.SetActive(false);
    }

    public bool CheckAttack(string attackName)
    {
        if (Instance._comboLocation > Instance._icons.Count - 1) return false;

        if (Instance._icons[Instance._comboLocation].CheckAttack(attackName))
        {
            Instance._comboLocation++;
            if (Instance._comboLocation > Instance._icons.Count - 1)
            {
                return true;
            }
        }

        return false;
    }
}

public class ComboItem
{
    public List<List<string>> Items = new();

    public int Repeat = 1;

    public List<string> GetIds() => Items.Select(x => x[0]).ToList();
    public List<string> GetNotation() => Items.Select(x => x[1]).ToList();
}

public class ComboItemOld
{
    public List<string> Ids = new();
    public List<string> Notation = new();

    public int Repeat = 1;
}
