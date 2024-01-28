using logiked.language.translate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using logiked.source.attributes;
using logiked.source.attributes.root;
using logiked.source.manager;
using logiked.source.extentions;


#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.language.translate
{
    /// <summary>
    /// Manager pour les patch de traductions. 
    /// Utilise majoritairement des Méthodes statiques, afin de pouvoir fonctionner dans l'éditeur sans avoir d'instance dans la scène.
    /// 
    /// Cepandant, il est recommandé d'avoir une instance de ce script à coté d'un GameManager. De cette manière, on peut on peut modifier le patch de langue actuel depuis un simple boutton UI appelant <see cref="LoadLanguage"/>.
    /// 
    /// 
    /// <example>
    /// 
    /// <code>
    ///public class RandomClass : MonoBehaviour
    ///{
    ///
    ///    public void Start()
    ///    {
    ///
    ///        //Obtenir les infors du patch actuel
    ///        var currentPatch = TranslationManager.GetCurrentPatchInfos();
    ///        Debug.Log(currentPatch.Author);
    ///        Debug.Log(currentPatch.Version);
    ///
    ///        //Changer de patch
    ///        TranslationManager.LoadLanguagePatch("Fr_patch");
    ///
    ///        //Obtenir une clée traduite
    ///        var dialog1 = TranslationManager.GetTranslatedValue("string.dialog.speak1");
    ///
    ///#if UNITY_EDITOR
    ///        //Afficher un GenericMenu contenant une liste des clées existantes
    ///        TranslationManager.DefaultTranslate.GenerateMenu(m => Debug.Log($"Selected key : {m}"));
    ///#endif
    ///
    ///    }
    ///}
    /// </code>   
    /// </example>
    /// 
    /// 
    /// <note>
    /// L'utilisation de <see cref="LocalizedTextComponent"/> et de <see cref="lstring"/> simplifient grandement l'implémentation du plugin en offrant des interfaces intuitives.
    /// </note>
    /// 
    /// 
    /// </summary>


    [ExecuteAlways]
    [Serializable]
    [AddComponentMenu("Logiked/Translation Manager")]
    public sealed class TranslationManager : BaseManager<TranslationManager>
    {




        [TranslationFileCheck]
        [Tooltip("Démarer automatiquement le manager, sans avoir besoin d'un GameManager")]
        [SerializeField]
        bool autoStart;

        protected override void InitManager()
        {
            RestartTranslate();
        }

        private void Awake()
        {
            if (autoStart)
                Initialization();
        }


        /// <summary>
        /// Le patch de patch de traduction par défaut (aussi utilisé en cas de clé non trouvée)
        /// </summary>
        static TranslationInstance defaultTranslate = null;

        /// <summary>
        /// Le patch de traduction actuellement choisi par l'utilisateur 
        /// </summary>
        static TranslationInstance currentTranslate = null;


#if UNITY_EDITOR
        /// <summary>
        /// [EDITOR] Accés au patch de traduction par défaut pour l'édition 
        /// </summary>
        public static TranslationInstance DefaultTranslate => defaultTranslate;
#endif



#region LOAD_FILE

        /// <summary>
        /// Charge le patch de langue contenu dans le dossier StreamingAssets/Language (Ne pas preciser l'extension) 
        /// Un code vide signifie le patch de traduction par défaut.
        /// </summary>
        /// <param name="fileName">Le nom du fichier à utiliser</param>
        /// <returns>Est ce que le changement s'est effectué correctement ?</returns>
        public void LoadLanguage(string fileName = "")
        {
            TryLoadLanguage(fileName);
        }

        /// <summary>
        /// Raccourci statique vers <see cref="LoadLanguage(string)"/>. <br></br>
        /// <inheritdoc cref="LoadLanguage"/>
        /// </summary>

        public static void LoadLanguagePatch(string fileName = "")
        {
           Instance.LoadLanguage(fileName);
        }


        /// <summary>
        /// Tente de charger le patch de langue associé
        /// </summary>
        /// <param name="fileName">Nom du patch de langue</param>
        /// <returns>Est ce que le changement s'est effectué correctement ?</returns>
        private bool TryLoadLanguage(string fileName = "")
        {
            if (!fileName.IsNullOrEmpty())
            {
                var loaded = LoadFile(fileName);
                if (loaded == null)
                {
                    return false;
                }

                try
                {
                    currentTranslate = TranslationInstance.GetFromFile(loaded);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

            }
            else
                currentTranslate = defaultTranslate;

            UpdateSceneLocalizedStrings();
            return true;
        }



        /// <summary>
        /// Charger le patch de langue associé
        /// </summary>
        /// <param name="fileName">Nom du patch de langue</param>
        /// <returns>Est ce que le changement s'est effectué correctement ?</returns>     
        private static TextAsset LoadFile(string fileName)
        {
            LogikedPlugin_Language.CheckOrCreateResourceFolders();

            string path = Path.Combine(LogikedPlugin_Language.LanguageStreamingAssetPath, $"{fileName}.po");

            if (!File.Exists(path))
            {
                LogikedPlugin_Language.Log($"Language file \"{path}\" not found !", DebugC.ErrorLevel.Error);
                return null;
            }

            TextAsset textAsset = new TextAsset(File.ReadAllText(path)); // (TextAsset)Resources.Load(path);
            textAsset.name = fileName;
            return textAsset;
        }







#endregion






        /// <summary>
        /// Rechargement du patch par défaut
        /// </summary>
        public static void RestartTranslate()
        {

            if (LogikedPlugin_Language.Instance.DefaultTranslation == null)
            {
                 LogikedPlugin_Language.Log("No default language patch were set", DebugC.ErrorLevel.Warning, LogikedPlugin_Language.Instance);
                return;
            }

            try
            {

                defaultTranslate = TranslationInstance.GetFromFile(LogikedPlugin_Language.Instance.DefaultTranslation);

            }catch(Exception e)
            {
                Debug.LogException(e);
#if UNITY_EDITOR
                LogikedPlugin_Language.Instance.DefaultTranslation = null;
                LogikedPlugin_Language.Instance.SetDirtyNow();
#endif
            }

            currentTranslate = defaultTranslate;
            UpdateSceneLocalizedStrings();
        }

        public static void UpdateSceneLocalizedStrings()
        {
            var listTranslate = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ILocalizedComponent>();

            foreach (var item in listTranslate)
                item.RefreshTranslate();
        }




        /// <summary>
        /// Obtenir un texte traduit. Si la chaine n'est pas disponible dans le patch actuel, c'est la valeur contenu dans le patch par défaut qui est retournée.
        /// </summary>
        /// <param name="id">La clé du texte à obtenir</param>
        /// <param name="warnIfNull">Log un message d'erreur si la clé n'a pas été trouvée</param>
        /// <returns>La chaine traduite (Null aucune chaîne n'a été trouvée)</returns>
        public static string GetTranslatedValue(string id, bool warnIfNull = false)
        {
#if UNITY_EDITOR
            if (currentTranslate == null)
                RestartTranslate();
#endif
            if (currentTranslate == null)
            {
                LogikedPlugin_Language.Log("No translation found", DebugC.ErrorLevel.Error);
                return "";

            }


            if (id.IsNullOrEmpty())//Empecher l'accés aux infos de la trad
                return null;

            string loc = null;

#if UNITY_EDITOR
            if (Application.isPlaying)//If editing, use only the defaultTranslate
#endif


                loc = currentTranslate.GetValue(id);

            if (loc == null)
                loc = defaultTranslate.GetValue(id);


            if (loc == null && warnIfNull)
                LogikedPlugin_Language.Log("null translation key:" + id);// + "\nStored key cnt :" + values.Count);

            return loc;
        }



        /// <summary>
        /// Retourne les informations d'en-tête du patch actuellement utilisé (langue, description, version..)
        /// </summary>
        /// <returns>Les informations du patch actuel</returns>
        public static TranslationHeaderInfos GetCurrentPatchInfos()
        {
#if UNITY_EDITOR
            if (currentTranslate == null)
                RestartTranslate();
#endif

            if (currentTranslate == null)
            {
                LogikedPlugin_Language.Log("No translation found", DebugC.ErrorLevel.Error);
                return null;
            }

            return currentTranslate.HeaderInfos;
        }



#if UNITY_EDITOR


            /// <summary>
            /// [EDITOR] Dessine un boutton dans l'editeur qui propose à l'utilisateur de créer un patch par défaut si aucun patch n'a été trouvé. 
            /// </summary>
            /// <returns>Retourne le choix de l'utilisateur</returns>

            public static bool CheckAndDrawDefaultTranslateHelper()
        {


            if (LogikedPlugin_Language.Instance.DefaultTranslation != null && DefaultTranslate == null)
                RestartTranslate();

            if (LogikedPlugin_Language.Instance.DefaultTranslation != null && DefaultTranslate != null) return true;





            UnityEditor.EditorGUILayout.HelpBox("No default translation file were found!", MessageType.Error);
            if (!GUILayout.Button("Fix")) return false;

            string patchName = "DefaultPatch.txt";
            System.IO.File.WriteAllText(Application.dataPath + "/" + patchName, " ", System.Text.Encoding.UTF8);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            TextAsset text = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/" + patchName, typeof(TextAsset));
            LogikedPlugin_Language.Instance.DefaultTranslation = text;

            TranslationManager.DefaultTranslate.SaveFile();

            return true;

        }

#endif

    }


}













