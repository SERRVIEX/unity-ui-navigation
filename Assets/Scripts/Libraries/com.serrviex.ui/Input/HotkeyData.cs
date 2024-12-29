namespace UnityEngine
{
    using System;

    using UnityEngine.Events;

    public struct HotkeyData
    {
        public Func<bool> Condition { get; private set; }
        public UnityAction Callback { get; private set; }

        public HotkeyData(Func<bool> condition, UnityAction callback)
        {
            Condition = condition;
            Callback = callback;
        }
    }
}