namespace UnityEngine.UI
{
    using UnityEngine;

    using DG.Tweening;

    public class DismissToastTransition : ScreenTransition
    {
        private RectTransform _background;

        // Constructors

        public DismissToastTransition(ScreenController target, RectTransform background) : base(target)
        {
            _background = background;
        }

        // Methods

        public override void OnBegin()
        {
            Target.CanvasGroup.DOFade(0, Duration);
            _background.DOAnchorPosY(0, Duration);
            _background.DOPunchRotation(Vector3.forward * 5, Duration, 30, 1);
        }
    }
}