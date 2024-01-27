#if UNITY_EDITOR
using logiked.source.attributes;
using logiked.source.extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Object = UnityEngine.Object;





namespace logiked.source.editor
{



    /// <summary>
    /// Script qui gère la configuration du plugin au sein de ton projet. Signleton.
    /// </summary>
    [Serializable]
    public class LogikedProjectConfig : ScriptableObject
    {

        public const int LogikedMenuItemPriority = 121;


        const string LogikedMenuItemBannerName = "Assets/===== LOGIKED =====";
        [MenuItem(LogikedMenuItemBannerName, priority = LogikedMenuItemPriority - 1, validate = false)]
        static void LogikedBanner() { }
        [MenuItem(LogikedMenuItemBannerName, true, priority = LogikedMenuItemPriority - 1)]
        static bool LogikedBanner_() { return false; }


        /*        

          /// <summary>
          /// Textures for <see cref="logiked.source.editor.GUILogiked"/>
          /// </summary>
          [SerializeField]
          private List<Texture2D> editorTextureCache;

  #if UNITY_EDITOR
          internal void AddTextureToCache(Texture2D tex) { editorTextureCache.Add(tex); }
          internal Texture2D GetTextureFromCache(string name) { return editorTextureCache.FirstOrDefault(m => m != null && m.name == name); }

  #endif
          */

        [Header("Edit")]

        /// <summary>
        /// Liste de toute les scenes utilisées fréquement dans le projet. Utilisé par la fenêtre HelperWindows.
        /// </summary>
        [Tooltip("List of frequent used Scenes that appear in the \"Scene\" tab")]
        [SerializeField] private List<SceneAsset> frequentlyUsedScenes = new List<SceneAsset>();
        public SceneAsset[] FrequentlyUsedScenes => frequentlyUsedScenes.ToArray();


        [Serializable]
        public class LogikedGlobalVariableAccess
        {
            public enum LogikedGlobalVarianle { ObjectField }//, SceneSingletonField } peut poser d'imenses problemes si ca permet de configurer un singleton qui est dans une scene particuliere
                                                             //  [Tooltip("La variable provient-t-elle d'un Objet dans les fichiers ou d'un singleton ?")]
            public LogikedGlobalVarianle globalType = LogikedGlobalVarianle.ObjectField;
            [ShowIf(nameof(globalType), ShowIfOperations.Equal, LogikedGlobalVarianle.ObjectField)]
            public Object serializedObject;
            public string fieldPath;
            public string displayedName;
            public string FieldName => displayedName.IsNullOrEmpty() ? fieldPath.Split('.').Last() : displayedName;


            public ReflectExtention.ReflectedObject ReflectedObject => serializedObject.GetReflectedValueAtPath(fieldPath);


        }


        #region Globals 


        /// <summary>
        /// Liste de toutes les variable globales à acceder 
        /// </summary>
        [Tooltip("List of all Globals used in the \"Globals\" tab")]
        [SerializeField] private List<LogikedGlobalVariableAccess> globalVariablePath = new List<LogikedGlobalVariableAccess>();
        public LogikedGlobalVariableAccess[] ReflectionGlobalVariable => globalVariablePath.ToArray();



        [Serializable]
        private class GlobalSaving
        {


            /// <summary>
            /// Classe pour serialiser des arrays de UnityObject
            /// </summary>
            [Serializable]
            private class UnityArraySerializerHelper
            {
                public UnityObjectSerializerHelper[] list;
                public string type;
                public bool array;
            }

            /// <summary>
            /// Classe pour serialiser des liens vers des UnityObject
            /// </summary>
            [Serializable]
            private class UnityObjectSerializerHelper
            {
                [SerializeField]
                public string guid;
                [SerializeField]
                public string type;

                public UnityObjectSerializerHelper(Object obj, Type t)
                {
                    type = t.AssemblyQualifiedName;
                    guid = AssetDatabase.AssetPathToGUID(obj.GetAssetPath(Logiked_AssetsExtention.PathFormat.AssetRelative));
                }

                public Object RetrieveObject()
                {
                    try
                    {
                        var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        return AssetDatabase.LoadAssetAtPath(assetPath, Type.GetType(type));
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Asset not exist ! Check LogikedGlobals.");
                        Debug.LogException(e);
                    }
                    return null;
                }
            }


