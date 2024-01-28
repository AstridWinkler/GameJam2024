
#if UNITY_EDITOR
using UnityEditor;
#endif


using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using logiked.source.extentions;

namespace logiked.source.types
{


     interface ILogikedPlugin
    {
        public Color LogColors { get; }
    }


    internal class LogikedPlugin : LogikedPlugin<LogikedPlugin>
    {
        internal const string documentation = "https://logiked.github.io/LogikedAssembliesOnlineDocumentation/";


        internal const string documentation_ConfigurationWindow = documentation + "articles/Features/logikedSourceProjectWindow/page.html";

        internal const string documentation_ConfigurationWindow_navigation = documentation_ConfigurationWindow + "#navigation";

        internal const string documentation_ConfigurationWindow_packages = documentation_ConfigurationWindow + "#packages";
        internal const string documentation_ConfigurationWindow_scenes = documentation_ConfigurationWindow + "#scenes";
        internal const string documentation_ConfigurationWindow_globals  = documentation_ConfigurationWindow + "#globals";

    }


    /// <summary>
    /// Scriptable object unique à chaque plugin permetant la configuration des assembly Logiked. 
    /// Accessible via le code par un accesseur statique <c>Instance</c> afin d'être disponible dans tout le projet.
    /// Accessible dans l'inspecteur unity dans le dossier Resources/Logiked
    /// </summary>
    public abstract class LogikedPlugin<T> : ScriptableObject, ILogikedPlugin where T : ScriptableObject
    {
        public class DefaultLogikedPlugin : LogikedPlugin<DefaultLogikedPlugin>
        {

        }



        public const string MenuItemName = "Assets/Source/";

        public const string LabelName = "LogikedPluginSettings";

        static T _instance = null;
        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    var lst = Resources.LoadAll("Logiked", typeof(T));
                    if (lst.Count() > 0)
                        _instance = (T)lst[0];

                }
                if (!_instance)
                    Debug.LogError("Logiked plugin instance null! ");

                return _instance;
            }
        }

        /// <summary>
        /// Couleur des logs de ce package
        /// </summary>
        public virtual Color LogColors { get => Color.gray; }


        /// <summary>
        /// Les les informations du plugin Logiked Source
        /// </summary>
         public static void Log(object message, DebugC.ErrorLevel errorLevel  = DebugC.ErrorLevel.Log, Object context = null)
        {
            DebugC.Log($"{message}",                  
                ((ILogikedPlugin)Instance).LogColors, 
                typeof(T).Assembly.GetName().Name, errorLevel: errorLevel, context: context);
        }



#if UNITY_EDITOR
        /// <summary>
        /// Creer une instance du plugin <c>T</c> dans un dossier resource, dans un répertoire parralelle à Path
        /// </summary>    
        internal protected static bool CreateSettingsInstance(/*string path, */string fileName, string asbName)
        {

            //path = "Assets" + System.Text.RegularExpressions.Regex.Split(path, "Assets")[1];
            //var dir = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(path))) + "\\Resources";
            var dir = "Assets\\Resources\\Logiked";
            var fic = dir + "\\" + fileName + ".asset";
            T ass = null;



            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);




            if (!File.Exists(fic))
            {
                ass = CreateInstance<T>();
                AssetDatabase.CreateAsset(ass, fic);
                AssetDatabase.SetLabels(ass, new string[] { LabelName, asbName });
                _instance = ass;
                //UnityEditor.AssetDatabase.SaveAssets();
                Log($"Automatic setting file created : {fic}");
                //  packages.EditorCoroutines.EditorCoroutineUtility.StartCoroutine(SaveAssetFile(), ass);
                return true;
            }
            else
            {

                _instance = AssetDatabase.LoadAssetAtPath<T>(fic);
            }

            /*
            IEnumerator SaveAssetFile()
            {
                yield return new packages.EditorCoroutines.EditorWaitForSeconds(1f);

            }*/
            return false;
        }
#endif

    }
}

