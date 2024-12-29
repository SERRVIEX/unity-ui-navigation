namespace UnityEngine.UI
{
    using DG.Tweening;

    public class DismissAlertTransition : ScreenTransition
    {
        public DismissAlertTransition(ScreenController target) : base(target) { }

        // Methods

        public override void OnBegin()
        {
            Target.Content.DOAnchorPosY(-Target.RectTransform.rect.height, Duration);
            Target.CanvasGroup.DOFade(0, Duration);
        }
    }
}