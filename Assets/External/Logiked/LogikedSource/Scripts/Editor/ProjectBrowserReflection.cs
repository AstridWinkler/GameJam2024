#if UNITY_EDITOR

using logiked.source.extentions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace logiked.source.editor
{

    /// <summary>
    /// Interface vers les fonctions privées du ProjectBrowser Unity. Utile pour afficher des Assets/ Naviguer dans les dossiers
    /// </summary>
    public static class ProjectBrowserReflection
    {

        #region Fields & Properties

        /// <summary>
        /// Le type de fenetre "ProjectBrowser"
        /// </summary>
        private static Type ProjectBrowserType
        => projectBrowserType ?? (projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser"));

        /// <summary>
        /// La fenetre du project browser actif
        /// </summary>
        private static EditorWindow ProjectBrowser
        => projectBrowser ?? (projectBrowser = EditorWindow.GetWindow(projectBrowserType));


        private static Type projectBrowserType;
        private static EditorWindow projectBrowser;

        private static UnityEngine.Object m_lastAssetSelected = null;
        private static HashSet<int> m_lastAssetFolderExpanded = new HashSet<int>();

        private static string FolderdTreeview_Name => ProjectBrowser_IsTwoColumn() ? "m_FolderTreeState" : "m_AssetTreeState";
        //private static string ViewField_FileName => "m_ListArea";





        #endregion



        /// <summary>
        /// Set the unity Project windows search bar
        /// </summary>
        public static void SetUnitySearchBar(string search)
        {
            var pwin = projectBrowser;
            if (pwin == null) return;

            var ShowFolderContents = projectBrowserType.GetMethod("SetSearch", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new System.Type[] { typeof(string) }, null);
            ShowFolderContents.Invoke(pwin, new object[] { search });
        }






        /// <summary>
        /// Selectionne l'asset dans la Project window et developpe les répertoires concernés. <br></br>
        /// Referme les anciens répértoires ouverts avec cette fonction si il l'utilisateur ne s'en est pas servi. (Pratique)
        /// </summary>
        public static void SelectAssetInProjectWindow(this UnityEngine.Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);

            if (path == null)
            {
                Debug.LogWarning($"Object '{obj}' is not a project asset");
                return;
            }


            Object toSelect = obj;


            var savedWin = EditorWindow.focusedWindow;
            EditorUtility.FocusProjectWindow();


            //On récupere les dossiers étendus
            var originalExpandedFolders = ProjectBrowser_GetAllExpandedFolders();
            var setRemoveExpandedFolders = new List<int>();
            var alreadyOpenDir = new List<int>();

            setRemoveExpandedFolders.AddRange(originalExpandedFolders);


            //Si jamais des dossiés ont déja été Expand précédement et que la selection n'a pas changé
            if (m_lastAssetSelected != null && m_lastAssetSelected == Selection.activeObject && m_lastAssetFolderExpanded.Count > 0)
            {
                //On va refermer ces dossiers en les sortant de la liste
                setRemoveExpandedFolders = setRemoveExpandedFolders.Except(m_lastAssetFolderExpanded).ToList();

                m_lastAssetSelected = null;
                m_lastAssetFolderExpanded.Clear();
            }

            //Liste de tout les dossier ouverts
            alreadyOpenDir.AddRange(setRemoveExpandedFolders);


            int fold;


            //On verifie si l'objet est un dossier.  
            if (AssetDatabase.IsValidFolder(path))
            {
                fold = ProjectBrowser_GetFolderId(path);
                setRemoveExpandedFolders.Add(fold);//Dans ce cas on l'ajoute aux dossier à ouvrir
                m_lastAssetFolderExpanded.Add(fold);
            }



            //On verifie si le parent de l'objet est un dossier.  
            var parentFold = Path.GetDirectoryName(path);
            if (!parentFold.IsNullOrEmpty() && AssetDatabase.IsValidFolder(parentFold))
            {
                fold = ProjectBrowser_GetFolderId(parentFold);
                setRemoveExpandedFolders.Add(fold);//Dans ce cas on l'ajoute aux dossier à ouvrir
                m_lastAssetFolderExpanded.Add(fold);
            }


            //On ouvre les dossiers
            ProjectBrowser_SetAllExpandedFolders(setRemoveExpandedFolders);


            //On ping l'objet et on ke selectionne
            Selection.activeObject = toSelect;
            EditorGUIUtility.PingObject(toSelect);


            //Recup des nouveaux directories expands
            var newExpandedIds = ProjectBrowser_GetAllExpandedFolders();


            //On compare à ceux d'avant pour obtenir ceux qui ont été déployés pour reveler le fichier
            m_lastAssetSelected = toSelect;
            m_lastAssetFolderExpanded.UnionWith(newExpandedIds);
            m_lastAssetFolderExpanded.ExceptWith(alreadyOpenDir);
        }




        /// <summary>
        /// Verifie si le ProjectBrowser est en mode 2 colones. (Utile pour certaine opérations)
        /// </summary>
        /// <returns>Le mode 2 colones est actif</returns>
        private static bool ProjectBrowser_IsTwoColumn()
        {
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;
            if (projectBrowser == null) return false;
            var inst = projectBrowserType.GetMethod("IsTwoColumns", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return (bool)inst.Invoke(projectBrowser, new object[] { });
        }



        /// <summary>
        /// Récuperer l'ID associé à un path vers un dossier.
        /// </summary>
        /// <param name="path">Chemin vers le dossier</param>
        /// <returns>Id du dossier</returns>
        private static int ProjectBrowser_GetFolderId(string path)
        {
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;
            if (projectBrowser == null) return 0;

            var GetFolderInstanceID_method = projectBrowserType.GetMethod("GetFolderInstanceID", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            int res = (int)GetFolderInstanceID_method.Invoke(projectBrowser, new object[1] { path });

            return res;
        }



        /// <summary>
        /// Récuperer les paths vers les éléments associé à leur instanceIds dans le ProjectBrowser
        /// </summary>
        /// <param name="path">InstanceIds des assets</param>
        /// <returns>Chemin vers les assets</returns>
        private static string[] ProjectBrowser_GetFolderPathsFromInstanceIDs(int[] path)
        {
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;
            if (projectBrowser == null) return new string[0];

            var GetFolderInstanceID_method = projectBrowserType.GetMethod("GetFolderPathsFromInstanceIDs", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            string[] res = (string[])GetFolderInstanceID_method.Invoke(projectBrowser, new object[1] {  path  });

            return res;
        }





        /// <summary>
        /// Permet d'afficher les objets dans la ProjectWindow comme résultat de rechecrhe
        /// </summary>
        /// <param name="objs"></param>

        public static void ShowAssetsInProjectWindows(Object[] objs, string researchName = "search_result")
        {
            List<int> objs_ids = new List<int>();

            if (objs == null || objs.Length == 0) return;



            objs_ids.AddRange(objs.Where(m => m != null).Select(o => o.GetInstanceID()));



            Debug.Log(string.Join(", ", objs_ids));

            ShowAssetsInProjectWindows(objs_ids.ToArray(), researchName);
        }


        /// <summary>
        /// Affiche dans le ProjectBrowser les assets qui correspondent aux InstanceIds
        /// </summary>
        /// <param name="instanceIds">Liste des assets à afficher</param>
        /// <param name="researchName">Nom design pour la bare de recherche</param>
        public static void ShowAssetsInProjectWindows(int[] instanceIds, string researchName = "search_result")
        {

            if (instanceIds == null || instanceIds.Length == 0) return;

            var paths = ProjectBrowser_GetFolderPathsFromInstanceIDs(instanceIds);
            var sorted = new List<int>();

            //On récupère seulement les InstanceIds publiques et cohérentes. 
            //Le dossier "Assets", ou le contenu du dossier "Library" ne doivent pas étre récupérès.
            for (int i = 0; i < instanceIds.Length; i++)
            {
                if (paths[i].IsNullOrEmpty() || !paths[i].Contains('/') || paths[i].StartsWith("Library/")) continue;
                sorted.Add(instanceIds[i]);
            } 
                   
            ProjectBrowser_ShowObjectsInList(sorted.ToArray(), researchName);
        }




        /// <summary>
        /// Affiche dans le ProjectBrowser les assets qui correspondent aux InstanceIds
        /// </summary>
        private static void ProjectBrowser_ShowObjectsInList(int[] toShow, string researchName = "search_result")
        {
            //Faire en sorte de ne pas avoir de caracteres bizzares dans la searchbar
            //researchName = researchName.Replace(" ", "_").Replace(":", "");

            //Recup des datas
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;
            if (projectBrowser == null) return;

            //On reset les champs
            var resetView = projectBrowserType.GetMethod("ResetViews", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            resetView.Invoke(projectBrowser, new object[] { });

            //On met du texte dans la searchBar histoire d'avoir un truc design,
            //mais surtout pour faire comprendre à la projectWindow OneColumn qu'il faut afficher les fichiers
            var m_SearchFieldText = projectBrowserType.GetField("m_SearchFieldText", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            m_SearchFieldText.SetValue(projectBrowser, researchName);

            //Dans la continuité, on dit à la fenettre que la recherche de la searchBar est terminée
            var InitSearchMenu = projectBrowserType.GetMethod("UpdateSearchDelayed", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            InitSearchMenu.Invoke(projectBrowser, new object[] { });

            //On attrape la variable qui liste les fichiers en icones
            var m_ListArea_field = projectBrowserType.GetField("m_ListArea", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var m_ListArea = m_ListArea_field.GetValue(projectBrowser);

            //On assigne dedans la liste de tout les fichiers que l'ont veut afficher
            var ShowObjectsInList_method = m_ListArea_field.FieldType.GetMethod("ShowObjectsInList", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, default(Binder), new Type[] { typeof(int[]) }, null);
            ShowObjectsInList_method.Invoke(m_ListArea, new object[] { toShow });

        }



        /// <summary>
        /// Retourne une liste de tous les dossiers dévéloppés dans le ProjectBrowser
        /// </summary>
        /// <returns>Ids des dossiers</returns>
        private static List<int> ProjectBrowser_GetAllExpandedFolders()
        {

            //Recup des datas
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;

            if (projectBrowser == null) return new List<int>();

            //On choppe la variable datas du treeview qui gère les dossiers 
            var m_FolderTreeState_field = projectBrowserType.GetField(FolderdTreeview_Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var m_FolderTreeState = m_FolderTreeState_field.GetValue(projectBrowser);


            //On récup la liste des dossier "expanded" dans la treeview
            var expandedIDs_field = m_FolderTreeState.GetType().GetProperty("expandedIDs", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var expandedIDs = (List<int>)expandedIDs_field.GetValue(m_FolderTreeState);

            return expandedIDs;
        }



        /// <summary>
        /// Permet de set l'intégralité des dossiers ouverts dans le ProjectBrowser
        /// </summary>
        /// <param name="values">Les InstanceId des dossiers ouverts</param>
        private static void ProjectBrowser_SetAllExpandedFolders(List<int> values)
        {
            //Recup des datas
            var projectBrowserType = ProjectBrowserType;
            var projectBrowser = ProjectBrowser;
            if (projectBrowser == null) return;

            values.Sort();
            values = values.Distinct().ToList();

            //Reflection!

            if (ProjectBrowser_IsTwoColumn())
            {
                var inst = projectBrowserType.GetMethod("SelectAssetsFolder", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                inst.Invoke(projectBrowser, new object[] { });
            }

            //ON choppe la variable du treeview qui gère les dossiers
            var m_FolderTreeState_field = projectBrowserType.GetField(FolderdTreeview_Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var m_FolderTreeState = m_FolderTreeState_field.GetValue(projectBrowser);

            //On récup la varaible des dossier "expanded" dans la treeview
            var expandedIDs_field = m_FolderTreeState.GetType().GetProperty("expandedIDs", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            expandedIDs_field.SetValue(m_FolderTreeState, values);

            //On les réassignes.
            var f = projectBrowserType.GetMethod("ResetViews", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            f.Invoke(projectBrowser, new object[] { });

            var EnsureValidFolders_method = ProjectBrowserType.GetMethod("EnsureValidFolders", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            EnsureValidFolders_method.Invoke(projectBrowser, new object[] { });

            var RefreshSelectedPath_method = ProjectBrowserType.GetMethod("RefreshSelectedPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            RefreshSelectedPath_method.Invoke(projectBrowser, new object[] { });

        }


        /// <summary>
        /// Selectionne l'asset dans le repertoire et l'affiche dans le ProjectBrowser
        /// </summary>
        /// <param name="path">Chemin du fichier ou dossier</param>
        public static void SelectObjectInUnityInspector(string path)
        {
            SelectAssetInProjectWindow(AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)));
        }




        /// <summary>
        /// Retourne le GUID de l'objet
        /// </summary>
        /// <param name="obj">Objet à checker</param>
        /// <returns>Le GUID</returns>
        public static string GetGUID(this UnityEngine.Object obj)
        {
            string guid;
            long file;

            if (obj == null)
                return null;

            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out guid, out file))
                return guid;

            Debug.LogError("Unable to get the GUID for " + obj.name);
            return "";
        }


    }


}


#endif