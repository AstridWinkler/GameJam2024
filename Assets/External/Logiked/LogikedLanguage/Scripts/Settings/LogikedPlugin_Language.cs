
using logiked.language.translate;
using logiked.source.types;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using logiked.source.extentions;
using logiked.source.attributes;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
using logiked.source.database.editor;
#endif

using UnityEngine;


namespace logiked.language
{
    /// <summary>
    /// <inheritdoc/>
    /// 
    /// Les paramètres du patch de langues par défauts sont configurables ici (Langue, Auteur, Version...)
    /// </summary>
    //[CreateAssetMenu(fileName = "LanguageAssemblySettings", menuName = "logiked/Language/AssemblySettings", order = 1)]
    [System.Serializable]
    public class LogikedPlugin_Language : LogikedPlugin<LogikedPlugin_Language>
    {

        /// <summary>
        /// Chemin des fichiers de localisation
        /// </summary>
        public static string LanguageStreamingAssetPath => Application.streamingAssetsPath + "/languages/";

#if UNITY_EDITOR
        /// <summary>
        /// Permet de creer le fichier automatiquement au chargement du script, dans un dossier resource.
        /// Initialise la translation
        /// </summary>
        [UnityEditor.InitializeOnLoad]
       private class InitPlugin
        {
            static InitPlugin()
            {
                LogikedPlugin<LogikedPlugin_Language>.CreateSettingsInstance("settingsLanguage", "Logiked_Language");
                TranslationManager.RestartTranslate();
            }
        }


        [UnityEditor.CustomEditor(typeof(LogikedPlugin_Language))]
        private class LogikedPlugin_Language_Editor : Editor
        {

            public string[] CultureCodes
            {
                get
                {
                    if (cultureCodes == null)
                    {
                        cultureCodes = CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).Select(c => c.Name).ToArray();
                    }

                    return cultureCodes;
                }
            }

            private string[] cultureCodes = null;


            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                LogikedPlugin_Language lang = target as LogikedPlugin_Language;




                if (lang.DefaultTranslation != null && TranslationManager.DefaultTranslate != null)
                {

                    GUILayout.Space(10);
                   var infos = TranslationManager.DefaultTranslate.HeaderInfos;


                    GUILayout.Label("Default patch settings", GUILogiked.Styles.Text_Bold);
                    EditorGUI.indentLevel++;




                    infos.Author = PropertyDrawerFinder.DrawPropertyOject(infos.Author, new GUIContent(nameof(infos.Author)));


                    GUILayout.BeginHorizontal();
                  
                    infos.Language = PropertyDrawerFinder.DrawPropertyOject(infos.Language, new GUIContent(nameof(infos.Language)));

                    if (GUILayout.Button("set", GUILayout.Width(40)))
                    {
                        GUI.FocusControl(null);
                        GUILogiked.Panels.GenericMenuFromStrings(CultureCodes, (w) => infos.Language = CultureCodes[w]).ShowAsContext();
                    }
                    GUILayout.EndHorizontal();


                    infos.Version = PropertyDrawerFinder.DrawPropertyOject(infos.Version, new GUIContent(nameof(infos.Version)));
                  
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel(new GUIContent(nameof(infos.Description)));

                    GUILayout.BeginVertical();
                    GUILayout.Space(3);
                    infos.Description = GUILayout.TextArea(infos.Description);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;

                  if( GUILayout.Button("Save changes")){
                        TranslationManager.DefaultTranslate.SaveFile();
                    }


                    if (!lang.CreateSamplePoeditFile && lang.GeneratedPoeditFile != null)
                    {
                        AssetDatabase.DeleteAsset(lang.GeneratedPoeditFile.GetAssetPath( Logiked_AssetsExtention.PathFormat.AssetRelative));
                        lang.GeneratedPoeditFile = null;
                        lang.SetDirtyNow();

                    }

                    if (lang.CreateSamplePoeditFile && lang.GeneratedPoeditFile == null)
                    {
                        TranslationManager.DefaultTranslate.SaveFile();
                    }



                }

            }
        }





#endif


        internal static void CheckOrCreateResourceFolders()
        {
            if (!Directory.Exists(LanguageStreamingAssetPath))
            {
                Directory.CreateDirectory(LanguageStreamingAssetPath);
            }
        }


        ///<inheritdoc cref="DefaultTranslation"/>
        [SerializeField] [TranslationFileCheck]
        private TextAsset defaultTranslation;


        /// <inheritdoc/>
        //public sealed override Color LogColors => (Color.magenta +Color.white * 0.5f)*0.75f;

        

        [ShowIf(nameof(defaultTranslation),  ShowIfOperations.NotEqual, null)]
        [Tooltip("Creer une copie du patch de traduction au format \".po\" dans le dossier StreamingAssets afin d'encourager les joeurs à créer leur propre patches en partant de ce modèle.")]
        [SerializeField]
        private bool createSamplePoeditFile = true;

        /// <summary>
        /// Creer une copie du patch de traduction au format <c>".po"</c> dans le dossier <c>StreamingAssets</c> afin d'encourager les joueurs à créer leur propre patches en partant de ce modèle.
        /// </summary>
        public bool CreateSamplePoeditFile => createSamplePoeditFile;

        [ShowIfSame()]
        [ShowIf(nameof(createSamplePoeditFile), "==", true)]
        [Tooltip("Fichier d'exemple généré dans le dossier StreamingAssets")]
        [GreyedField]
        [SerializeField]
        private DefaultAsset generatedPoeditFile;


#if UNITY_EDITOR
        /// <summary>
        /// Fichier d'exemple généré dans le dossier <c>StreamingAssets</c>
        /// </summary>
        public DefaultAsset GeneratedPoeditFile { get => generatedPoeditFile; internal set => generatedPoeditFile = value; }
#else
private class DefaultAsset{

}
#endif



        /// <summary>
        /// Le patch de traduction par défaut utilisé par le jeu.
        /// </summary>
        public TextAsset DefaultTranslation
        {
            get => defaultTranslation;
#if UNITY_EDITOR
            set { defaultTranslation = value; EditorUtility.SetDirty(this); AssetDatabase.SaveAssets(); TranslationManager.RestartTranslate(); }
#endif
        }

    }



}
