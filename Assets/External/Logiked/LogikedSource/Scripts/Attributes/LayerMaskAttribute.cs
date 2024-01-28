using logiked.source.utilities;
using UnityEngine;
using logiked.source.attributes.root;
using System.Collections.Generic;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Permet à un entier d'être une liste de Layer.
    /// </summary>
    public class LayerMaskAttribute : FutureFieldAttribute
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
            {
                List<string> layerNames = new List<string>();

                for (int i = 0; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
                {
                    var layerN = LayerMask.LayerToName(i); //get the name of the layer
                    layerNames.Add(i.ToString() + " : " + layerN);
                }

                property.intValue = EditorGUI.MaskField(position, label, property.intValue, layerNames.ToArray());

            }
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