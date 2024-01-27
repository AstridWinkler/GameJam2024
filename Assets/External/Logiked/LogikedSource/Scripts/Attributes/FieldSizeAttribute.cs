using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using logiked.source.utilities;

using logiked.source.attributes.root;
using System;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif


namespace logiked.source.attributes
{

    /// <summary>
    /// Dessine un champ de sprite en carré, c'est plus visuel qu'une ligne
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldSizeAttribute : FutureFieldAttribute
    {

        float labelWidth;
        bool fieldWidthCoef;

        public FieldSizeAttribute(float labelWidthCoef = 1f, bool modifyWidth = true)
        {
            this.labelWidth = labelWidthCoef;
            this.fieldWidthCoef = modifyWidth;
        }






#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {

            var sub = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth *= labelWidth;
            sub = sub - EditorGUIUtility.labelWidth;

            if (this.fieldWidthCoef)
            {
                EditorGUIUtility.fieldWidth += sub;
            }
        
           //     EditorGUIUtility.fieldWidth *= fieldWidthCoef;



            position.width = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;
        
            CallNextAttribute(position, property, label);
        }

    




#endif
    }
}

