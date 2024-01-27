#if UNITY_EDITOR
using logiked.Tool2D.animation;
using UnityEditor;
using UnityEngine;

namespace logiked.Tool2D.editor
{



    [CustomEditor(typeof(AnimatorController2DFile))]
    [CanEditMultipleObjects]
    class Inspector_AnimatorController2DFile : Editor
    {

    
        public override void OnInspectorGUI()
        {
            AnimatorController2DFile file = (AnimatorController2DFile)target;

            serializedObject.Update();


            EditorGUILayout.HelpBox("Open animator in the Animator Editor Window to edit it safely.", MessageType.Info);
        
            if (GUILayout.Button("Open editor"))            
                AnimatronWindow2D.EnableAnimotron();

            GUILayout.Space(20);

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();

        }

    }


}

#endif