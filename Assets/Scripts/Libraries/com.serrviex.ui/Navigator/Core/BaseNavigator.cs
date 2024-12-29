namespace UnityEngine.UI
{
    using UnityEngine;

    public class BaseNavigator : MonoBehaviour
    {
        public static BaseNavigator Instance { get; private set; }

        public Navigator Navigator => _navigator;
        [field: SerializeField] private Navigator _navigator;

        public bool MakeAsSingletone => _makeAsSingletone;
        [SerializeField] private bool _makeAsSingletone = true;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }

            if (_makeAsSingletone)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnValidate()
        {
            if (name != "BaseNavigator")
                name = "BaseNavigator";

            if (Navigator == null)
            {
                var navigator = transform.GetComponent<Navigator>();
                if (navigator != null)
                {
                    _navigator = navigator;
                    return;
                }

                _navigator = gameObject.AddComponent<Navigator>();
            }
        }

        private void Reset()
        {
            OnValidate();
        }
    }
}