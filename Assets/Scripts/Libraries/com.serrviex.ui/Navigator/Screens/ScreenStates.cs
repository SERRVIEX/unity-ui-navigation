namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;

    public sealed class ScreenStates : IDisposable
    {
        private Dictionary<string, object> _states;

        // Constructors

        public ScreenStates()
        {
            _states = new Dictionary<string, object>();
        }

        // Methods

        public void Save(string key, object value)
        {
            _states.Add(key, value);
        }

        public T Load<T>(string key)
        {
            return (T)_states[key];
        }

        public void Clear()
        {
            _states.Clear();
        }

        public void Dispose()
        {
            _states = null;
        }
    }
}