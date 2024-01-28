#if UNITY_EDITOR


using logiked.source.editor;
using logiked.source.extentions;
using logiked.source.types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace logiked.editor.navigation
{

    /// <summary>
    /// Extension for the unity Project window, with new search tool,shortcuts..
    /// </summary>
    [InitializeOnLoad]
    public class Logiked_ProjectBrowserExtention
    {



        private static Stack<Object> assetSearchHistory = new Stack<Object>();
        private static string[] searchingTypes = { "Texture", "Material", "Script", "ScriptableObject", "Model", "Prefab", "asmdef" };//No sprites

        private const string context_showElementLocation = "Navigate to element";
        private const string context_showElementLocation_shortcut = " %W";



        private const string context_searchLinkedLabels = "Search assets with same label";
        private const string context_searchLinkedLabels_shortcut = " %&SPACE";


        private const string context_searchConnectedAssets = "Search connected assets";
        private const string context_searchConnectedAssets_shortcut = " %SPACE";


        private const string context_RevertSearch = "Revert search";
        private const string context_RevertSearch_shortcut = " %Q";

        // private const string context_ShowNavigationContextMenu = "Show Navigation Utils %#SPACE";
        private const string context_ShowNavigationContextMenu = "<Shortcut Popup> %#SPACE";

        private const string LogikedSourceAssetMenu = LogikedPlugin<ScriptableObject>.MenuItemName;

        private const string context_ClearUnusedAssets = "Clear Unused Assets";



        [MenuItem("Logiked/Project/[TEST]" + context_ClearUnusedAssets)]
        public static void ClearAssets()
        {
            Resources.UnloadUnusedAssets();
            EditorUtility.UnloadUnusedAssetsImmediate();
            System.GC.Collect();
        }



        /// <summary>
        /// Search in Project window all items with the selected item labels.
        /// </summary>
        [MenuItem("Logiked/Project/" + context_searchLinkedLabels)]
        [MenuItem(LogikedSourceAssetMenu + context_searchLinkedLabels + context_searchLinkedLabels_shortcut,  priority = LogikedProjectConfig.LogikedMenuItemPriority)]
        private static void FindAssetWithSameLabel()
        {


            if (Application.isPlaying)//EditorWindow.focusedWindow != projectBrowser)
                return;

            if (Event.current != null && Event.current.modifiers.HasFlag(EventModifiers.Control)) return;



            var objs = Selection.objects;
            if (objs.Length == 0)
            {
                Debug.LogError("Nothing selected.");
                return;
            }

            if (assetSearchHistory.Count == 0 || assetSearchHistory.Peek() !=  objs[0])
                assetSearchHistory.Push(objs[0]);
        
            HashSet<string> labs = new HashSet<string>();


            for (int i = 0; i < objs.Length; i++)
            {
                var lab = AssetDatabase.GetLabels(objs[i]);
                foreach (var a in lab) labs.Add(a);
            }

            string end = ((labs.Count > 0) ? "l:" + string.Join(" l:", labs) : "") + " t:" + string.Join(" t:", searchingTypes);
            ProjectBrowserReflection.SetUnitySearchBar(end);
            // EditorGUIUtility.PingObject(Selection.activeObject);
            //t: Material t:Script t:ScriptableObject t:Model t:Prefab l:t: Texture
        }

        /// <summary>
        /// Clear the project window searchbar
        /// </summary>
        [MenuItem("Logiked/Project/" + context_showElementLocation)]
        [MenuItem(LogikedSourceAssetMenu + context_showElementLocation + context_showElementLocation_shortcut, priority = LogikedProjectConfig.LogikedMenuItemPriority)]
        private static void ClearSearchbar()
        {
            if (Application.isPlaying)
                return;

            // SetUnitySearchBar("");
            EditorGUIUtility.PingObject(Selection.activeObject);
            assetSearchHistory.Clear();

        }

        /// <summary>
        /// Back to the first selected object.
        /// </summary>
        [MenuItem("Logiked/Project/" + context_RevertSearch)]
        [MenuItem(LogikedSourceAssetMenu + context_RevertSearch + context_RevertSearch_shortcut, priority = LogikedProjectConfig.LogikedMenuItemPriority)]
        private static void BackSearchbar()
        {
            //SetUnitySearchBar("");


            if (assetSearchHistory.Count == 0) return;

            var obj = assetSearchHistory.Pop();
            EditorGUIUtility.PingObject(obj);
            if (obj == null) return;
            Selection.objects = new Object[] { obj };//Restaurer la séléction du début de la recherche
        }


        private static void Log(string message, DebugC.ErrorLevel error = DebugC.ErrorLevel.Log)
        {
            DebugC.Log(message, Color.yellow + Color.red/2f , "ProjectBrowser" ,  errorLevel: error);
        }


        /// <summary>
        /// Search in Project window all items with the selected item labels.
        /// </summary>
        [MenuItem("Logiked/Project/"+ context_searchConnectedAssets)]
        [MenuItem(LogikedSourceAssetMenu + context_searchConnectedAssets + context_searchConnectedAssets_shortcut, priority = LogikedProjectConfig.LogikedMenuItemPriority)]
        private static void SearchAllonnectedAssets()
        {


            var objs = Selection.objects;
            if (objs.Length == 0 || objs[0] == null)
            {
                Log("Nothing selected.");
                return;
            }

            
            List<Object> linkedAssets = new List<Object>();

            for (int i = 0; i < objs.Length; i++)
            {
                linkedAssets.AddRange(AssetNavigationUtility.GetAllConnectedObjects(objs[i]));
            }


            //Ajout personnel pour la visibilité : charge que les assets principaux (Genre pas les 35 sprites de la spritesheet) (Peutetre faire 2 modes)
            linkedAssets = linkedAssets.Select(m => AssetDatabase.IsMainAsset(m) ? m : AssetDatabase.LoadAssetAtPath(m.GetAssetPath(Logiked_AssetsExtention.PathFormat.AssetRelative), typeof(UnityEngine.Object)  )).ToList();
            linkedAssets.Distinct();



            if (linkedAssets.Count > objs.Length)
            {

                //On enregistre pour le Ctrl+Q
                if (assetSearchHistory.Count == 0 || assetSearchHistory.Peek() != Selection.activeObject)//Anti doublon
                    assetSearchHistory.Push(Selection.activeObject);

                ProjectBrowserReflection.ShowAssetsInProjectWindows(linkedAssets.ToArray(), "Linked Objects");
            }
            else
                Log($"No linked object found for objects [{ string.Join(", ", objs.Select(m => m.name)) }]");

        }


        /// <summary>
        /// Fonction de recherche sur l'asset selectionné
        /// </summary>
        [MenuItem(LogikedSourceAssetMenu + context_ShowNavigationContextMenu,  priority = LogikedProjectConfig.LogikedMenuItemPriority)]
        private static void ShowNavMenu()
        {
            GenericMenu menu = new GenericMenu();

            var path = Selection.activeObject.GetAssetPath();

            if (Selection.activeObject == null || path.IsNullOrEmpty())
            {
                return;
            }


            menu.AddItem(new GUIContent(context_searchConnectedAssets), () => SearchAllonnectedAssets());
            menu.AddItem(new GUIContent(context_searchLinkedLabels), () => FindAssetWithSameLabel());

            menu.AddSeparator("");

            menu.AddItem(new GUIContent(context_showElementLocation), () => ClearSearchbar());
            menu.AddItem(new GUIContent(context_RevertSearch), () => BackSearchbar(), assetSearchHistory.Count == 0);


            menu.ShowAsContext();
        }







    }


}
#endif