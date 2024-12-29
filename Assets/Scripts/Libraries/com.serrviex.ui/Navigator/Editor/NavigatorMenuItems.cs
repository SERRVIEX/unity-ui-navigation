namespace UnityEngine.UI
{
    using UnityEditor;

    using UnityEngine;

    public static class NavigatorMenuItems
    {
        [MenuItem("GameObject/UI/Base Navigator")]
        public static void Create()
        {
            var obj = new GameObject();
            obj.AddComponent<BaseNavigator>();
        }
    }
}