using logiked.source.utilities;
using UnityEngine;
using logiked.source.attributes.root;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Permet à un entier d'être une liste de Layer.
    /// </summary>
    public class LayerAttribute : FutureFieldAttribute
    {

#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            if (property.propertyType != SerializedPropertyType.Integer)
            {
                GUI.Label(position, label, EditorStyles.label);
                position.x += EditorGUIUtility.labelWidth;
                position.width -= EditorGUIUtility.labelWidth;
                GUI.Box(position, "This Field should be an integer", GUILogiked.Styles.Box_ErrorBox1);
            }
            else
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
           
            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);
        }



        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
              return SIZE_LINE;
        }


#endif

    }
}