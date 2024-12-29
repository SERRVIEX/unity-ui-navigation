namespace UnityEngine.UI
{
    using UnityEngine;

    public class SafeAreaRectTransform : SafeAreaBase
    {
        private void OnValidate()
        {
            name = "SafeAreaRectTransform";

            Canvas = GetComponentInParent<Canvas>();
            RectTransform = GetComponent<RectTransform>();

            RectTransform.SetAnchor(Anchor.Stretch);
            RectTransform.SetOffset(0, 0, 0, 0);
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
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Canvas.pixelRect.width;
            anchorMin.y /= Canvas.pixelRect.height;
            anchorMax.x /= Canvas.pixelRect.width;
            anchorMax.y /= Canvas.pixelRect.height;

            Vector2 outputAnchorMin = Vector2.zero;
            Vector2 outputAnchorMax = Vector2.one;

            if (Affect.HasFlag(Direction.Left)) outputAnchorMin.x = anchorMin.x;
            if (Affect.HasFlag(Direction.Right)) outputAnchorMax.x = anchorMax.x;
            if (Affect.HasFlag(Direction.Top)) outputAnchorMax.y = anchorMax.y;
            if (Affect.HasFlag(Direction.Bottom)) outputAnchorMin.y = anchorMin.y;

            RectTransform.anchorMin = outputAnchorMin;
            RectTransform.anchorMax = outputAnchorMax;
        }
    }
}