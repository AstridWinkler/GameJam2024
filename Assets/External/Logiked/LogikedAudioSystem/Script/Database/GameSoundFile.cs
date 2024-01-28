using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

using logiked.source.extentions;
using logiked.source.attributes;
using logiked.source.database;

#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif



namespace logiked.audio
{
    [CreateAssetMenu(fileName = "AudioFile", menuName = LogikedPlugin_AudioSystem.CreateAssetMenuName + "Audio File")]
    public class GameSoundFile : DatabaseAbstractElement
    {



        #region Create Asset

#if UNITY_EDITOR

        [MenuItem("Assets/Logiked/Create Audio File")]
        private static void CreateAudioFileFromAudioClips()
        {
            var lst = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets).Select(m => m as AudioClip).ToArray();


            var newFileName = lst[0].name;
            newFileName = Regex.Replace(newFileName, @"(\s+)|(\d)", "");
            newFileName = Regex.Replace(newFileName, @"_+$", "");
            newFileName += ".asset";



            var savePath = lst[0].GetAssetPath(Logiked_AssetsExtention.PathFormat.AssetRelative);
            savePath = Path.Combine(Path.GetDirectoryName(savePath), newFileName);


            if (File.Exists(Logiked_AssetsExtention.GetAbsolutePath(savePath)))
            {
                savePath = savePath.Replace(".asset", $"{Time.frameCount}.asset");
                //Debug.LogError("End:" + savePath);

            }



            GameSoundFile file = CreateInstance<GameSoundFile>();

            file.sound = new GameSound(lst);
            file.Database = LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase;

            AssetDatabase.CreateAsset(file, savePath);
            //EditorUtility.SetDirty(file);
            //AssetDatabase.SaveAssets();
        }


        // Note that we pass the same path, and also pass "true" to the second argument.
        [MenuItem("Assets/Logiked/Create Audio File", true)]
        private static bool CreateAudioFileFromAudioClipsValidation()
        {
            // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
            var lst = Selection.GetFiltered(typeof(AudioClip), SelectionMode.Assets);

            return lst.Count() > 0 && lst.Length == Selection.objects.Length;
        }

#endif

#endregion




        public override string ItemName => name;

        /// <summary>
        /// Sons associés à ce fichier.
        /// </summary>
        [SerializeField] private GameSound sound;
        public GameSound Sound => sound;
    


#if UNITY_EDITOR

        [CustomEditor(typeof(GameSoundFile))]
        public class GameSoundFileInspector : Editor
        {
            SerializedProperty tags;
            SerializedProperty categoryId;
            SerializedProperty sound;

            DatabaseTagNumberAttribute tagAttr = new DatabaseTagNumberAttribute();
            DatabaseCategoryNumberAttribue catAttr = new DatabaseCategoryNumberAttribue();

            GameSoundFile file;

            private void OnEnable()
            {
                file = target as GameSoundFile;
                tags = serializedObject.FindProperty(nameof(tags));
                categoryId = serializedObject.FindProperty(nameof(categoryId));
                sound = serializedObject.FindProperty(nameof(sound));
            }





            public override void OnInspectorGUI()
            {

                serializedObject.Update();


                tagAttr.OnGUI(EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight), tags, new GUIContent(tags.displayName, tags.tooltip));

                catAttr.OnGUI(EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight), categoryId, new GUIContent(categoryId.displayName, categoryId.tooltip));


                //EditorGUILayout.PropertyField(categoryId);
                EditorGUILayout.PropertyField(sound);

                if (file.sound.IsFileUsed)
                {
                    EditorGUILayout.HelpBox("Le son va chercher ses données dans un autre fichier. Si cela n'est pas intentionnel, cliquer sur le dossier avec une étoile à droite du champ.", MessageType.Info);
                }

                serializedObject.ApplyModifiedProperties();

            }









            public override bool HasPreviewGUI()
            {
                return true;
            }


            public override void OnPreviewGUI(Rect r, GUIStyle background)
            {
                if (file.sound.IsEmpty)
                {
                    GUI.Label(r, "Aucun son à afficher.");
                    return;
                }

                var clips = file.sound.ClipList;

                float cnt = clips.Length+1;
                float height = r.height / cnt;
                height = height.Clamp(r.width/3);
                height = height.Clamp(120);


                Rect labelPos = r;
                labelPos.height = height;
                labelPos.width = r.width / 2f;
                labelPos.x += r.width / 2f;

                Rect iconPos = labelPos;
                iconPos.width = height / 2f;
                iconPos.height = height / 2f;
                iconPos.x -= height / 2f  + 10;
                iconPos.y += height / 4f;

                Rect iconPos2 = iconPos;
                iconPos2.x -= height / 2;

                Rect lab = r;
                lab.height = height;
                lab.x = iconPos2.x;
                GUI.Label(lab,  "<color=cyan><b>Liste des sons :</b></color>");


                for (int i = 0; i < file.sound.ClipCount; i++)
                {

                    labelPos.y += height;
                    iconPos.y += height;
                    iconPos2.y += height;


                    GUI.Label(labelPos, clips[i]?.name);
                    GUILogiked.Panels.GUIDrawEditorIcon(() =>
                    {
                        //  Logiked_AssetsExtention.SelectObjectInUnityInspector(clips[i])
                        PlayClipEditor(clips[i]);//startSample doesn't work in this function????
                    }, GUILogiked.Panels.EditorIconType.AudioPlay, iconPos2);

                    GUI.Label(labelPos, clips[i]?.name);
                    GUILogiked.Panels.GUIDrawEditorIcon(StopClipEditor
                    , GUILogiked.Panels.EditorIconType.AudioStop, iconPos);


   
                }
            }



            public static void PlayClipEditor(AudioClip clip, int startSample = 0, bool loop = false)
            {
                if (clip == null) return;
                Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
                Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
                MethodInfo method = audioUtilClass.GetMethod(
                    "PlayPreviewClip",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new System.Type[] {
        typeof(AudioClip),
        typeof(Int32),
        typeof(Boolean)
                },
                null
                );

                method.Invoke(
                    null,
                    new object[] {
        clip,
        startSample,
        loop
                }
                );

            }

            public static void StopClipEditor()
            {
                Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
                Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
                MethodInfo method = audioUtilClass.GetMethod(
                    "StopAllPreviewClips",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    new System.Type[] { },
                null
                );

                method.Invoke(
                    null,
                    new object[] { }
                );
            }
        }



#endif



    }
}