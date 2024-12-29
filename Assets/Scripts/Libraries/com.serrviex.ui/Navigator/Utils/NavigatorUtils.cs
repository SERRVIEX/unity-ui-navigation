namespace UnityEngine.UI
{
    using UnityEngine;

    public static class NavigatorUtils
    {
        public static NavigatorCanvas CreateNavigatorCanvas(string name, Transform parent)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.layer = 5;

            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            rectTransform.SetAnchor(Anchor.Stretch);
            rectTransform.SetPivot(Pivot.MiddleCenter);

            rectTransform.SetOffset(0, 0, 0, 0);

            var navigatorCanvas = obj.AddComponent<NavigatorCanvas>();
            navigatorCanvas.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            navigatorCanvas.CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            return navigatorCanvas;
        }

        public static Canvas CreateLayer(NavigatorLayerType type, Transform parent)
        {
            var obj = new GameObject(type.ToString());
            obj.transform.SetParent(parent);
            obj.layer = 5;

            var rectTransform = obj.AddComponent<RectTransform>();
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;

            rectTransform.SetAnchor(Anchor.Stretch);
            rectTransform.SetPivot(Pivot.MiddleCenter);

            rectTransform.SetOffset(0, 0, 0, 0);

            var canvas = obj.AddComponent<Canvas>();
            canvas.overrideSorting = true;

            obj.AddComponent<CanvasScaler>();
            obj.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        /// <summary>
        /// Adapt canvas scaler to target screen.
        /// </summary>
        public static void AdaptCanvasScaler(CanvasScaler canvasScaler)
        {
            if (DeviceUtils.Device == DeviceUtils.DeviceType.Phone)
            {
                canvasScaler.matchWidthOrHeight = 0;
              
                if (GetScreenOrientation() == ScreenOrientation.Portrait)
                {
                    return;
                    if(Screen.width < 1080)
                        canvasScaler.referenceResolution = new Vector2(1080f * (1080f / Screen.width), canvasScaler.referenceResolution.y);
                    
                    else
                        canvasScaler.referenceResolution = new Vector2(1080 * (Screen.height / 1920f), canvasScaler.referenceResolution.y);
                }
                else
                    canvasScaler.matchWidthOrHeight = 1;
            }
            else if (DeviceUtils.Device == DeviceUtils.DeviceType.Tablet)
            {
                canvasScaler.matchWidthOrHeight = 0;

                if (GetScreenOrientation() == ScreenOrientation.Portrait)
                {
                    return;
                    if (Screen.width < 1080)
                        canvasScaler.referenceResolution = new Vector2(1080f * (1080f / Screen.width), canvasScaler.referenceResolution.y);

                    else
                        canvasScaler.referenceResolution = new Vector2(1080 * (Screen.height / 1920f), canvasScaler.referenceResolution.y);
                }
            }
            else
            {
                canvasScaler.matchWidthOrHeight = 1;

                if (GetScreenOrientation() == ScreenOrientation.Portrait)
                    canvasScaler.referenceResolution = new Vector2(Screen.height, 1920);
                else
                    canvasScaler.referenceResolution = new Vector2(1920, 1080);
            }
        }

        /// <summary>
        /// Return the current screen orientation that also work in the editor without simulator.
        /// </summary>
        private static ScreenOrientation GetScreenOrientation()
        {
#if UNITY_EDITOR
            return Screen.height > Screen.width ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;
#else
            return Screen.orientation;
#endif
        }
    }
}