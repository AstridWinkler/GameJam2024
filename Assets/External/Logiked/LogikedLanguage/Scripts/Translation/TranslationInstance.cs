using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using logiked.source.extentions;
using Karambolo.PO;
using System.Linq;



#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;//Pour l'edition
#endif



namespace logiked.language.translate
{

    /// <summary>
    /// Instance parsée d'un fichier de traduction au format <c>PoEdit</c>.
    /// 
    /// Lorsque un patch de traduction est chargé par le <see cref="TranslationManager"/>, cette classe est générée par <see cref="GetFromFile(TextAsset)"/>.
    /// Elle est la représentation d'un patch de traduction et permet d'obtenir ses chaînes traduites.
    /// Certaines fonctions tels que <see cref="TranslationInstance.SaveFile"/> sont disponibles seulement dans l'éditeur afin de pouvoir enregister les modifications du patch.
    /// 
    /// Pour récuperer des chaines traduites et les informations du patch actuel, favoriser les méthodes de <see cref="TranslationManager"/>
    /// 
    /// <example>
    /// <note>Ces fonctions n'ont pas de raisons d'être utilisées pour un usage normal du plugin. Les classes telles que <see cref="logiked.language.editor.Property_Lstring"/> s'occupent déja de l'ajout et l'enregistrement des données. 
    /// Cepandant elle restent accessibles dans le code éditeur.</note>
    /// 
    /// <code>
    /// 
    ///#if UNITY_EDITOR
    ///        void AddNewKey()
    ///        {
    ///            //Récupère la traduction par défaut du jeu
    ///            TranslationInstance patch = TranslationManager.DefaultTranslate;
    ///
    ///            if (patch != null)
    ///            {
    ///                //Ajout de valeurs
    ///                patch.SaveValue("strings.player.dialog1", "Hello evryoone", true, "jack speaking");
    ///                patch.SaveValue("strings.player.dialog2", "Okay buddy", true, "bob speaking");
    ///
    ///                //Suppression
    ///                patch.RemoveKey("strings.player.dialog1");
    ///                
    ///                //Windows dialog Ask "Are you sure ?"
    ///
    ///                patch.HeaderInfos.Author = "Chon Jenna";
    ///                patch.HeaderInfos.Version = "1.2";
    ///                patch.HeaderInfos.Description = "My custom patch";
    ///                patch.HeaderInfos.Language = "En_US";
    ///
    ///                patch.SaveFile();
    ///                
    /// 
    ///                 //Afficher un GenericMenu contenant une liste des clées existantes
    ///                 GenerateMenu( m => Debug.Log($"Selected key : {m}"));
    /// 
    ///            }
    ///
    ///        }
    ///#endif
    ///
    /// </code>  
    /// </example>   
    /// </summary>


    public class TranslationInstance
    {



        /// <summary>
        /// Nom du fichier parsé
        /// </summary>
        private string fileName;
        /// <summary>
        /// Fichier parsé
        /// </summary>
        private TextAsset file;

        /// <inheritdoc cref="TranslationHeaderInfos"/>
        private TranslationHeaderInfos headerInfos;


        /// <summary>
        /// StringID to text
        /// </summary>
        private Dictionary<string, POSingularEntry> values = new Dictionary<string, POSingularEntry>();


        /// <summary>
        /// Le nom du fichier associé
        /// </summary>
        public string FileName { get => fileName; }

        /// <inheritdoc cref="TranslationHeaderInfos"/>
        public TranslationHeaderInfos HeaderInfos { get => headerInfos; }




