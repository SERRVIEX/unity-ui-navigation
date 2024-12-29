namespace UnityEngine.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class AlertButtonViewBase : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }
        [SerializeField] protected Button Button;
        [SerializeField] protected Graphic Background;

        // Methods

        public virtual void SetData(AlertButtonBase data)
        {
            Background.color = data.BackgroundColor;
            Button.onClick.AddListener(() => data.OnClick?.Invoke());
        }

        public void SetWidth(float value) => Background.rectTransform.SetWidth(value);

        public virtual void OnValidate()
        {
            name = GetType().Name;
            RectTransform = GetComponent<RectTransform>();
            Button = GetComponent<Button>();
            Background = GetComponent<Graphic>();
        }
    }
}