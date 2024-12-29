namespace UnityEngine.UI
{
    using TMPro;
    using UnityEngine;

    public static class GraphicsExtensions
    {
        public static void SetColorR(this Image source, byte value)
        {
            Color32 color = source.color;
            color.r = value;
            source.color = color;
        }

        public static void SetColorG(this Image source, byte value)
        {
            Color32 color = source.color;
            color.g = value;
            source.color = color;
        }

        public static void SetColorB(this Image source, byte value)
        {
            Color32 color = source.color;
            color.b = value;
            source.color = color;
        }

        public static void SetColorA(this Image source, byte value)
        {
            Color32 color = source.color;
            color.a = value;
            source.color = color;
        }

        public static void SetColorR(this Image source, float value)
        {
            var color = source.color;
            color.r = value;
            source.color = color;
        }

        public static void SetColorG(this Image source, float value)
        {
            var color = source.color;
            color.g = value;
            source.color = color;
        }

        public static void SetColorB(this Image source, float value)
        {
            var color = source.color;
            color.b = value;
            source.color = color;
        }

        public static void SetColorA(this Image source, float value)
        {
            var color = source.color;
            color.a = value;
            source.color = color;
        }

        public static void SetColorR(this TextMeshProUGUI source, byte value)
        {
            Color32 color = source.color;
            color.r = value;
            source.color = color;
        }

        public static void SetColorG(this TextMeshProUGUI source, byte value)
        {
            Color32 color = source.color;
            color.g = value;
            source.color = color;
        }

        public static void SetColorB(this TextMeshProUGUI source, byte value)
        {
            Color32 color = source.color;
            color.b = value;
            source.color = color;
        }

        public static void SetColorA(this TextMeshProUGUI source, byte value)
        {
            Color32 color = source.color;
            color.a = value;
            source.color = color;
        }

        public static void SetColorR(this TextMeshProUGUI source, float value)
        {
            var color = source.color;
            color.r = value;
            source.color = color;
        }

        public static void SetColorG(this TextMeshProUGUI source, float value)
        {
            var color = source.color;
            color.g = value;
            source.color = color;
        }

        public static void SetColorB(this TextMeshProUGUI source, float value)
        {
            var color = source.color;
            color.b = value;
            source.color = color;
        }

        public static void SetColorA(this TextMeshProUGUI source, float value)
        {
            var color = source.color;
            color.a = value;
            source.color = color;
        }
    }
}