        private TranslationInstance(POCatalog entries, TextAsset file)
        {

            this.file = file;
            string fileName = file.text;

            values = new Dictionary<string, POSingularEntry>();

            var Language = entries.Headers.GetOrDefault("Language") ?? "unkown";
            var Author = entries.Headers.GetOrDefault("Author") ?? "unkown";
            var Version = entries.Headers.GetOrDefault("Version") ?? "unkown";
            var Descritpion = entries.Headers.GetOrDefault("Description") ?? "";
           // var LanguageName = entries.Headers.GetOrDefault("LanguageName") ?? "";
            headerInfos = new TranslationHeaderInfos(Language, Author, Version, Descritpion);



            foreach (IPOEntry entry in entries)
            {

                if (entry is POSingularEntry)
                {
                    values.Add(entry.Key.ContextId, ((POSingularEntry)entry));
                }
            }

/*
            Debug.Log($" Patch {fileName}:Loaded {values.Count} entries \n" +
                $"{nameof(Language)}:{Language}\n" +
                $"{nameof(Version)}:{Version}\n" +
                $"{nameof(Author)}:{Author}\n" +
                $"{nameof(Descritpion)}:{Descritpion}");
*/


        }


        /// <summary>
        /// Factory pour les patchs de traduction, interprété depuis un fichier.
        /// <note>Le fichier à utiliser doit être formaté au standar <c>".po"</c> de PoEdit.
        /// </summary>
        /// <param name="text">Le fichier à utiliser au format <c>PoEdit</c>
        /// </param>
        /// <returns>Le patch de traduction</returns>
        public static TranslationInstance GetFromFile(TextAsset text)
        {

            var parser = new POParser(new POParserSettings
            {
                // parser options...
            });

            var result = parser.Parse(text.text);

            if (result.Success)
            {
                var catalog = result.Catalog;
                // process the parsed data...

                var pack = new TranslationInstance(catalog, text);


                return pack;
            }
            else
            {
                var diagnostics = result.Diagnostics;
                // examine diagnostics, display an error, etc...
                foreach(var e in diagnostics)
                    LogikedPlugin_Language.Log(e.ToString(), DebugC.ErrorLevel.Error, LogikedPlugin_Language.Instance);
                throw new Exception($"Error when parsing translation file \"{ text?.name}\" \nContent: { text.text}");
            }
        }





        /// <summary>
        /// Obtenir la valeur associée à la clé dans cette traduction
        /// </summary>
        /// <param name="stringId">La clé</param>
        /// <returns>La traduction. Retourne Null en cas de clé non trouvée.</returns>
        public string GetValue(string stringId)
        {
            if (HasKey(stringId))
                return values[stringId].Translation;

            return null;
        }

        /// <summary>
        /// Rechercher si le patch contient la clé de traduction
        /// </summary>
        /// <param name="stringId">Clée à recherher</param>
        /// <returns>La clé est-t-elle disponible dans le patch ?</returns>

        public bool HasKey(string stringId)
        {
            if (stringId.IsNullOrEmpty()) return false;
            return values.ContainsKey(stringId);
        }


#if UNITY_EDITOR



        /// <summary>
        /// [EDITOR ONLY] Retourne un menu contextuel pour l'editeur qui liste tout les clés de cette traduction.
        /// </summary>
        /// <param name="callOnFinish">Fonction de retour de la chaîne choisie</param>
        /// <param name="generateUntracked">Générer une clé "Untracked" en première position</param>
        public void GenerateMenu(Action<string> callOnFinish, bool generateUntracked = false)
        {
            GenericMenu menu = new GenericMenu();
            if (generateUntracked)
                menu.AddItem(new GUIContent("Untracked"), false, () => callOnFinish(""));


            foreach (var k in values.Keys)
                menu.AddItem(new GUIContent(k.Replace('.', '/')), false, () => callOnFinish(k));


            menu.ShowAsContext();
        }


        /// <summary>
        /// [EDITOR ONLY] Ecrit et sauvegarde cette instance à la place du fichier source <see cref="file"/>
        /// <note>Le fichier est enregistré au format <c>".txt"</c> afin de pouvoir être considéré comme un <see cref="TextAsset"/> par Unity.</note>
        /// </summary>
        public void SaveFile()
        {

            var generator = new POGenerator(new POGeneratorSettings
            {
                //  IgnoreEncoding=true
            });
            

            string[] lst = new string[values.Count];



            var catalog = new POCatalog();

            catalog.Headers = new Dictionary<string, string>();
            catalog.Headers.Add("Author", headerInfos.Author);
            catalog.Headers.Add("Version", headerInfos.Version);
            catalog.Headers.Add("Description", headerInfos.Description);
            catalog.Encoding = "UTF-8";
            catalog.Language = headerInfos.Language;


            foreach (var v in values)
                catalog.Add(v.Value);


            string filePath = AssetDatabase.GetAssetPath(file);

            if (file == null || filePath.IsNullOrEmpty())
            {
                throw new ArgumentException($"filePath [{filePath}] is empty. Patch cannot be saved.");
            }



            LogikedPlugin_Language.Log($"[{filePath}] Language patch saved, entries count = {catalog.Count}");


            
            using (var writer = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite))
            {
                generator.Generate(writer, catalog);
                writer.Flush();
            }




