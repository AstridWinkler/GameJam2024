
#if UNITY_EDITOR
using UnityEditor;
#endif


using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

namespace logiked.source.types
{




    /// <summary>
    /// Fichier-signleton de configuration de l'assembly.
    /// </summary>
    /// <seealso cref="ScriptableObject" />
    public abstract class LogikedPlugin<T> : ScriptableObject where T : ScriptableObject
    {

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







#if UNITY_EDITOR
        /// <summary>
        /// Creer une instance tu plugin T dans un dossier resource, dans un répertoire parralelle à Path
        /// </summary>    
        public static bool CreateSettingsInstance(/*string path, */string fileName, string asbName)
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

                //UnityEditor.AssetDatabase.SaveAssets();
                Debug.Log("<color=#800080><size=16>Automatic setting file created : " + fic + "</size></color>");
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

