namespace UnityEngine.Predefined.Alert
{
    using UnityEngine;

    public struct IconData
    {
        public Sprite Icon { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public IconData(Sprite icon)
        {
            Icon = icon;
            Width = 35;
            Height = 35;
        }

        public IconData(Sprite icon, int width, int height)
        {
            Icon = icon;
            Width = width;
            Height = height;
        }
    }
}