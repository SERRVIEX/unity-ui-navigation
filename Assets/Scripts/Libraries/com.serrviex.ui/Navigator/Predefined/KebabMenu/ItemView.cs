namespace UnityEngine.Predefined.KebabMenu
{
    using UnityEngine;
    using UnityEngine.UI;
    
    using TMPro;

    public sealed class ItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;

        // Methods

        public void Initialize(KebabItemData data)
        {
            _button.onClick.AddListener(() => data.Callback?.Invoke());
            _label.text = data.Label;
        }

        private void OnValidate()
        {
            if(_button == null)
                _button = GetComponent<Button>();

            if (_label == null)
                _label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        }
    }
}