using logiked.source.attributes.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using logiked.source.types;
using logiked.source.extentions;

#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Attribut similaire à UnityEngine.Range(), mais compatible avec tout les autres attributs Logiked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class RangeLogikedAttribute : FutureFieldAttribute
    {
        public readonly float min;
        public readonly float max;

        public RangeLogikedAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public RangeLogikedAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }


#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {

     

            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer)
            {
  
                GUI.Box(position, new GUIContent($"Property {property.name} must be Integger or Float to use RangeLogikedAttribute."), GUILogiked.Styles.Box_ErrorBox1);


                CallNextAttribute(position, property, label);
                return;
            }


            if (property.propertyType == SerializedPropertyType.Integer)
                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)min, (int)max);
            else
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, min, max);
           
            propertyAlreadyDrawn = true;

          //  Context.SetData("drawn", propertyAlreadyDrawn);

            CallNextAttribute(position, property, label);
        }
        /*

        public override float GetPropertyHeightRecursive(SerializedProperty property, GUIContent label)
        {
            return  GetContext(property).GetData<bool>("drawn") ? SIZE_LINE : base.GetPropertyHeightRecursive(property, label);
        }
        */
        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return SIZE_LINE;
        }








#endif



    }
}
