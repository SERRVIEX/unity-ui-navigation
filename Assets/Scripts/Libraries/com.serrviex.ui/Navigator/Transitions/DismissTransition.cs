namespace UnityEngine.UI
{
    using DG.Tweening;

    public class DismissTransition : ScreenTransition
    {
        public DismissTransition(ScreenController target) : base(target) { }

        public override void OnBegin()
        {
            Target.CanvasGroup.DOFade(0, Duration);

            Target.Content.transform.DOPunchScale(Vector3.one * .1f, Duration);
        }
    }
}