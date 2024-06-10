using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.Modules.ComboTrial.UI;

public class ComboItemUIElement
{
    private readonly string _notation;
    private readonly List<CanvasRenderer> _canvasRenderers;
    private readonly GameObject _buttonContainer;
    private Text text;
    private SimpleSpriteAnimation _animation;

    public ComboItemUIElement(GameObject buttonContainer, string notation)
    {
        _notation = notation;
        _buttonContainer = buttonContainer;
        _canvasRenderers = new();
        Generate();
    }

    public void SetComplete(bool completed, bool animate = true)
    {
        if (completed)
        {
            _canvasRenderers.ForEach(cr =>
            {
                if (animate)
                {
                    var sequence = new Sequence();
                    sequence.Append(cr.transform.DOLocalMoveY(8, 0.1f).SetEase(Ease.Linear)
                        .OnComplete((TweenCallback)(() => { cr.transform.DOLocalMoveY(0, 0.01f); })));

                    sequence.Join(cr.transform.DOScale(1.2f, 0.1f).SetEase(Ease.Linear).OnComplete((TweenCallback)(() =>
                    {
                        cr.transform.DOScale(1, 0.01f);
                    })));
                    sequence.Play();
                }

                cr.SetAlpha(0.25f);
            });
        }
        else
        {
            _canvasRenderers.ForEach(cr => cr.SetAlpha(1));
        }
    }

    private void Generate()
    {
        if (_notation.ToLower() == "super" || _notation.ToLower() == "ex")
        {
            var buttonGo = new GameObject($"grimui_{_notation}_button_item");
            buttonGo.transform.SetParent(_buttonContainer.transform);
            text = buttonGo.AddComponent<Text>();
            var bracketSizeFitter = buttonGo.AddComponent<ContentSizeFitter>();
            bracketSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            text.AssignDefaultFont();
            text.fontSize = 24;
            text.font = FontAssetManager.Instance.SuperFont;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = _notation.ToUpper();
            _canvasRenderers.Add(buttonGo.transform.GetComponent<CanvasRenderer>());
            return;
        }

        for (var index = 0; index < _notation.Length; index++)
        {
            var c = _notation[index];
            Transform transform;
            if (c == 'A' && _notation[index + 1] is '1' or '2')
            {
                transform = GenerateUIItemFromString($"A{_notation[++index]}");
                _canvasRenderers.Add(transform.GetComponent<CanvasRenderer>());
            }
            else
            {
               transform = GenerateUIItemFromString(c.ToString());
            }

            if (transform != null)
            {
                _canvasRenderers.Add(transform.GetComponent<CanvasRenderer>());
            }
        }
    }

    private Transform GenerateUIItemFromString(string notation)
    {
        if (notation == "5") return null;

        var buttonGo = new GameObject($"grimui_{notation}_button_item");
        buttonGo.transform.SetParent(_buttonContainer.transform);
        var sprite = SpriteMap.Instance.GetMapping(notation);
        if (sprite)
        {
            var img = buttonGo.AddComponent<Image>();
            var rect = buttonGo.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new() { x = 32, y = 32 };
                rect.transform.localScale = new(1, 1, 1);
                rect.localScale = new(1, 1, 1);
                rect.position = new(1, 1, 1);
                img.sprite = sprite;
                img.overrideSprite = sprite;
                img.preserveAspect = true;
            }
        }
        else
        {
            text = buttonGo.AddComponent<Text>();
            var bracketSizeFitter = buttonGo.AddComponent<ContentSizeFitter>();
            bracketSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            text.AssignDefaultFont();
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = $"{notation}";
        }

        return buttonGo.transform;
    }

    public void SetText(string newText)
    {
        if (text)
        {
            text.text = newText;
        }
    }
}
