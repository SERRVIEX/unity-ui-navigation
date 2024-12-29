using UnityEditor;

public static class ComponentsUtil
{
    private static SerializedObject _source;

    // Methods

    [MenuItem("CONTEXT/Component/Copy Properties")]
    public static void CopySerialized(MenuCommand command)
    { 
        _source = new SerializedObject(command.context); 
    }

    [MenuItem("CONTEXT/Component/Paste Properties")]
    public static void PasteSerialized(MenuCommand command)
    {
        // Check if they're the same type - if so do the ordinary copy/paste.
        if (_source.targetObject.GetType() == command.context.GetType())
        {
            EditorUtility.CopySerialized(_source.targetObject, command.context);
            return;
        }

        SerializedObject dest = new SerializedObject(command.context);
        SerializedProperty iterator = _source.GetIterator();

        // Jump into serialized object, this will skip script type so that we dont override the destination component's type.
        if (iterator.NextVisible(true))
        {
            while (iterator.NextVisible(true)) // Itterate through all serializedProperties.
            {
                // Try obtaining the property in destination component.
                SerializedProperty property = dest.FindProperty(iterator.name);

                // Validate that the properties are present in both components, and that they're the same type
                if (property != null && property.propertyType == iterator.propertyType)
                {
                    // Copy value from source to destination component.
                    dest.CopyFromSerializedProperty(iterator);
                }
            }
        }
        dest.ApplyModifiedProperties();
    }
}
