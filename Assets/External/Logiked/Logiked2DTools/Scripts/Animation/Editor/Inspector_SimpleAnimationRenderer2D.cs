#if UNITY_EDITOR

using logiked.source.editor;
using logiked.Tool2D.animation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace logiked.Tool2D.editor
{

    [CustomEditor(typeof(SimpleAnimationRenderer2D))]
    [CanEditMultipleObjects]
    public class Inspector_SimpleAnimationRenderer2D : Editor
    {

        void OnEnable()
        {
            Repaint();

        }

        public override void OnInspectorGUI()
        {
            SimpleAnimationRenderer2D t = (SimpleAnimationRenderer2D)target;
            serializedObject.Update();
            var lastFile = t.AnimationFile;
            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck())
            {
                if (t.AnimationFile != lastFile)
                    t.SetAnim(t.AnimationFile);
            }




            GUILayout.Space(15);
            GUILayout.Label("Preview", GUILogiked.Styles.Text_BigBold);
            GUILayout.BeginHorizontal();
            GUI.enabled = !t.IsPlaying;
            if (GUILayout.Button("Play"))
                t.Play();
            GUI.enabled = t.IsPlaying;
            if (GUILayout.Button("Stop"))
                t.Stop();
            GUI.enabled = true;


            GUILayout.EndHorizontal();





            //FOR UPDATE SPRITE IN SCENE
            /*  Repaint();
              t.EditorUpdate();
               */




            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif