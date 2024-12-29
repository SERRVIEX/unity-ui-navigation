namespace UnityEngine.UI
{
    using UnityEngine;

    using UnityEditor;

    [CustomEditor(typeof(Navigator), true)]
    public class NavigatorEditor : Editor
    {
        private Navigator _target;

        private void OnEnable()
        {
            _target = target as Navigator;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Base();
            Canvas();
            InitialScreenController();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }

        private void Base()
        {
            AdvancedEditorLayout.Headline("Base");

            SerializedProperty cameras = serializedObject.FindProperty("_cameras");

            if (cameras.arraySize == 0)
                EditorGUILayout.HelpBox("No cameras are assigned. Please ensure at least one camera is properly assigned.", MessageType.Error);

            EditorGUILayout.PropertyField(cameras, true);

            EditorGUILayout.BeginHorizontal();

            if (cameras.arraySize == 0)
            {
                if(Camera.main != null)
                    if (GUILayout.Button("Add MainCamera"))
                        _target.AddMainCamera(); 
            }

            if (GUILayout.Button("New Camera"))
                _target.NewCamera();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            SerializedProperty events = serializedObject.FindProperty("_eventSystem");
            EditorGUILayout.PropertyField(events, true);
        }

        private void Canvas()
        {
            AdvancedEditorLayout.Headline("Canvas");

            SerializedProperty canvases = serializedObject.FindProperty("_canvases");
            SerializedProperty cameras = serializedObject.FindProperty("_cameras");
            if (canvases.arraySize != cameras.arraySize)
                EditorGUILayout.HelpBox("Canvas array length is different from camera array length.", MessageType.Error);

            EditorGUILayout.PropertyField(canvases, true);

            SerializedProperty layers = serializedObject.FindProperty("_layers");
            EditorGUILayout.PropertyField(layers, true);
        }

        private void InitialScreenController()
        {
            AdvancedEditorLayout.Headline("Initial");

            SerializedProperty initialScreenController = serializedObject.FindProperty("_initialScreenController");
            EditorGUILayout.PropertyField(initialScreenController, true);
        }
    }
}