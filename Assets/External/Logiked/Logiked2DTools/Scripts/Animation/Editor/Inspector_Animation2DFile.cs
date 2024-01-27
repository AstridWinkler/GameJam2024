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

    [CustomEditor(typeof(Animation2DFile))]
   // [CanEditMultipleObjects]
    public class Inspector_Animation2DFile : Editor
    {


       protected SerializedProperty sprites;
       protected SerializedProperty loopMode;
       protected SerializedProperty duration;


       // public bool autoDrawPreview = true;
        Animation2DReader preview;



        bool needRepaint;


       virtual protected void OnEnable()
        {
            loopMode = serializedObject.FindProperty(nameof(loopMode));
            sprites = serializedObject.FindProperty(nameof(sprites));
            duration = serializedObject.FindProperty(nameof(duration));
            preview = null;
        }

        

        public override void OnInspectorGUI()
        {
            if(Selection.objects?.Length > 1)
            {
                base.DrawDefaultInspector();
                return;
            }

            serializedObject.Update();


            GUILayout.Space(10);



            GUILayout.Label("Settings", GUILogiked.Styles.Text_BigBold);

            EditorGUILayout.PropertyField(duration);

            // duration.floatValue =   (EditorGUILayout.FloatField("FrameRate", duration.floatValue / (float)spriteList.arraySize ) * spriteList.arraySize).Rnd(0.001f);


            EditorGUILayout.PropertyField(loopMode);

            GUILayout.Space(10);
            GUILayout.Label("Frames", GUILogiked.Styles.Text_BigBold);

            DrawOnGuiFrames();

            if (serializedObject.hasModifiedProperties)
                preview = null;

            serializedObject.ApplyModifiedProperties();


            if (preview == null)
            {
                preview = new Animation2DReader((Animation2DFile)target);
                preview.Play();
            }


            if (needRepaint) Repaint();
            needRepaint = false;
        }


        protected virtual void DrawOnGuiFrames()
        {
            EditorGUILayout.PropertyField(sprites);
        }


        public void CloseWindow()
        {
            DestroyImmediate(this, true);
        }





        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("Animation Preview");
        }


        
        public static void DrawPreview(Animation2DReader preview, Rect r)
        {



            if (preview?.file == null || preview.file.Sprites.Length < 1)
            {
                GUI.Label(r, new GUIContent("No preview to show"), GUILogiked.Styles.Text_BoldCentered);
                return;
            }

            Sprite sprite = preview.GetCurrentSprite();

            if (sprite != null)
            {
                GUILogiked.Panels.DrawSprite(sprite, r, GUILogiked.Panels.ImageFitMode.ClampInside);
            }

            if (preview.file is Animation2DFileVariations)
            {
                var dirFile = preview.file as Animation2DFileVariations;
                GUILayout.BeginVertical(GUILayout.Width(80));
                GUILayout.BeginHorizontal();
                GUILayout.Label($"Variation : {preview.Variation} ");
             
                if (GUILayout.Button("-"))
                {
                    preview.Variation--;
                }
                if (GUILayout.Button("+"))
                {
                    preview.Variation++;
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

            }

            GUILayout.BeginVertical(GUILayout.Width(80), GUILayout.ExpandWidth(false));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));


            if (GUILayout.Button(preview.IsPlaying ? "Stop" : "Play", GUILayout.ExpandWidth(true)))
            {
                if (preview.IsPlaying) preview.Stop();
                else preview.Play();
            }

            GUILayout.Label("Time : " + (preview.FrameDeltaCumulated.Cycle(99)).ToString("00.0"), GUILogiked.Styles.Text_Bold, GUILayout.ExpandWidth(true));

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }



        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            DrawPreview(preview, r);
            needRepaint = true;
        }



    }




}


#endif