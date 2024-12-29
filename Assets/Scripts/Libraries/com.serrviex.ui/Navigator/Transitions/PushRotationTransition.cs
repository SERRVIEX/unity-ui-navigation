using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PushRotationTransition : ScreenTransition
{
    public override float Duration => 1;

    public PushRotationTransition(ScreenController target) : base(target)
    {
    }

    public override void OnBegin()
    {
        Target.CanvasGroup.alpha = 0;
        Target.CanvasGroup.DOFade(1, Duration);

        Target.transform.rotation = Quaternion.Euler(0, 0, 90);
        Target.transform.DORotate(Vector3.zero, Duration);
    }
}
