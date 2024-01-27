using System.Collections;
using System.Collections.Generic;
using logiked.source.attributes;
using UnityEngine;
using System.Reflection;
using logiked.source.attributes.root;
using logiked.source.utilities;
using logiked.source.extentions;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Dessine un entier en mode Sorting layer
    /// </summary>
    public class SortingLayerAttribute : FutureFieldAttribute
    {

#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                Debug.LogError("SortedLayer property should be an integer ( the layer id )");
            }
            else
            {
                SortingLayerField(position, new GUIContent("Sorting Layer"), property, EditorStyles.popup, EditorStyles.label);
            }
            typeof(EditorGUI).DebugClassContentReflection();
            CallNextAttribute(position, property, label);

        }

        public static void SortingLayerField(Rect position, GUIContent label, SerializedProperty layerID, GUIStyle style, GUIStyle labelStyle)
        {
            MethodInfo methodInfo = typeof(EditorGUILayout).GetMethod("SortingLayerField", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(GUIContent), typeof(SerializedProperty), typeof(GUIStyle), typeof(GUIStyle) }, null);

            if (methodInfo != null)
            {
                object[] parameters = new object[] { label, layerID, style, labelStyle };
                methodInfo.Invoke(null, parameters);
            }
        }




#endif
    }
}