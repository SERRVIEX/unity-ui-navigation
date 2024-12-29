namespace UnityEngine.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    using TMPro;

    public class AlertDefaultButtonView : AlertButtonViewBase
    {
        [SerializeField] protected TextMeshProUGUI Label;
        [SerializeField] protected Image Icon;

        // Methods

        public override void SetData(AlertButtonBase data)
        {
            base.SetData(data);

            AlertDefaultButtonData defaultData = data as AlertDefaultButtonData;

            Label.text = defaultData.Label;
            Label.color = defaultData.LabelColor;

            if (defaultData.Icon != null)
            {
                Icon.transform.parent.gameObject.SetActive(true);
                Icon.sprite = defaultData.Icon.Value.Icon;
                Icon.rectTransform.SetSize(defaultData.Icon.Value.Width, defaultData.Icon.Value.Height);
            }
            else
                Icon.transform.parent.gameObject.SetActive(false);
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (Label == null)
                Label = transform.Find("Label").GetComponent<TextMeshProUGUI>();

            if (Icon == null)
                Icon = transform.Find("IconBox/Icon").GetComponent<Image>();
        }
    }
}