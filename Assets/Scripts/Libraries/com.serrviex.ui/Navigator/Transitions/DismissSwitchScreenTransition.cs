using UnityEngine.UI;

using DG.Tweening;

public class DismissSwitchScreenTransition : ScreenTransition
{
    private ScreenController _previous;

    public DismissSwitchScreenTransition(ScreenController target) : base(target)
    {
        _previous = target.Previous;
    }

    public override void OnBegin()
    {
        _previous.gameObject.SetActive(true);
        _previous.RectTransform.DOAnchorPosX(0, Duration);

        Target.RectTransform.DOAnchorPosX(Target.RectTransform.rect.width, Duration);
    }
}
