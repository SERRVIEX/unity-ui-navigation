namespace UnityEngine.UI
{
    using DG.Tweening;

    public class DismissFadeTransition : ScreenTransition
    {
        public DismissFadeTransition(ScreenController target) : base(target) { }

        // Methods

        public override void OnBegin()
        {
            Target.CanvasGroup.DOFade(0, Duration);
        }
    }
}
