namespace UnityEngine.UI
{
    using DG.Tweening;

    public class PushAlertTransition : ScreenTransition
    {
        public PushAlertTransition(ScreenController target) : base(target) { }

        // Methods

        public override void OnBegin()
        {
            Target.Content.DOPunchScale(Vector3.one * .1f, Duration);
            Target.CanvasGroup.alpha = 0;
            Target.CanvasGroup.DOFade(1, Duration);
        }
    }
}