namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    [Serializable]
    public struct NavigatorLayers
    {
        [SerializeField, Immutable] public Canvas Root;
        [SerializeField, Immutable] public Canvas OverRoot;
        [SerializeField, Immutable] public Canvas Main;
        [SerializeField, Immutable] public Canvas OverMain;
        [SerializeField, Immutable] public Canvas Context;
        [SerializeField, Immutable] public Canvas OverContext;
        [SerializeField, Immutable] public Canvas Alert;
        [SerializeField, Immutable] public Canvas OverAlert;
        [SerializeField, Immutable] public Canvas Top;

        private Dictionary<Canvas, Camera> _canvasCameraMapping;
        private Dictionary<Camera, List<Canvas>> _cameraToCanvasMap;

        public void Initialize()
        {
            _canvasCameraMapping = new Dictionary<Canvas, Camera>();
            _cameraToCanvasMap = new Dictionary<Camera, List<Canvas>>();

            Mapping(Root);
            Mapping(OverRoot);
            Mapping(Main);
            Mapping(OverMain);
            Mapping(Context);
            Mapping(OverContext);
            Mapping(Alert);
            Mapping(OverAlert);
            Mapping(Top);

            foreach (var item in _canvasCameraMapping)
            {
                var camera = item.Value;
                if (!_cameraToCanvasMap.ContainsKey(camera))
                    _cameraToCanvasMap.Add(camera, new List<Canvas>());
                _cameraToCanvasMap[camera].Add(item.Key);
            }
        }

        public Canvas GetCanvas(NavigatorLayerType layer)
        {
            return layer switch
            {
                NavigatorLayerType.Root => Root,
                NavigatorLayerType.OverRoot => OverRoot,
                NavigatorLayerType.Main => Main,
                NavigatorLayerType.OverMain => OverMain,
                NavigatorLayerType.Context => Context,
                NavigatorLayerType.OverContext => OverContext,
                NavigatorLayerType.Alert => Alert,
                NavigatorLayerType.OverAlert => OverAlert,
                NavigatorLayerType.Top => Top,
                _ => Root,
            };
        }

        public void UpdateProperties(HideFlags hideFlags)
        {
            UpdateLayer(Root, 0, hideFlags);
            UpdateLayer(OverRoot, 20, hideFlags);
            UpdateLayer(Main, 40, hideFlags);
            UpdateLayer(OverMain, 60, hideFlags);
            UpdateLayer(Context, 80, hideFlags);
            UpdateLayer(OverContext, 100, hideFlags);
            UpdateLayer(Alert, 120, hideFlags);
            UpdateLayer(OverAlert, 140, hideFlags);
            UpdateLayer(Top, 160, hideFlags);
        }

        private void UpdateLayer(Canvas canvas, int sortingOrder, HideFlags hideFlags)
        {
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = sortingOrder;
                canvas.gameObject.hideFlags = hideFlags;
            }
        }

        private void Mapping(Canvas canvas)
        {
            Canvas temp = canvas;
            while(temp != null)
            {
                if(temp.worldCamera != null)
                {
                    _canvasCameraMapping.Add(canvas, canvas.worldCamera);
                    return;
                }

                if (temp == temp.rootCanvas)
                    break;

                temp = temp.rootCanvas;
            }
        }

        public int GetActiveViewControllersRenderedByCamera(Camera target)
        {
            int count = 0;
            foreach (var canvas in _cameraToCanvasMap[target])
                count += canvas.transform.childCount;
            return count;
        }
    }
}