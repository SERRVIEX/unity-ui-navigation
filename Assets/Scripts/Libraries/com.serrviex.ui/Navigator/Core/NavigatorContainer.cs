namespace UnityEngine.UI
{
    using System.IO;

    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public static class NavigatorContainer
    {
        public static NavigatorContainerSettings Settings
        {
            get
            {
                if (_settings == null)
                    _settings = Resources.Load<NavigatorContainerSettings>("NavigatorContainerSettings");
                return _settings;
            }
        }

        private static NavigatorContainerSettings _settings;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            var directoryPath = $"{Application.dataPath}/Resources";
            Directory.CreateDirectory(directoryPath);
            var resourcePath = "Assets/Resources/NavigatorContainerSettings.asset";
            var settings = Resources.Load<NavigatorContainerSettings>("NavigatorContainerSettings");
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<NavigatorContainerSettings>();
                AssetDatabase.CreateAsset(settings, resourcePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif
    }
}