using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PushSwitchScreenTransition : ScreenTransition
{
    private ScreenController _previous;

    public PushSwitchScreenTransition(ScreenController target) : base(target)
    {
        _previous = target.Previous;
    }

    public override void OnBegin()
    {
        Target.gameObject.SetActive(true);
        Target.RectTransform.anchoredPosition = new Vector2(Target.RectTransform.rect.width, 0);
        Target.RectTransform.DOAnchorPosX(0, Duration);

        _previous.RectTransform.DOAnchorPosX(-_previous.RectTransform.rect.width, Duration).onComplete += () =>
        {
            _previous.gameObject.SetActive(false);
        };
    }
}