            /// <summary>
            /// Cette classe sert à serialiser ce qu'on veut pour les configurations <see cref="globalVariablePath"/>.
            /// Elle suporte les les Unity.Object[], System.Object, etc... Mais c'est peutetre pas trés propre.
            /// </summary>
            [Serializable]
            internal class LogikedConfigSerializer
            {
                [SerializeField]
                byte[] value;


                public LogikedConfigSerializer(object savedObject)
                {
                    value = new byte[0];

                    if (savedObject == null) return;

                    var objType = savedObject.GetType();

                    if (savedObject is Object)
                    {
                        savedObject = new UnityObjectSerializerHelper(savedObject as Object, objType);
                    }

                    //SI on a affaire a un array de UnityObjects,on le serialize en array de GUIID
                    if (objType.IsGenericArray())
                    {
                        List<UnityObjectSerializerHelper> guidArray = new List<UnityObjectSerializerHelper>();
                        var arrayElemenType = objType.GetGenericArrayElementType();

                        if (arrayElemenType.Is<Object>())
                        {
                            foreach (var elemnt in savedObject as IList)
                                guidArray.Add(new UnityObjectSerializerHelper(elemnt as Object, arrayElemenType));

                            UnityArraySerializerHelper endObj = new UnityArraySerializerHelper();

                            endObj.array = objType.Is(typeof(Array));
                            endObj.list = guidArray.ToArray();
                            endObj.type = arrayElemenType.AssemblyQualifiedName;

                            savedObject = endObj;
                        }
                    }


                    IFormatter formatter = new BinaryFormatter();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, savedObject);
                        value = stream.ToArray();
                    }
                }


