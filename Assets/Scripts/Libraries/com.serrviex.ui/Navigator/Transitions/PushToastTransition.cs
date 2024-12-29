namespace UnityEngine.UI
{
    using DG.Tweening;

    public class PushToastTransition : ScreenTransition
    {
        private RectTransform _background;

        // Constructors

        public PushToastTransition(ScreenController target, RectTransform background) : base(target)
        {
            _background = background;
        }

        // Methods

        public override void OnBegin()
        {
            Target.CanvasGroup.alpha = 0;
            Target.CanvasGroup.DOFade(1, Duration);
            Vector2 anchoredPosition = _background.anchoredPosition;
            anchoredPosition.y = -200;
            _background.anchoredPosition = anchoredPosition;
            _background.DOAnchorPosY(-175, Duration);
            _background.DOPunchScale(Vector3.one * .1f, Duration);
            _background.DOPunchRotation(Vector3.forward * 5, Duration, 30, 1);
        }
    }
}