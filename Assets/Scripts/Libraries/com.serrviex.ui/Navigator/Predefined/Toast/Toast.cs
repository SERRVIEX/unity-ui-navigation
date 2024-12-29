namespace UnityEngine.UI
{
    using System.Collections;

    using UnityEngine;

    using TMPro;

    public sealed class Toast : ScreenController
    {
        private static Toast _instance;

        private string _id;

        [SerializeField] private RectTransform _background;
        [SerializeField] private TextMeshProUGUI _message;

        public static Toast Present(string id, string message, float duration = 2)
        {
            // Avoid spamming toasts with the same id.
            if (_instance != null)
                if (id == _instance._id)
                    return _instance;

            _instance = Navigator.Present<Toast>(false);
            _instance._id = id;
            _instance.Set(message, duration);

            return _instance;
        }

        private void Set(string text, float duration)
        {
            _message.text = text;
            StartCoroutine(Countdown(duration));
        }

        private IEnumerator Countdown(float duration)
        {
            // _background.DOAnchorPosY(0, 5);
            yield return new WaitForSeconds(duration);
            Dismiss();
        }

        public override ScreenTransition GetPresentTransition() => new PushToastTransition(this, _background);
        public override ScreenTransition GetDismissTransition() => new DismissToastTransition(this, _background);
    }
}