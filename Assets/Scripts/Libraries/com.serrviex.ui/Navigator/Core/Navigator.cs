namespace UnityEngine.UI
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem.UI;

    public sealed class Navigator : MonoBehaviour
    {
        /// <summary>
        /// Maximum number of screen controllers (reusable) that can be hold in memory, otherwise
        /// only its state will be saved and the view itself will be reused.
        /// </summary>
        private const int MaxForegroundScreenControllers = 4;

        public static Navigator Instance { get; private set; }

        public static List<Camera> Cameras => Instance._cameras;
        [SerializeField] private List<Camera> _cameras = new List<Camera>();

        public static EventSystem EventSystem => Instance._eventSystem;
        [SerializeField] private EventSystem _eventSystem;

        [SerializeField] private List<NavigatorCanvas> _canvases = new List<NavigatorCanvas>();
        [SerializeField] private NavigatorLayers _layers;

        [SerializeField] private ScreenController _initialScreenController;

        private List<ScreenHolder> _holders = new List<ScreenHolder>();
        private List<ScreenController> _unconstraintedScreens = new List<ScreenController>();

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;

            if (_cameras.Count == 0)
                Debug.LogError("No cameras are assigned. Please ensure at least one camera is properly assigned.");

            if (_eventSystem == null)
                Debug.LogError("Event System is not assigned. Please ensure the event system is properly assigned.");

            _layers.Initialize();

            if (_initialScreenController != null)
                Present(_initialScreenController);

            UpdateCamerasStates();
        }

        /// <summary>
        /// Retrieves the canvas object associated with the specified layer.
        /// </summary>
        /// <param name="layer">The layer to retrieve the canvas for.</param>
        /// <returns>The canvas object corresponding to the specified layer.</returns>
        public static Canvas GetLayer(NavigatorLayerType layer)
        {
            return layer switch
            {
                NavigatorLayerType.Root => Instance._layers.Root,
                NavigatorLayerType.OverRoot => Instance._layers.OverRoot,
                NavigatorLayerType.Main => Instance._layers.Main,
                NavigatorLayerType.OverMain => Instance._layers.OverMain,
                NavigatorLayerType.Context => Instance._layers.Context,
                NavigatorLayerType.OverContext => Instance._layers.OverContext,
                NavigatorLayerType.Alert => Instance._layers.Alert,
                NavigatorLayerType.OverAlert => Instance._layers.OverAlert,
                NavigatorLayerType.Top => Instance._layers.Top,
                _ => Instance._layers.Root,
            };
        }

        /// <summary>
        /// Presents a screen instantiated from a prefab in the active section's canvas.
        /// </summary>
        /// <typeparam name="T">Type of the screen to present.</typeparam>
        /// <param name="prefab">The screen prefab to present.</param>
        /// <param name="blockRaycast">Determines whether to block raycasts to objects underneath the screen.</param>
        /// <returns>The presented screen.</returns>
        public static T Present<T>(T prefab, bool blockRaycast = true) where T : ScreenController
        {
            Assert.IsFalse(prefab == null);
            T screen;
            if (!TryGetReusableScreen(out screen))
                screen = Instantiate(prefab, Instance._layers.GetCanvas(prefab.Layer).transform);
            screen.name = screen.GetType().Name;
            PresentInternal(screen, blockRaycast);
            return screen;
        }

        /// <summary>
        /// Presents a screen of the specified type in the specified section's canvas.
        /// </summary>
        /// <typeparam name="T">Type of the screen to present.</typeparam>
        /// <param name="blockRaycast">Determines whether to block raycasts to objects underneath the screen.</param>
        /// <returns>The presented screen.</returns>
        public static T Present<T>(bool blockRaycast = true) where T : ScreenController
        {
            T screen;
            if (!TryGetReusableScreen(out screen))
            {
                var prefab = NavigatorContainer.Settings.GetViewController<T>();
                Assert.IsFalse(prefab == null);
                screen = Instantiate(prefab, Instance._layers.GetCanvas(prefab.Layer).transform);
            }

            screen.name = screen.GetType().Name;
            PresentInternal(screen, blockRaycast);

            return screen;
        }

        /// <summary>
        /// Presents a screen of the specified type within another parent transform.
        /// Presenting in this way does not support reusable.
        /// </summary>
        /// <typeparam name="T">Type of the screen to present.</typeparam>
        /// <param name="parent">The transform to parent the instantiated screen to.</param>
        /// <param name="blockRaycast">Determines whether to block raycasts to objects underneath the screen.</param>
        /// <returns>The presented screen.</returns>
        public static T Present<T>(Transform parent, bool blockRaycast = true) where T : ScreenController
        {
            var prefab = NavigatorContainer.Settings.GetViewController<T>();
            Assert.IsFalse(prefab == null);
            var screen = Instantiate(prefab, parent);
            screen.name = screen.GetType().Name;
            screen.Unconstrainted = true;
            screen.Reusable = false;
            PresentInternal(screen, blockRaycast);
            return screen;
        }

        private static void PresentInternal(ScreenController viewController, bool blockRaycast)
        {
            Instance.StartCoroutine(Instance.PresentCoroutine(viewController, blockRaycast));
        }

        /// <summary>
        /// Coroutine to handle the presentation of a screen.
        /// </summary>
        /// <param name="screen">The screen being presented.</param>
        /// <param name="blockRaycast">Determines whether to block raycasts during the presentation.</param>
        private IEnumerator PresentCoroutine(ScreenController screen, bool blockRaycast)
        {
            UpdateCamerasStates();

            if (!screen.Independent)
            {
                if (!screen.Unconstrainted)
                {
                    var holder = new ScreenHolder(screen);
                    _holders.Add(holder);
                }
                else
                {
                    // An unconstrainted screen cannot be reused.
                    screen.Reusable = false;
                    _unconstraintedScreens.Add(screen);
                }
            }

            screen.InitializeController();

            var transition = screen.GetPresentTransition();

            StartCoroutine(DelayCall(transition.Duration, () => UpdateBackgroundState()));

            if (blockRaycast)
                SetEventSystemInactive(transition.Duration);

            screen.OnWillAppear();

            screen.Canvas.enabled = false;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            screen.Canvas.enabled = true;

            yield return StartCoroutine(Animate(transition));

            screen.OnDidAppear();
        }

        /// <summary>
        /// Dismisses the specified screen with the default blockRaycast setting (true).
        /// </summary>
        /// <param name="viewController">The screen to dismiss.</param>
        public static void Dismiss(ScreenController viewController)
        {
            Assert.IsFalse(viewController == null);

            DismissInternal(viewController, true);
        }

        /// <summary>
        /// Dismisses the specified screen with the specified blockRaycast setting.
        /// </summary>
        /// <param name="viewController">The screen to dismiss.</param>
        /// <param name="blockRaycast">Determines whether to block raycasts during the dismissal.</param>
        public static void Dismiss(ScreenController viewController, bool blockRaycast = true)
        {
            Assert.IsFalse(viewController == null);

            DismissInternal(viewController, blockRaycast);
        }

        private static void DismissInternal(ScreenController viewController, bool blockRaycast)
        {
            Instance.StartCoroutine(Instance.DismissCoroutine(viewController, blockRaycast));
        }

        /// <summary>
        /// Coroutine to handle the dismissal of a screen.
        /// </summary>
        /// <param name="viewController">The screen to dismiss.</param>
        /// <param name="blockRaycast">Determines whether to block raycasts during the dismissal.</param>
        private IEnumerator DismissCoroutine(ScreenController viewController, bool blockRaycast)
        {
            var transition = viewController.GetDismissTransition();

            if (blockRaycast)
                SetEventSystemInactive(transition.Duration);

            viewController.OnWillDisappear();

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);

            if (viewController.Previous != null)
            {
                var holder = GetHolder(viewController.Previous);
                if (holder.Unloaded)
                {
                    holder.Unloaded = false;
                    holder.Controller.OnEnteringForeground(holder.States);
                    holder.States.Clear();
                }
            }

            yield return StartCoroutine(Animate(transition));

            viewController.OnDidDisappear();

            if (!viewController.Independent)
            {
                if (viewController.Reusable)
                {
                    var holder = GetScreenHolder(viewController);
                    if (!TryReleaseReusableScreen(viewController))
                        Destroy(viewController.gameObject);

                    holder.Controller = null;
                    holder.Dispose();
                    _holders.Remove(holder);
                }
                else
                {
                    if (viewController.Unconstrainted)
                    {
                        _unconstraintedScreens.Remove(viewController);
                        Destroy(viewController.gameObject);
                    }
                    else
                    {
                        var holder = GetScreenHolder(viewController);
                        holder.Dispose();
                        _holders.Remove(holder);
                    }
                }
            }
            else
            {
                Destroy(viewController.gameObject);
            }

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);

            UpdateCamerasStates();
        }

        private IEnumerator Animate(ScreenTransition transition, float time = 0)
        {
            transition.OnBegin();

            while (time <= transition.Duration)
            {
                transition.Animate(time / transition.Duration);
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            transition.Animate(1);

            transition.OnEnd();
        }

        private IEnumerator DelayCall(float time, Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action();
        }

        /// <summary>
        /// Updates the background state of the screen holder at the specified index from the end of the list.
        /// If the holder's state is already unloaded, the method exits early. Otherwise, it marks the holder as unloaded
        /// and triggers the OnEnteringBackground method on the controller.
        /// </summary>
        private static void UpdateBackgroundState()
        {
            var list = Instance._holders;

            // Check if there are enough holders in the list to access the specified index from the end.
            if (list.Count >= MaxForegroundScreenControllers)
            {
                // Access the holder at the index specified from the end of the list.
                var holder = list[^MaxForegroundScreenControllers];

                // If the holder is already unloaded, exit the method.
                if (holder.Unloaded)
                    return;

                // Mark the holder as unloaded and call OnEnteringBackground on the controller.
                holder.Unloaded = true;
                holder.Controller.OnEnteringBackground(holder.States);
            }
        }

        /// <summary>
        /// Searches for a screen holder that contains the specified screen.
        /// If found, returns the holder; otherwise, returns null.
        /// </summary>
        /// <param name="viewController">The screen to find.</param>
        /// <returns>The screen holder containing the specified screen, or null if not found.</returns>
        private static ScreenHolder GetHolder(ScreenController viewController)
        {
            var list = Instance._holders;

            // Iterate through the list to find the holder with the matching controller.
            for (int i = 0; i < list.Count; i++)
                if (list[i].Controller == viewController)
                    return list[i];

            // Return null if the holder is not found
            return null;
        }

        /// <summary>
        /// Retrieves the screen at the specified index in the holders list.
        /// </summary>
        /// <param name="index">The index of the screen in the list.</param>
        /// <returns>The screen at the specified index.</returns>
        public static ScreenController GetScreenByIndex(int index)
        {
            return Instance._holders[index].Controller;
        }

        /// <summary>
        /// Retrieves the screen by type from the holders list.
        /// </summary>
        public static ScreenController GetScreenByType<T>() where T : ScreenController
        {
            foreach (var item in Instance._holders)
            {
                if (item.Controller == null)
                    continue;

                if (item.Controller is T)
                    return item.Controller;
            }
            return null;
        }

        /// <summary>
        /// Retrieves the last screen in the holders list.
        /// </summary>
        /// <returns>The last screen in the list.</returns>
        public static ScreenController GetLastScreen()
        {
            var list = Instance._holders;

            // Return the screen from the last holder in the list.
            return list[list.Count - 1].Controller;
        }

        /// <summary>
        /// Retrieves the previous screen relative to the specified screen.
        /// If the specified screen is the first one or not found, returns null.
        /// </summary>
        /// <param name="viewController">The target screen.</param>
        /// <returns>The previous screen if found; otherwise, null.</returns>
        public static ScreenController GetPreviousScreen(ScreenController viewController)
        {
            var controllers = Instance._holders;

            // If there's only one controller in the list, return null.
            if (controllers.Count == 1)
                return null;

            // Iterate from the end of the list to find the previous screen.
            for (int i = controllers.Count - 1; i >= 0; i--)
            {
                // If it's the first element, return null.
                if (i == 0) return null;

                // If the current controller matches the target, return the previous one.
                if (controllers[i].Controller == viewController)
                    return controllers[i - 1].Controller;
            }

            // Return null if the target screen is not found.
            return null;
        }

        /// <summary>
        /// Searches for a screen holder that contains the specified screen.
        /// If found, returns the holder; otherwise, returns null.
        /// </summary>
        /// <param name="viewController">The screen to find.</param>
        /// <returns>The screen holder containing the specified screen, or null if not found.</returns>
        private static ScreenHolder GetScreenHolder(ScreenController viewController)
        {
            // Iterate through the list of holders to find one containing the specified screen.
            foreach (var item in Instance._holders)
            {
                if (item.Controller == viewController)
                    return item; // Return the holder if found.
            }

            // Return null if the holder is not found.
            return null;
        }

        /// <summary>
        /// Attempts to retrieve a reusable screen of type T.
        /// The screen is taken from the holders list if it is unloaded, is of the specified type,
        /// and is marked as reusable. The screen's transform is set to be the last sibling in the hierarchy.
        /// </summary>
        /// <param name="instance">Output parameter that will contain the reusable screen if successful.</param>
        /// <typeparam name="T">The type of screen to retrieve.</typeparam>
        /// <returns>True if a reusable screen was found and assigned; otherwise, false.</returns>
        private static bool TryGetReusableScreen<T>(out T instance) where T : ScreenController
        {
            var list = Instance._holders;

            // Iterate backward through the list of holders.
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var holder = list[i];

                // Check if the holder is unloaded, contains a screen of type T, and the controller is reusable.
                if (holder.Unloaded && holder.Controller is T && holder.Controller.Reusable)
                {
                    // Take the screen from the holder and cast it to type T.
                    instance = holder.Take() as T;

                    // Move the screen to be the last sibling in the hierarchy.
                    instance.transform.SetAsLastSibling();
                    return true; // Successfully retrieved a reusable screen.
                }
            }

            // If no reusable screen was found, assign null to the output parameter and return false.
            instance = null;
            return false;
        }

        /// <summary>
        /// Attempts to release a screen back into the holders list if it is unloaded,
        /// and its type matches the specified screen's type.
        /// The screen's transform is set to be the first sibling in the hierarchy.
        /// An animation coroutine is started for presenting the screen.
        /// </summary>
        /// <param name="viewController">The screen to release.</param>
        /// <returns>True if the screen was successfully released; otherwise, false.</returns>
        private static bool TryReleaseReusableScreen(ScreenController viewController)
        {
            var list = Instance._holders;

            // Iterate backward through the list of holders.
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var holder = list[i];

                // Check if the holder is unloaded, has no current controller, and matches the screen's type.
                if (holder.Unloaded && holder.Controller == null && holder.Type == viewController.GetType())
                {
                    // Return the screen to the holder and set its transform to be the first sibling in the hierarchy.
                    holder.Return(viewController);
                    viewController.transform.SetAsFirstSibling();

                    // Start a coroutine to animate the presentation of the screen.
                    Instance.StartCoroutine(Instance.Animate(viewController.GetPresentTransition()));
                    return true; // Successfully released the screen.
                }
            }

            // Return false if no matching holder was found to release the screen.
            return false;
        }

        /// <summary>
        /// Forces dismissal of the specified screen without any transition animations.
        /// </summary>
        /// <param name="viewController">The screen to dismiss forcibly.</param>
        public static void ForceDismiss(ScreenController viewController)
        {
            var holder = GetScreenHolder(viewController);
            holder.Dispose();
            Instance._holders.Remove(holder);

            viewController.OnWillDisappear();
            viewController.OnDidDisappear();

            Destroy(viewController.gameObject);

            UpdateCamerasStates();
        }

        /// <summary>
        /// Forces dismissal of all view controllers without any transition animations.
        /// </summary>
        public static void ForceDismissAll()
        {
            foreach (var holder in Instance._holders)
            {
                holder.Controller.OnWillDisappear();
                holder.Controller.OnDidDisappear();

                holder.Dispose();
            }

            Instance._holders.Clear();

            UpdateCamerasStates();
        }

        /// <summary>
        /// Forces dismissal of all view controllers without any transition animations except target.
        /// </summary>
        public static void ForceDismissAllExcept(ScreenController target)
        {
            var holders = Instance._holders;
            for (int i = holders.Count - 1; i >= 0; i--)
            {
                if (holders[i].Controller == target)
                    continue;
                holders[i].Controller.OnWillDisappear();
                holders[i].Controller.OnDidDisappear();
                holders[i].Dispose();
                holders.RemoveAt(i);
            }

            UpdateCamerasStates();
        }

        /// <summary>
        /// Updates the state of all cameras by iterating through each camera in the list
        /// and calling <see cref="UpdateCameraState"/> on them.
        /// </summary>
        private static void UpdateCamerasStates()
        {
            // Iterate through each camera in the Cameras collection.
            foreach (var item in Cameras)
            {
                // Update the state of the current camera.
                UpdateCameraState(item);
            }
        }

        /// <summary>
        /// Updates the state of a single camera based on the number of active view controllers
        /// rendered by that camera. The camera's GameObject is activated if there are active
        /// view controllers rendered by it; otherwise, it is deactivated.
        /// </summary>
        /// <param name="camera">The camera to update.</param>
        private static void UpdateCameraState(Camera camera)
        {
            // Return immediately if the provided camera is null.
            if (camera == null)
                return;

            // Get the number of active view controllers rendered by the provided camera.
            int activeObjectsRendered = Instance._layers.GetActiveViewControllersRenderedByCamera(camera);

            // Set the camera's GameObject active status based on whether there are active objects rendered.
            camera.gameObject.SetActive(activeObjectsRendered > 0);
        }


        /// <summary>
        /// Temporarily disables the EventSystem to prevent interactions with the interface during animations.
        /// </summary>
        /// <param name="duration">The duration for which the EventSystem will be disabled.</param>
        public static void SetEventSystemInactive(float duration)
        {
            Assert.IsFalse(Instance._eventSystem == null);

            Instance.StartCoroutine(Instance.SetEventSystemInactiveImpl(duration));
        }

        private IEnumerator SetEventSystemInactiveImpl(float duration)
        {
            _eventSystem.enabled = false;
            yield return new WaitForSecondsRealtime(duration);
            _eventSystem.enabled = true;
        }

        /// <summary>
        /// Finds the nearest screen plane distance in the specified section.
        /// </summary>
        /// <param name="section">The section in which the search should be performed.</param>
        /// <returns>The distance of the nearest screen plane.</returns>
        public static float GetNearestCanvasPlaneDistance()
        {
            if (Instance._holders.Count == 0)
                return 100;

            var distance = 100f;
            var holders = Instance._holders;
            for (int i = 0; i < holders.Count; i++)
            {
                var holder = holders[i];
                if (holder.Controller != null && holder.Controller.Canvas.planeDistance < distance)
                    distance = holder.Controller.Canvas.planeDistance;
            }

            return distance - 0.5f;
        }

        /// <summary>
        /// Finds and retrieves all view controllers of a specified type from the current instance's holders.
        /// </summary>
        /// <typeparam name="T">The type of screen to find. Must inherit from <see cref="ScreenController"/>.</typeparam>
        /// <returns>A list of view controllers of type <typeparamref name="T"/> found in the holders.</returns>
        public static List<T> FindScreenOfType<T>() where T : ScreenController
        {
            // Initialize a list to store the results.
            var result = new List<T>();

            // Iterate through each holder in the instance's _holders list.
            for (int i = 0; i < Instance._holders.Count; i++)
            {
                var holder = Instance._holders[i];

                // Check if the holder's controller is not null and is of the specified type T.
                if (holder.Controller != null && holder.Controller.GetType() == typeof(T))
                {
                    // Add the controller of type T to the result list.
                    result.Add(holder.Controller.GetComponent<T>());
                }
            }

            // Return the list of view controllers of type T
            return result;
        }

        private void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (_eventSystem == null)
            {
                var eventSystems = FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                if (eventSystems.Length > 0)
                    _eventSystem = eventSystems[0];
                else
                {
                    _eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
                    _eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                }
            }

            if (_eventSystem.transform.parent != transform)
            {
                _eventSystem.transform.SetParent(transform, false);
                _eventSystem.transform.SetSiblingIndex(0);
            }

            // Try to get canvases in children.
            if (_canvases.Count == 0)
                _canvases = gameObject.GetComponentsInChildren<NavigatorCanvas>().ToList();

            // Navigator must have at least one canvas.
            if (_canvases.Count == 0)
                _canvases.Add(NavigatorUtils.CreateNavigatorCanvas("RootCanvas", transform));

            // Create layers if need.
            CreateLayer(ref _layers.Root, NavigatorLayerType.Root);
            CreateLayer(ref _layers.OverRoot, NavigatorLayerType.OverRoot);
            CreateLayer(ref _layers.Main, NavigatorLayerType.Main);
            CreateLayer(ref _layers.OverMain, NavigatorLayerType.OverMain);
            CreateLayer(ref _layers.Context, NavigatorLayerType.Context);
            CreateLayer(ref _layers.OverContext, NavigatorLayerType.OverContext);
            CreateLayer(ref _layers.Alert, NavigatorLayerType.Alert);
            CreateLayer(ref _layers.OverAlert, NavigatorLayerType.OverAlert);
            CreateLayer(ref _layers.Top, NavigatorLayerType.Top);

            _layers.UpdateProperties(HideFlags.None);
        }

        private void CreateLayer(ref Canvas canvas, NavigatorLayerType layerType)
        {
            if (canvas != null)
                return;

            canvas = NavigatorUtils.CreateLayer(layerType, _canvases[0].transform);
        }

        public void AddMainCamera()
        {
            if (_cameras.Count != 0)
                return;

            var mainCamera = Camera.main;
            _cameras.Add(mainCamera);
        }

        public void NewCamera()
        {
            var newCamera = new GameObject("UICamera").AddComponent<Camera>();
            newCamera.clearFlags = CameraClearFlags.SolidColor;
            newCamera.backgroundColor = Color.clear;
            newCamera.orthographic = true;
            newCamera.orthographicSize = 5;
            newCamera.transform.SetParent(transform);
            newCamera.transform.SetAsFirstSibling();
            newCamera.transform.localScale = Vector3.one;
            newCamera.gameObject.layer = 5;
            newCamera.cullingMask = 1 << 5;
            _cameras.Add(newCamera);
        }
    }
}