                public object Deserialize()
                {
                    IFormatter formatter = new BinaryFormatter();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(value, 0, value.Length);
                        stream.Seek(0, SeekOrigin.Begin);
                        return formatter.Deserialize(stream);
                    }
                }
            }


            [SerializeField]
            internal LogikedConfigSerializer[] objectArray = new LogikedConfigSerializer[0];

            internal GlobalSaving()
            {
            }

            internal GlobalSaving(List<LogikedGlobalVariableAccess> reflectionGlobalVariable)
            {
                objectArray = new LogikedConfigSerializer[reflectionGlobalVariable.Count];

                for (int i = 0; i < objectArray.Length; i++)
                {
                    //   Debug.Log(reflectionGlobalVariable[i].FieldName + " " + reflectionGlobalVariable[i].ReflectedObject.Value);
                    objectArray[i] = new LogikedConfigSerializer(reflectionGlobalVariable[i].ReflectedObject.Value);
                }
            }

            /// <summary>
            /// Appelé au rechargement de valeurs
            /// </summary>
            internal void LoadValues(List<LogikedGlobalVariableAccess> toAssign)
            {

                for (int i = 0; i < objectArray.Length && i < toAssign.Count; i++)
                {

                    if (objectArray[i] == null || toAssign[i] == null) continue;

                    var val = objectArray[i].Deserialize();


                    //Pour les fichiers UnityObjects complexes : 

                    if (val is UnityObjectSerializerHelper)
                        val = (val as UnityObjectSerializerHelper).RetrieveObject();



                    if (val is UnityArraySerializerHelper)
                    {
                        UnityArraySerializerHelper res = val as UnityArraySerializerHelper;


                        var retrievedArrayType = ReflectExtention.ByName(res.type);


                        Type listType = typeof(List<>).MakeGenericType(new[] { retrievedArrayType });
                        IList list = (IList)Activator.CreateInstance(listType);



                        for (int j = 0; j < res.list.Length; j++)
                        {
                            list.Add(res.list[j].RetrieveObject());
                        }

                        if (res.array)
                            val = list.CastListToArray();
                        else
                            val = list;
                    }

                    toAssign[i].ReflectedObject.SetValue(val);

                    if (toAssign[i].serializedObject != null)
                        toAssign[i].serializedObject.SetDirtyNow();
                }
            }


            public void OnBeforeSerialize()
            {
                /*
                Debug.LogWarning("Saving Content Printing");
                int k = 0;
                foreach (var x in objectArray)
                {
                    Debug.LogError(k++ + " "  + x.Deserialize()?.ToString());

                }
                */
            }

            #endregion
        }




        [Space(20)]
        [Header("Internal")]


        /// <summary>
        /// Liste de tout les modules Loigiked activés dans le projet
        /// </summary>
        [GreyedField]
        [SerializeField] private List<string> activeModulesName = new List<string>();


        #region GlobalSaving

        List<GlobalSaving> GlobalSaves
        {
            get
            {
                if (savedGlobals.Count < 2)
                {
                    savedGlobals.Add(null);
                    savedGlobals.Add(null);
                }

                return savedGlobals;
            }
        }

        [GreyedField]
        [SerializeField]
        List<GlobalSaving> savedGlobals = new List<GlobalSaving>(2);


        public enum GlobalWindowConfig { Editor = 0, Build = 1 }

        [SerializeField]
        [GreyedField]
        public GlobalWindowConfig globalMode = GlobalWindowConfig.Editor;


        [InitializeOnLoadMethod]
        static void EditorGlobals_SetGlobalOnLoad()//Recharger les globals dés que ca démare
        {
            //  Instance.EditorGlobals_ReloadSave();
        }

        public void EditorGlobals_ChangeMode(GlobalWindowConfig newMode)
        {


            //Debug.Log((int)globalMode + " saving");

            GlobalSaves[(int)globalMode] = new GlobalSaving(globalVariablePath);
            globalMode = newMode;
            EditorGlobals_ReloadSave();
        }

        void EditorGlobals_ReloadSave()
        {
            if (GlobalSaves[(int)globalMode] != null) GlobalSaves[(int)globalMode] = new GlobalSaving();

            if (globalVariablePath == null) globalVariablePath = new List<LogikedGlobalVariableAccess>();

            //   Debug.Log((int)globalMode + " reloadin");
            GlobalSaves[(int)globalMode].LoadValues(globalVariablePath);
            //  Debug.Log("resaving");
            GlobalSaves[(int)globalMode] = new GlobalSaving(globalVariablePath);


            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }


        [SerializeField, HideInInspector]
        private GlobalWindowConfig globalModeBeforeBuild;

        /// <summary>
        /// Classe qui gère le changement de config de <see cref="globalModeBeforeBuild"/> vers <see cref="GlobalWindowConfig.Build"/> au moment du build.
        /// </summary>
        private class OnStartBuildGlobalVariableApply : IPreprocessBuildWithReport, IPostprocessBuildWithReport
        {
            public int callbackOrder { get { return 0; } }

            public void OnPreprocessBuild(BuildReport report)//Passage en mode build
            {
                Instance.globalModeBeforeBuild = Instance.globalMode;
                Instance.EditorGlobals_ChangeMode(GlobalWindowConfig.Build);
                Application.logMessageReceived += OnBuildError;
            }

            private void OnBuildError(string condition, string stacktrace, LogType type)
            {
                if (type == LogType.Error || condition.Contains("Cancelled"))//Restore de la config précédente
                {
                    // FAILED TO BUILD, STOP LISTENING FOR ERRORS
                    Application.logMessageReceived -= OnBuildError;
                    Instance.EditorGlobals_ChangeMode(Instance.globalModeBeforeBuild);
                }
            }


            public void OnPostprocessBuild(BuildReport report)
            {
                Instance.EditorGlobals_ChangeMode(Instance.globalModeBeforeBuild);//Restore de la config précédente

                // IF BUILD FINISHED AND SUCCEEDED, STOP LOOKING FOR ERRORS
                Application.logMessageReceived -= OnBuildError;
            }
        }

        #endregion


        #region WorkSpaces 

        /// <summary>
        /// Gestion des listes de fichier dans la section "Naviation" de <see cref="LogikedConfigurationWindow"/>
        /// </summary>
        [Serializable]
        public class WorkSpaceFrame
        {
            public string name;
            public List<Object> files = new List<Object>();
        }

        public WorkSpaceFrame[] Workspaces { get => workspaces; set => workspaces = value; }
        [SerializeField, Tooltip("List of files in LogikedConfigurationWindow => Navigation section")]
        private WorkSpaceFrame[] workspaces = new WorkSpaceFrame[0];

        public int selectedWorkspace = -1;
        public WorkSpaceFrame CurrentWorkspace => selectedWorkspace < 0 || selectedWorkspace >= Workspaces.Length || Workspaces.Length == 0 ? null : Workspaces[selectedWorkspace];

        #endregion




        //Static



        public static readonly string LogikedPluginsAbsolutePath = Path.GetDirectoryName(Path.GetDirectoryName(new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName()));
        //public static readonly string LogikedPluginsRelativePath = Logiked_AssetsExtention.GetRelativePath(LogikedPluginsAbsolutePath);
        //Chemin de ce script, puis
        //On remonte au dossier de tout les assemblies logiked

        public static readonly string LogikedResourcesRelativePath = "Assets/Resources/Logiked/";
        public static string LogikedAbsoluteConfigFolder => Logiked_AssetsExtention.GetAbsolutePath(LogikedResourcesRelativePath);

        //public static readonly string LogikedConfigFullPath = LogikedConfigFolderPath + "ProjectSettings.txt";
        public static readonly string LogikedConfigPath = LogikedResourcesRelativePath + "/ProjectSettings.asset";


        [InitializeOnLoadMethod]
        private static void OnLoadCreateProjectConfig()
        {
            _ = Instance;//Creer l'instance
            RebuildActivePuginsCache();
        }



        /// <summary>
        /// Reconstruit la liste des plugins activés en regardant les dossiers
        /// </summary>
        public static void RebuildActivePuginsCache()
        {

            var findAssemblies = GetActiveLogikedAssemblies();

            var names = findAssemblies.Select(m => Path.GetFileNameWithoutExtension(m)).ToArray();

            Instance.activeModulesName.Clear();

            for (int i = 0; i < names.Length; i++)
            {
                SetModuleActive(names[i], true, false);
            }

            ApplyChanges();
        }



        public static LogikedProjectConfig Instance
        {
            get
            {

                if (instance == null)
                {


                    if (!Directory.Exists(LogikedAbsoluteConfigFolder))
                        Directory.CreateDirectory(LogikedAbsoluteConfigFolder);


                    if (!File.Exists(Logiked_AssetsExtention.GetAbsolutePath(LogikedConfigPath)))
                    {
                        instance = (LogikedProjectConfig)CreateInstance(typeof(LogikedProjectConfig));
                        AssetDatabase.CreateAsset(instance, LogikedConfigPath);
                    }
                    else
                    {
                        instance = AssetDatabase.LoadAssetAtPath<LogikedProjectConfig>(LogikedConfigPath);
                        if (instance == null) Debug.LogError($"File at path {LogikedConfigPath} is corrupted. Check in project window.");
                    }
                }


                return instance;
            }
        }
        private static LogikedProjectConfig instance;



        public static void LoadRefreshConfig()
        {
            ApplyChanges();

        }


        /// <summary>
        ///Retourne le chemin relatif des logikedAssemblies actives
        /// </summary>
        public static string[] GetActiveLogikedAssemblies()
        {
            var findAssemblies = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset Logiked_");
            var paths = findAssemblies.Select(m => AssetDatabase.GUIDToAssetPath(m)).Where(m => m != null).Where(m => AssetDatabase.GetMainAssetTypeAtPath(m) == typeof(UnityEditorInternal.AssemblyDefinitionAsset)).ToArray();

            return paths;
        }





        /// <summary>
        /// Charge/Déchgarge les plugins en fonction de la liste ActiveModulesName
        /// </summary>
        static void ApplyChanges()
        {



            var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            //Debug.LogError(symbols);

            string symbolsModified = Regex.Replace(symbols, ";?USING_Logiked_[^;]+", "");
            string searchSymbol;

            //Debug.LogError(symbolsModified);

            var config = Instance;

            var assemblyList = GetActiveLogikedAssemblies();//On chope toute les assbs

            //Deja on autorise dans les buildsymbol les scripts
            foreach (var modules in config.activeModulesName)
            {

                searchSymbol = "USING_" + modules;



                if (!symbolsModified.Contains(modules))
                    symbolsModified += $";{searchSymbol}";

                try
                {
                    AssetDatabase.Refresh();
                }
                catch (Exception)
                {

                }
            }


            //Application
            if (symbolsModified != symbols)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbolsModified);
        }




        public static void SaveConfig()
        {
            var config = Instance;

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            LoadRefreshConfig();
        }



        public static void SetModuleActive(string folderName, bool active, bool autoSave = true)
        {
            var config = Instance;
            var mod = config.activeModulesName;
            var cont = mod.Contains(folderName);

            if (!cont && active)
                mod.Add(folderName);

            if (cont && !active)
                mod.Remove(folderName);

            if (autoSave)
                SaveConfig();
        }




        static void RemoveIfEmptyFolder(string path)
        {
            if (Directory.Exists(path) && Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count() == 0)
            {//ifmpty
                Directory.Delete(path);
            }
        }


    }
}


#endif
