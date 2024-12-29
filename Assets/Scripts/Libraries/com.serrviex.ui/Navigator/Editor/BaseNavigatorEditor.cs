namespace UnityEngine.UI
{
    using UnityEngine;

    using UnityEditor;

    [CustomEditor(typeof(BaseNavigator), true)]
    public class BaseNavigatorEditor : Editor
    {
        private BaseNavigator _target;

        private void OnEnable()
        {
            _target = target as BaseNavigator;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AdvancedEditorLayout.Headline("Settings");

            SerializedProperty navigator = serializedObject.FindProperty("_navigator");
            EditorGUILayout.PropertyField(navigator, true);
            
            SerializedProperty makeAsSingletone = serializedObject.FindProperty("_makeAsSingletone");
            EditorGUILayout.PropertyField(makeAsSingletone, true);

            if(_target.MakeAsSingletone)
                EditorGUILayout.HelpBox("Keep this navigator across all scenes (this object is not destroyed on scene changing).", MessageType.Info);
            else
                EditorGUILayout.HelpBox("Each scene must have its own navigator.", MessageType.Info);

            AdvancedEditorLayout.Headline("Only Editor");
            DeviceUtils.SimulateDevice = (DeviceUtils.DeviceType)EditorGUILayout.EnumPopup("Simulated Device (Shared)", DeviceUtils.SimulateDevice);

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}