namespace UnityEngine
{
    using System.Collections.Generic;

    public class InputData
    {
        public string Id { get; private set; }

        private List<HotkeyData> _hotkeys = new List<HotkeyData>();

        public InputData(string id, List<HotkeyData> hotkeys)
        {
            Id = id;
            _hotkeys = hotkeys;
        }

        public void Add(HotkeyData data)
        {
            _hotkeys.Add(data);
        }

        public void RemoveAt(int index)
        {
            _hotkeys.RemoveAt(index);
        }

        public void Update()
        {
            for (int i = 0; i < _hotkeys.Count; ++i)
            {
                var hotkey = _hotkeys[i];
                if(hotkey.Condition())
                {
                    hotkey.Callback();
                    break;
                }
            }
        }
    }
}