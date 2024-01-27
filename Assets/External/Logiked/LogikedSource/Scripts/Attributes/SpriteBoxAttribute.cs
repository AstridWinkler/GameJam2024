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
    public sealed class DrawSpriteBox : FutureFieldAttribute
    {
        bool showLabel;
        float size;

        public DrawSpriteBox(bool showLabel = false, float size = 72f)
        {
            this.showLabel = showLabel;
            this.size = size;
        }





#if UNITY_EDITOR
        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            //if(!Context.datas.ContainsKey(nameof(size)) )
            //Context.SetData(nameof(size), 72);
            
            
            //GUILogiked.Panels.DrawSprite((Sprite)property.objectReferenceValue, previewRect);

            // property.objectReferenceValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(70), GUILayout.Width(70));

            //  position.width = Context.AttributeHeight;
            // position.height -= 2;

            if (showLabel)
            {
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(Sprite), false);
            }
            else
            {
                position.height = size;
                position.width = position.height;
                property.objectReferenceValue = EditorGUI.ObjectField(position, property.objectReferenceValue, typeof(Sprite), false);
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);
        }

        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return size;//GetContext(property).GetData<float>(nameof(size));
           
        }




#endif
    }
}

