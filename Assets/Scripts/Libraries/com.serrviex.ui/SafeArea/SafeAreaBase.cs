namespace UnityEngine.UI
{
    using System;

    using UnityEngine;

    public abstract class SafeAreaBase : MonoBehaviour
    {
        protected Canvas Canvas;
        protected RectTransform RectTransform;

        [Flags] protected enum Direction { Left = 1, Right = 2, Top = 4, Bottom = 8 }
        [SerializeField] protected Direction Affect;

        private Rect _previousSafeAreaValue = Rect.zero;

        // Methods

        protected virtual void Awake()
        {
            Canvas = GetComponentInParent<Canvas>();
            RectTransform = GetComponent<RectTransform>();

            _previousSafeAreaValue = Screen.safeArea;

            SafeArea.Subscribe(this);
        }

        private void OnEnable() => Apply();

        private void LateUpdate()
        {
            if (_previousSafeAreaValue != Screen.safeArea)
            {
                _previousSafeAreaValue = Screen.safeArea;
                Apply();
            }
        }

        private void OnDestroy() => SafeArea.Unsubscribe(this);

        public abstract void Apply();
    }
}