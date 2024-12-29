namespace UnityEngine.UI
{
    using UnityEngine;

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public sealed class NavigatorCanvas : MonoBehaviour
    {
        [field: SerializeField, Immutable] public Canvas Canvas { get; private set; }
        [field: SerializeField, Immutable] public CanvasScaler CanvasScaler { get; private set; }
        [field: SerializeField, Immutable] public GraphicRaycaster GraphicRaycaster { get; private set; }

        private void Start()
        {
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            NavigatorUtils.AdaptCanvasScaler(canvasScaler);
        }

        private void OnValidate()
        {
            var canvas = Canvas;
            var scaler = CanvasScaler;
            var raycaster = GraphicRaycaster;
            GetComponent(ref canvas);
            GetComponent(ref scaler);
            GetComponent(ref raycaster);
            Canvas = canvas;
            CanvasScaler = scaler;
            GraphicRaycaster = raycaster;
        }

        private void GetComponent<T>(ref T component) where T : Component
        {
            if (component == null)
            {
                if (gameObject.GetComponent<T>() != null)
                    component = gameObject.GetComponent<T>();
                else
                    component = gameObject.AddComponent<T>();
            }
        }
    }
}