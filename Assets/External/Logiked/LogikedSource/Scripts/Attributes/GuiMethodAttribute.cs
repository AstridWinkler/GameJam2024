using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using logiked.source.utilities;

using logiked.source.attributes.root;
using System;
using System.Reflection;
using logiked.source.extentions;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif


namespace logiked.source.attributes
{
    [AttributeUsage( AttributeTargets.Field, AllowMultiple =true)]
    public class GuiMethodAttribute : FutureFieldAttribute
    {

  
        string  methodName;
        object[] parameters = new object[0];

        public GuiMethodAttribute(string methodName, params object[] parameters )
        {
            this.methodName = methodName;
            this.parameters = parameters;
        }



#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            var obj = property.GetReflectedValueAtPath(methodName);


            if (methodName != null)
            {


            //    method.Invoke(obj, parameters);
            }



            var g = GUI.enabled;
            GUI.enabled = false;
            CallNextAttribute(position, property, label);
            GUI.enabled = g;
        }





#endif
    }

}
