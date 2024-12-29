namespace UnityEngine.UI.Editors
{
    using System.Linq;

    using UnityEngine;

    using UnityEditor;

    [CustomEditor(typeof(ScreenController), true)]
    public class ScreenControllerEditor : Editor
    {
        private ScreenController _target;

        private string[] _excludeProperties = new string[]
        {
            "m_Script",
            "_layer",
            "_independent",
            "_unconstrainted",
            "_reusable",
            "_sendAnalytics",
        };

        private string _resourcesPath = "";

        // Methods

        private void OnEnable()
        {
            _target = target as ScreenController;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Info();
            Base();

            if (HasOtherProperties())
            {
                AdvancedEditorLayout.Headline("Other Properties");
                DrawPropertiesExcluding(serializedObject, _excludeProperties);
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void Info()
        {
            AdvancedEditorLayout.Headline("Info");

            GameObject targetGameObject = (target as MonoBehaviour).gameObject;
            string assetPath = AssetDatabase.GetAssetPath(targetGameObject);

            bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(targetGameObject);

            if (isPrefab && !string.IsNullOrEmpty(assetPath))
            {
                var style = new GUIStyle(EditorStyles.helpBox);
                style.richText = true;
                EditorGUILayout.LabelField("Prefab", "true", EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                if (IsInsideResources(assetPath))
                    EditorGUILayout.LabelField("Location", GUILayout.Width(EditorGUIUtility.labelWidth - 3));
                else
                    EditorGUILayout.LabelField("Location", GUILayout.Width(EditorGUIUtility.labelWidth - 3));
                if (GUILayout.Button(_resourcesPath, style))
                    EditorGUIUtility.PingObject(_target);
                EditorGUILayout.EndHorizontal();
            }
            else
                EditorGUILayout.LabelField("Prefab", "false", EditorStyles.helpBox);
        }

        private bool IsInsideResources(string assetPath)
        {
            string[] pathComponents = assetPath.Split('/');
            for (int i = 0; i < pathComponents.Length; i++)
            {
                if (pathComponents[i] == "Resources")
                {
                    if (i < pathComponents.Length - 1)
                    {
                        pathComponents[pathComponents.Length - 1] = "<b><color=#00C8FF>" + pathComponents[pathComponents.Length - 1] + "</color></b>";
                        _resourcesPath = string.Join("/", pathComponents, i + 1, pathComponents.Length - i - 1).Replace(".prefab", string.Empty);
                        return true;
                    }
                    else
                    {
                        _resourcesPath = _target.GetType().FullName.Replace(".prefab", string.Empty);
                        return true;
                    }
                }
            }

            return false;
        }

        private void Base()
        {
            AdvancedEditorLayout.Headline("Base");

            var layer = serializedObject.FindProperty("_layer");
            EditorGUILayout.PropertyField(layer, true);

            var independent = serializedObject.FindProperty("_independent");
            EditorGUILayout.PropertyField(independent, true);

            if (!_target.Independent)
            {
                var unconstrainted = serializedObject.FindProperty("_unconstrainted");
                EditorGUILayout.PropertyField(unconstrainted, true);

                if (_target.Unconstrainted)
                    _target.Reusable = false;
                else
                {
                    var reusable = serializedObject.FindProperty("_reusable");
                    EditorGUILayout.PropertyField(reusable, true);
                }
            }

#if ENABLE_ANALYTICS
            var sendAnalytics = serializedObject.FindProperty("_sendAnalytics");
            EditorGUILayout.PropertyField(sendAnalytics, true);
#endif
        }

        private bool HasOtherProperties()
        {
            var iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (!_excludeProperties.Contains(iterator.name))
                    return true;
            }

            return false;
        }
    }
}