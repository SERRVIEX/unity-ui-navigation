namespace UnityEngine.UI
{
    using System.Collections.Generic;

    using UnityEngine.Assertions;

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(CanvasGroup))]
    /// <summary>
    /// Represents a base class for all screen controllers in the application.
    /// Provides methods for managing the screen controller's state and lifecycle events.
    /// </summary>
    public abstract class ScreenController : MonoBehaviour
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public string Id { get; private set; }

        public bool Initialized { get; private set; }

        public NavigatorLayerType Layer => _layer;
        [SerializeField] private NavigatorLayerType _layer;

        /// <summary>
        /// Use for specific cases like debug screen controller.
        /// </summary>
        public bool Independent { get => _independent; set => _independent = value; }
        [SerializeField] private bool _independent;

        /// <summary>
        /// Use for pop-up screen controllers that don't interact with other screen controllers.
        /// </summary>
        public bool Unconstrainted { get => _unconstrainted; set => _unconstrainted = value; }
        [SerializeField] private bool _unconstrainted = false;

        /// <summary>
        /// Indicates whether the screen controller can be reused multiple times within the same section.
        /// </summary>
        public bool Reusable { get => _reusable; set => _reusable = value; }
        [SerializeField] private bool _reusable;

        /// <summary>
        /// Sends analytics to the providers like firebase, mixpanel...
        /// </summary>
        public bool SendAnalytics { get => _sendAnalytics; set => _sendAnalytics = value; }
        [SerializeField] private bool _sendAnalytics;

        /// <summary>
        /// Resources that are temporary while screen controller is in foreground.
        /// </summary>
        protected List<Object> TemporaryForegroundResources = new List<Object>();

        public ScreenController Previous => Navigator.GetPreviousScreen(this);

        public RectTransform RectTransform { get; private set; }
        public Canvas Canvas { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }
        public RectTransform Content { get; private set; }

        protected ScreenController()
        {
            Id = System.Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Initializes the screen controller with the specified section.
        /// This method is intended to be called only from the Storyboard class.
        /// </summary>
        /// <param name="section">The section to which this screen controller belongs.</param>
        public void InitializeController()
        {
            if (Initialized)
                return;

            RectTransform = GetComponent<RectTransform>();

            Canvas = GetComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceCamera;

            float planeDistance = Navigator.GetNearestCanvasPlaneDistance();
            Canvas.planeDistance = planeDistance;

            CanvasGroup = GetComponent<CanvasGroup>();

            Content = transform.Find("Content").GetComponent<RectTransform>();

            Initialized = true;
        }

        protected virtual void OnEnable()
        {
#if ANALYTICS
            if (_sendAnalytics)
            {
                var ev = new Alytics.ScreenEvent(this);
                Alytics.Track(Id, ev);
            }
#endif
        }

        protected virtual void OnDisable()
        {
#if ANALYTICS
            if (_sendAnalytics)
            {
                var ev = Alytics.Untrack(Id);
                Alytics.Send(ev);
            }
#endif
        }

        /// <summary>
        /// Called before transition.
        /// </summary>
        public virtual void OnWillAppear()
        {
            var hotkeys = new List<HotkeyData>();
            var inputData = new InputData(Id, hotkeys);
            InputHandler.Register(inputData);

            /* ESCAPE
            var hotkeys = new List<HotkeyData>();
            var inputData = new InputData(Id, hotkeys);

            {
                var dismissHotKey = new HotkeyData(
                    () => Input.GetKeyDown(KeyCode.Escape),
                    Dismiss);

                inputData.Add(dismissHotKey);
            }

            InputHandler.Register(inputData);
            */
        }

        /// <summary>
        /// Called after transition.
        /// </summary>
        public virtual void OnDidAppear() { }

        /// <summary>
        /// Called before transition.
        /// </summary>
        public virtual void OnWillDisappear()
        {
            InputHandler.Remove(Id);
        }

        /// <summary>
        /// Called after transition.
        /// </summary>
        public virtual void OnDidDisappear() { }

        public virtual ScreenTransition GetPresentTransition() => new PushTransition(this);
        public virtual ScreenTransition GetDismissTransition() => new DismissTransition(this);

        /// <summary>
        /// Passive screen controller state.
        /// </summary>
        public virtual void OnEnteringBackground(ScreenStates states)
        {
            foreach (var item in TemporaryForegroundResources)
                Destroy(item);
            TemporaryForegroundResources.Clear();
        }

        /// <summary>
        /// Active screen controller state.
        /// </summary>
        public virtual void OnEnteringForeground(ScreenStates states) { }

        protected T PresentViewController<T>() where T : ScreenController
        {
            return Navigator.Present<T>();
        }

        public virtual void Dismiss() => Navigator.Dismiss(this, true);
        public virtual void Dismiss(bool blockRaycast = true) => Navigator.Dismiss(this, blockRaycast);

        /// <summary>
        /// Dismiss without transitions.
        /// </summary>
        public virtual void ForceDismiss() => Navigator.ForceDismiss(this);

        protected virtual void OnDestroy()
        {
            foreach (var item in TemporaryForegroundResources)
                Destroy(item);
            TemporaryForegroundResources.Clear();
        }

        protected virtual void OnValidate()
        {
            name = GetType().FullName;

            var rectTransform = GetComponent<RectTransform>();
            Assert.IsTrue(rectTransform != null);

            rectTransform.SetPivot(Pivot.MiddleCenter);
            rectTransform.SetAnchor(Anchor.Stretch);
            rectTransform.SetOffset(0, 0, 0, 0);

            if (Content == null)
            {
                var content = transform.Find("Content") as RectTransform;
                if (content != null)
                    Content = content;

                else
                {
                    Content = new GameObject("Content").AddComponent<RectTransform>();
                    Content.SetParent(transform, false);
                    Content.gameObject.layer = 5;
                }

                Content.localPosition = Vector3.zero;
                Content.localRotation = Quaternion.identity;
                Content.localScale = Vector3.one;
                Content.SetAnchor(Anchor.Stretch);
                Content.SetOffset(0, 0, 0, 0);
            }

            Content.name = "Content";
        }
    }
}