namespace UnityEngine.UI
{
    using DG.Tweening;

    public class PushTransition : ScreenTransition
    {
        public PushTransition(ScreenController target) : base(target) { }

        public override void OnBegin()
        {
            Target.gameObject.SetActive(true);

            Target.CanvasGroup.alpha = 0;
            Target.CanvasGroup.DOFade(1, Duration);

            Target.Content.transform.DOPunchScale(Vector3.one * .1f, Duration);
        }
    }
}