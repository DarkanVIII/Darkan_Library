using UnityEditor;
using UnityEngine;

namespace Darkan.Editor
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExposedScriptableObjectAttribute : PropertyAttribute
    {

    }

    [CustomPropertyDrawer(typeof(ExposedScriptableObjectAttribute))]
    public class ExposedScriptableObjectAttributeDrawer : PropertyDrawer
    {
        UnityEditor.Editor _editor = null;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType is not SerializedPropertyType.ObjectReference)
            {
                label.text = "Only use ExposedScriptableObject on Scriptable Objects";
                EditorGUI.LabelField(position, label);
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);

            if (property.objectReferenceValue is not ScriptableObject) return;

            if (property.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
            }

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                if (!_editor)
                {
                    UnityEditor.Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
                }

                _editor.OnInspectorGUI();

                EditorGUI.indentLevel--;
            }
        }
    }
}
