#if UNITY_EDITOR

//C# Example (LookAtPointEditor.cs)
using UnityEngine;
using UnityEditor;
using logiked.Tool2D.animation;
using logiked.source;
using logiked.source.editor;
using logiked.source.extentions;

namespace logiked.Tool2D.editor
{

    [CustomEditor(typeof(Animation2DFileVariations))]
    [CanEditMultipleObjects]
    public class Inspector_Animation2DFileMulti : Inspector_Animation2DFile
    {
        protected SerializedProperty spritesVariations;
        protected SerializedProperty variationCountMode;
        protected SerializedProperty customVariationCount;
        

        override protected void OnEnable()
        {
            base.OnEnable();
            spritesVariations = serializedObject.FindProperty(nameof(spritesVariations));
            variationCountMode = serializedObject.FindProperty(nameof(variationCountMode));
            customVariationCount = serializedObject.FindProperty(nameof(customVariationCount));
            
        }
        


        protected override void DrawOnGuiFrames()
        {
            EditorGUILayout.PropertyField(variationCountMode);
            EditorGUILayout.PropertyField(customVariationCount);
            EditorGUILayout.PropertyField(spritesVariations); 
        }
        



    }




}


#endif