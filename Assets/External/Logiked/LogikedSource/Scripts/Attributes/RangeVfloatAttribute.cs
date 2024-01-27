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
    public class RangeVfloatAttribute : FutureFieldAttribute
    {
        public readonly float min;
        public readonly float max;


        public RangeVfloatAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public RangeVfloatAttribute(int min, int max)
        {
            this.min = min;
            this.max = max;
        }


#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
  {
            
            var type = (property.type);


            if (type == "vfloat")
            {
               //GUILayout.Label(property.name);
                var f = (vfloat)property.GetValue();
                var val = property.FindPropertyRelative("value");
                val.floatValue = val.floatValue.Clamp(min, max);
                CallNextAttribute(position, property, label);

                return;
            }



            Debug.LogError($"Property {property.name} must be vfloat to use RangeVfloatAttribute.");
            property.DrawPropertyField(position, label);

            CallNextAttribute(position, property, label);

        }


        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return SIZE_LINE;
        }

#endif



    }
}
