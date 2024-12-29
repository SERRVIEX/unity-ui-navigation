namespace UnityEngine
{
    using System.Collections.Generic;

    using UnityEngine.UI;

    public sealed class InputHandler : MonoBehaviour
    {
        private static InputHandler Instance
        {
            get
            {
                if(_instance == null)
                {
                    var gameObject = new GameObject("InputHandler");
                    _instance = gameObject.AddComponent<InputHandler>();
                    DontDestroyOnLoad(gameObject);
                    gameObject.hideFlags = HideFlags.HideInHierarchy;
                }

                return _instance;
            }
        }

        private static InputHandler _instance;

        public List<InputData> _inputs = new List<InputData>();

        public static void Register(InputData input)
        {
            Instance.RegisterImpl(input);
        }

        private void RegisterImpl(InputData input)
        {
            for (int i = 0; i < _inputs.Count; i++)
            {
                InputData item = _inputs[i];
                if (item.Id == input.Id)
                {
                    _inputs[i] = input;
                    Debug.Log($"An input with ID '{item.Id}' was already registered and has now been replaced with the new entry.");
                    return;
                }
            }
            _inputs.Add(input);
        }

        public static void Remove(string id)
        {
            Instance.RemoveImpl(id);
        }

        private void RemoveImpl(string id)
        {
            for (int i = _inputs.Count - 1; i >= 0; i--)
            {
                if(_inputs[i].Id == id)
                {
                    _inputs.RemoveAt(i);
                    if(_inputs.Count == 0)
                        DestroyImmediate(gameObject);
                    return;
                }
            }
        }

        private void Update()
        {
            if (!Navigator.EventSystem.enabled)
                return;

            if (_inputs.Count == 0)
                return;

            _inputs[_inputs.Count - 1].Update();
        }
    }
}