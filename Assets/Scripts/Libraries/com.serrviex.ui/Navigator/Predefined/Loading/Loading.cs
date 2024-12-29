namespace UnityEngine.UI
{
    using UnityEngine;

    using TMPro;

    public sealed class Loading : ScreenController
    {
        private static Loading _instance;

        [SerializeField] private TextMeshProUGUI _message;

        public static Loading Present(string text = "")
        {
            if (_instance == null)
                _instance = Navigator.Present<Loading>();

            _instance.UpdateContentImpl(text);

            return _instance;
        }

        public static void UpdateContent(string text)
        {
            if (_instance == null)
                return;

            _instance.UpdateContentImpl(text);
        }

        private void UpdateContentImpl(string text) => _message.text = text;

        public override ScreenTransition GetPresentTransition() => new PushFadeTransition(this);
        public override ScreenTransition GetDismissTransition() => new DismissFadeTransition(this);

        public static void Release()
        {
            if (_instance == null)
                return;

            _instance.Dismiss();

            _instance = null;
        }
    }
}