            if (LogikedPlugin_Language.Instance.CreateSamplePoeditFile && file == LogikedPlugin_Language.Instance.DefaultTranslation)
            {
                if (LogikedPlugin_Language.Instance.GeneratedPoeditFile != null)
                    filePath = LogikedPlugin_Language.Instance.GeneratedPoeditFile.GetAssetPath(Logiked_AssetsExtention.PathFormat.AbsolutePath);
                else
                {

                    LogikedPlugin_Language.CheckOrCreateResourceFolders();

    

                    filePath =  Path.Combine(LogikedPlugin_Language.LanguageStreamingAssetPath, "BuiltinLanguagePatch.po");
                }
                catalog.HeaderComments = new POComment[] { new POExtractedComment { Text = "This file was generated from game's default translation pack. By using PoEdit technologies, you can create your own translation pack from it !" } };

                using (var writer = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    generator.Generate(writer, catalog);
                    writer.Flush();
                }

                if (LogikedPlugin_Language.Instance.GeneratedPoeditFile == null)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    DefaultAsset text = (DefaultAsset)AssetDatabase.LoadAssetAtPath(Logiked_AssetsExtention.GetRelativePath(filePath), typeof(DefaultAsset));

                    LogikedPlugin_Language.Instance.GeneratedPoeditFile = text;
                    LogikedPlugin_Language.Instance.SetDirtyNow();
                }
            }


            AssetDatabase.Refresh();

        }

        /// <summary>
        /// [EDITOR ONLY] Enregistre une clé et sa valeur dans l'instance et enregistre le fichier.
        /// </summary>
        /// <param name="key">La clé</param>
        /// <param name="valueText">La valeur</param>
        /// <param name="generateNewKey">Faut-il génerer une nouvelle clé si elle n'hexiste pas déja ?</param>
        /// <param name="comments">Les commentaires à inserer dans le fichier localisé</param>
        /// <returns>La sauvegarde s'est bien effectuée</returns>
        public bool SaveValue(string key, string valueText, bool generateNewKey = false, params string[] comments)
        {

            if (key.IsNullOrWhiteSpace())
            {
                LogikedPlugin_Language.Log("Key is null. Override Patch datas is forbidden.", DebugC.ErrorLevel.Error);
                return false;
            }


            var entry = new POSingularEntry(new POKey(valueText, null, key))
            {
                Translation = valueText,
                Comments = new List<POComment>(comments.Select(m => new POExtractedComment { Text = m }))
            };


          //  LogikedPlugin_Language.Log(comments?.Length);


            if (generateNewKey && !HasKey(key))
            {
                values.Add(key, entry);
            }

            if (HasKey(key))
            {
                values[key] = entry;
                LogikedPlugin_Language.Log(string.Format($"key edited {key} : {valueText}"));

            }
            else
            {
                LogikedPlugin_Language.Log($"Translation with key {key} not found !", DebugC.ErrorLevel.Error);
                return false;
            }

            SaveFile();

            return true;
        }


        /// <summary>
        /// [EDITOR ONLY] Supression d'une clé dans l'instance et sauvegarde le fichier.
        /// </summary>
        /// <param name="key">Valeur de la clé</param>
        public void RemoveKey(string key)
        {
            if (!HasKey(key))
            {
                LogikedPlugin_Language.Log("Cannot remove key " + key,  DebugC.ErrorLevel.Error);
                return;
            }

            values.Remove(key);

            LogikedPlugin_Language.Log($"Key {key} Succefully removed");


            SaveFile();
        }




#endif


    }
}