using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GrimbaHack.module;

public class ComboItemUIElement
{
    private readonly List<char> _notation;
    private readonly List<CanvasRenderer> _canvasRenderers;
    private readonly GameObject _buttonContainer;
    private Text text;
    private SimpleSpriteAnimation _animation;

    public ComboItemUIElement(GameObject buttonContainer, string notation)
    {
        _notation = notation.ToCharArray().ToList();
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
        foreach (var c in _notation)
        {
            var transform = GenerateUIItemFromChar(c);
            if (transform != null)
            {
                _canvasRenderers.Add(transform.GetComponent<CanvasRenderer>());
            }
        }
    }

    private Transform GenerateUIItemFromChar(char character)
    {
        if (character == '5') return null;

        var buttonGo = new GameObject($"grimui_{character}_button_item");
        buttonGo.transform.SetParent(_buttonContainer.transform);
        var sprite = SpriteMap.Instance.GetMapping(character.ToString());
        if (sprite)
        {
            var img = buttonGo.AddComponent<Image>();
            var rect = buttonGo.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new() { x = character == '+' ? 24 : 32, y = character == '+' ? 24 : 32 };
                rect.transform.localScale = new(1, 1, 1);
                rect.localScale = new(1, 1, 1);
                rect.position = new(1, 1, 1);
                img.sprite = sprite;
                img.overrideSprite = sprite;
            }
        }
        else
        {
            text = buttonGo.AddComponent<Text>();
            var bracketSizeFitter = buttonGo.AddComponent<ContentSizeFitter>();
            bracketSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            text.AssignDefaultFont();
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleLeft;
            text.text = $"{character}";
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
