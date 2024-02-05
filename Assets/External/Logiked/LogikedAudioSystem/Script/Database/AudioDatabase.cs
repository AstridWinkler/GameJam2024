using logiked.source.database;
using logiked.source.utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using logiked.source.extentions;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
using logiked.source.database.editor;
#endif

namespace logiked.audio
{

    /// <summary>
    /// Classe qui permet le référencement des audios autiliser dans le projet
    /// </summary>
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = LogikedPlugin_AudioSystem.CreateAssetMenuName+"Audio database")]
    public class AudioDatabase : LogikedDatabase<AudioDatabase, GameSoundFile, DatabaseCategory>
    {

        #region Editor Ticks

        //Initalisation avec des valeurs de AudioDatabase
        [HideInInspector][SerializeField]
        bool startDatas;

#if UNITY_EDITOR
        private void Awake()
        {
            if (!startDatas)
            {
                startDatas = true;
                databaseElementsLabel = "sound";
                linkPressetFolder = true;
                createFolderForEachElements = false;

                if (LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase == null)
                    LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase = this;

                EditorUtility.SetDirty (this);
            }
        }
#endif

        #endregion

#pragma warning disable 0414
        [SerializeField, Tooltip("Connecter cette liste au dossier adjacent 'AudioSourcePresets' ?")]
        private bool linkPressetFolder;
#pragma warning restore 0414



        [SerializeField, Tooltip("Liste des presets d'AudioSource disponibles pour une instantiation rapide.")]
        private List<AudioSource> audioSourcePresetPrefabs = new List<AudioSource>();
        /// <summary>
        /// Liste des presets d'AudioSource disponibles pour une instantiation rapide.
        /// </summary>
        public AudioSource[] AudioSourcePresetPrefabs { get => audioSourcePresetPrefabs.ToArray(); set => audioSourcePresetPrefabs = new List<AudioSource>(value); }


        //[WIP : modifier pour contenir plus d'information (genre type de son, loop etc]
        [SerializeField, Tooltip("[WIP] Liste d'AudioMixer utilisés dans le jeu")]
        private List<AudioMixerGroup> audioMixerLayers = new List<AudioMixerGroup>();

        /// <summary>
        /// Retourne la liste des presets d'AudioSource disponibles.
        /// </summary>
        public string[] AudioSourcePresetPrefabNames => audioSourcePresetPrefabs.ConvertAll(m => m?.name).ToArray();
      
        //[WIP : modifier pour contenir plus d'information (genre type de son, loop etc]
        /// <summary>
        /// Retourne la liste d'AudioMixer disponibles.
        /// </summary>
        public string[] AudioMixerLayersNames => audioMixerLayers.ConvertAll(m => m?.name).ToArray();


        /// <summary>
        /// Renvoi le preset d'AudioSource, stocké sur un prefab 
        /// </summary>
        /// <param name="id">L'id du preset dans la liste</param>
        /// <returns>L'AudioSource correspondant</returns>
        public AudioSource GetAudioSourcePresset(int id) => audioSourcePresetPrefabs[id.Cycle(audioSourcePresetPrefabs.Count)];
        /// <summary>
        /// Renvoi un AudioMixer dans la liste des preset
        /// </summary>
        /// <param name="id">L'id du preset dans la liste</param>
        /// <returns>L'AudioMixer correspondant</returns>
        public AudioMixerGroup GetAudioMixerPresset(int id) => audioMixerLayers.Count==0?null: audioMixerLayers[id.Cycle(audioMixerLayers.Count)];

    

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(AudioDatabase))]
    public class Inspector_AudioDatabase : Inspector_LogikedDatabase
    {
        AudioDatabase targ;

        public override DatabaseAbstractElement CreateNewElement(int category, string fileName)
        {
            return base.CreateNewElement(category, fileName);
        }

        public override bool DrawNewElementPanel()
        {
            return true;
        }

        void Start()
        {

            if (serializedObject.FindProperty("linkPressetFolder").boolValue)
                RefreshPressetsFromFolder();
        }

        public override void OnCustomInspectorGui()
        {
            if (targ == null)
            {
                targ = target as AudioDatabase;
                Start();
            }

            EditorGUILayout.HelpBox("Cette BDD est WIP, beaucoup de fonctionnalités n'ont pas encore été implémentés/Ne servent à rien", MessageType.Error);

            DrawSectionText("Presets");

            EditorGUILayout.BeginVertical("box");
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioSourcePresetPrefabs"));
            GUILayout.EndHorizontal();

            var LinkPressetFolder = serializedObject.FindProperty("linkPressetFolder");
            EditorGUILayout.PropertyField(LinkPressetFolder);


            GUILayout.Space(15);




            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("audioMixerLayers"));
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {

                if (LinkPressetFolder.boolValue)
                {
                    RefreshPressetsFromFolder();
                }

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }


            GUILayout.Space(15);


        }


        void RefreshPressetsFromFolder()
        {

            //Recherche des Pressets dans le dossier
            string folderName = "AudioSourcePresets";

            var f = targ.GetFolderPath();
            if (f.IsNullOrEmpty()) return;
            var path = Path.Combine(f, folderName);

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(targ.GetFolderPath(), folderName);
            }
            else
            {

                var assets = Directory.GetFiles(Logiked_AssetsExtention.GetAbsolutePath(path));
                List<AudioSource> lst = new List<AudioSource>();
                AudioSource comp;

                for (int i = 0; i < assets.Length; i++)
                {
                    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(Logiked_AssetsExtention.GetRelativePath(assets[i]), typeof(GameObject));
                    if (obj is GameObject asset)
                    {
                        comp = asset.GetComponent<AudioSource>();
                        if (comp)
                            lst.Add(comp);
                    }
                }
                targ.AudioSourcePresetPrefabs = lst.ToArray();
            }


        }



        public override void OnItemRemoved(DatabaseAbstractElement element, string assetPath)
        {
            base.OnItemRemoved(element, assetPath);
        }
    }

#endif




}