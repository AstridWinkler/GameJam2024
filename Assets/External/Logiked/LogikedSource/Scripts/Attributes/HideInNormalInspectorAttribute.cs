using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using logiked.source.utilities;
using logiked.source.attributes.root;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace logiked.source.attributes
{
    /// <summary>
    /// Masque la propriété dans l'inspecteur meme si elle est publique / serialisable (mais pas quand l'inspecteur est en mode Debug)
    /// </summary>
    public class HideInNormalInspectorAttribute : FutureFieldAttribute
    {
        public HideInNormalInspectorAttribute()
        {
          //  order = -100;
        }



#if UNITY_EDITOR


        public override float GetPropertyHeightRecursive(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        protected override void OnGUIRecursive(Rect nextPosition, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context) 
        { }



#endif
    }

}
