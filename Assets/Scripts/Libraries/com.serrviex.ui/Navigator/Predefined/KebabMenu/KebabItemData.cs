namespace UnityEngine
{
    using UnityEngine.Events;

    public struct KebabItemData
    {
        public string Label;
        public UnityEvent Callback;

        // Constructors

        public KebabItemData(string label, UnityAction callback)
        {
            Label = label;
            Callback = new UnityEvent();
            Callback.AddListener(callback);
        }
    }
}