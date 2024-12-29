namespace UnityEngine.UI
{
    using System.Collections;

    using UnityEngine;

    using TMPro;

    public sealed class ActionToast : ScreenController
    {
        private static ActionToast _instance;

        [SerializeField] private TextMeshProUGUI _message;

        public static ActionToast Present(string message)
        {
            _instance = Navigator.Present<ActionToast>(false);
            _instance.Set(message);
            return _instance;
        }

        private void Set(string text)
        {
            _message.text = text;
            StartCoroutine(Countdown());
        }

        private IEnumerator Countdown()
        {
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                transform.localPosition += new Vector3(0, 1f, 1);
                yield return null;
            }

            Dismiss();
        }

        public override ScreenTransition GetPresentTransition() => new PushFadeTransition(this);
        public override ScreenTransition GetDismissTransition() => new DismissFadeTransition(this);
    }
}