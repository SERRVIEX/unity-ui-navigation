namespace UnityEngine.UI
{
    using DG.Tweening;

    public class PushFadeTransition : ScreenTransition
    {
        public PushFadeTransition(ScreenController target) : base(target) { }

        // Methods

        public override void OnBegin()
        {
            Target.CanvasGroup.alpha = 0;
            Target.CanvasGroup.DOFade(1, Duration);
        }
    }
}
