#if UNITY_EDITOR

using logiked.source.editor;
using logiked.source.extentions;
using logiked.Tool2D.animation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace logiked.Tool2D.editor
{


    [CustomEditor(typeof(AnimatorRenderer2D))]
    public class Inspector_AnimatorRenderer2D : Editor
    {


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            GUILayout.Space(15);
            GUILayout.Label("Current variables", GUILogiked.Styles.Text_BigBold);


           var rd =  target as AnimatorRenderer2D;

            if (rd != null && rd.IsPlaying)
            {
                var vars = rd.GetVariablesList();

                GUILayout.BeginVertical("box");

                EditorGUI.BeginChangeCheck();
              

                for (var i = 0; i < vars.Length; i++)
                {
                    vars[i].Value = PropertyDrawerFinder.DrawPropertyOject(vars[i].Value, new GUIContent(vars[i].Name));
                }


                if (EditorGUI.EndChangeCheck())
                {
                    vars.ForEach(m => rd.SetStateValue(m.Name, m.Value, false));
                }

                GUILayout.EndVertical();


                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Stop"))
                {
                    rd.Stop();

                }
                GUILayout.EndHorizontal();


            }
            else
            {
                EditorGUILayout.HelpBox("Animator isn't running.", MessageType.Info, true);

                GUI.enabled = rd?.AnimatorFile != null;

                if (GUILayout.Button("Play"))
                {
                    rd.Play();
                }


            }







            //FOR UPDATE SPRITE IN SCENE
            /*  Repaint();
              t.EditorUpdate();
               */




            serializedObject.ApplyModifiedProperties();
        }
    }

}
#endif