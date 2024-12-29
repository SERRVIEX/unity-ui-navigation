#undef OSA
namespace UnityEngine.UI
{
#if OSA
    using UnityEngine;

    using Com.TheFallenGames.OSA.Core;

    public sealed class SafeAreaOSA : SafeAreaBase
    {
        private IOSA _osa;
        private RectOffset _padding;

        // Methods

        protected override void Awake()
        {
            base.Awake();

            _osa = GetComponent<IOSA>();
            _padding = _osa.BaseParameters.ContentPadding;
        }

        public override void Apply()
        {
            RectOffset offset = SafeArea.GetOffset();
            Rect safeArea = Screen.safeArea;

            float rectX = safeArea.x * Screen.width;
            float rectY = safeArea.y * Screen.height;
            float rectWidth = safeArea.width;
            float rectHeight = safeArea.height;

            safeArea.x = rectX / Screen.width + offset.left;
            safeArea.y = rectY / Screen.height + offset.bottom;
            safeArea.width = rectWidth - offset.left - offset.right;
            safeArea.height = rectHeight - offset.bottom - offset.top;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = new Vector2(Screen.width, Screen.height) - (safeArea.position + safeArea.size);

            RectOffset padding = new RectOffset(_padding.left, _padding.right, _padding.top, _padding.bottom);

            if (Affect.HasFlag(Direction.Left)) padding.left += (int)anchorMin.x;
            if (Affect.HasFlag(Direction.Right)) padding.right += (int)anchorMax.x;
            if (Affect.HasFlag(Direction.Top)) padding.top += (int)anchorMax.y;
            if (Affect.HasFlag(Direction.Bottom)) padding.bottom += (int)anchorMin.y;

            _osa.BaseParameters.ContentPadding = padding;
        }
    }
#endif
}