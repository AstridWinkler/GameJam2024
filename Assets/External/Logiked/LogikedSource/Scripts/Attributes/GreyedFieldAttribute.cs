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
    /// Grise la propriété dans l'inspecteur afin de ne pas pouvoir l'editer
    /// </summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple =true)]
    public class GreyedField : FutureFieldAttribute
    {
        public GreyedField()
        {
            //order = -100;
        }



#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            var g = GUI.enabled;
            GUI.enabled = false;
            CallNextAttribute(position, property, label);
            GUI.enabled = g;
        }





#endif
    }

}
