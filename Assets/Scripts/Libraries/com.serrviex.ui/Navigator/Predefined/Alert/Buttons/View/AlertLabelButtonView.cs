namespace UnityEngine.UI.Predefined.Alert
{
    using UnityEngine;

    using TMPro;

    public class AlertLabelButtonView : AlertButtonViewBase
    {
        [SerializeField] protected TextMeshProUGUI Label;

        public override void SetData(AlertButtonBase data)
        {
            base.SetData(data);

            AlertLabelButtonData textActionData = data as AlertLabelButtonData;
            Label.text = textActionData.Label;
            Label.color = textActionData.LabelColor;
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (Label == null)
                Label = transform.Find("Label").GetComponent<TextMeshProUGUI>();
        }
    }
}