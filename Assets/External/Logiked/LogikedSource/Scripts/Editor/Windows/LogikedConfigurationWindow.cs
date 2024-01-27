#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditorInternal;
using System.Linq;
using UnityEditor.SceneManagement;
using logiked.source.extentions;
using logiked.source.utilities;
using Object = UnityEngine.Object;
using System.Collections;

#if JsonTest1
using Newtonsoft.Json.Linq;
#else
using Unity.Plastic.Newtonsoft.Json.Linq;
#endif


namespace logiked.source.editor
{
    [Serializable]
    public class LogikedConfigurationWindow : EditorWindow
    {

        #region Field & Props

        private Vector2 scrollView;

        private Color colorGrey = new Color(0.93f, 0.93f, 0.93f);
        //Color colorGrey2 = new Color(0.8f, 0.8f, 0.8f);
        private Color colorDefault;

        [SerializeField]
        private List<KeyValuePair<string, string>> navHistory;

        [SerializeField]
        private bool[] pluginEditOpen;

        private List<string> logikedAssembliesPath = new List<string>();

        private bool isDragingFiles;


        //Liste des asb files et de leur DEFINES nescessaires 
        Dictionary<string, Tuple<AssemblyDefinitionAsset, List<string>>> asbDictionary = new Dictionary<string, Tuple<AssemblyDefinitionAsset, List<string>>>();


#if History_prototype
        Object activeObj;

        [SerializeField]
        private int selectHistory = 0;

        bool inSelection;
#endif


        private List<DrawSectionMethod> menuList = new List<DrawSectionMethod>();

        public LogikedProjectConfig projectConfig => LogikedProjectConfig.Instance;

        #endregion

        #region Class

        [Serializable]
        private class DrawSectionMethod
        {
            internal string name;
            [SerializeField]
            internal bool isOpen = false;
            [SerializeField]
            internal Action contextAction;

            public DrawSectionMethod(string name, bool isOpen, Action contextAction)
            {
                this.name = name;
                this.isOpen = isOpen;
                this.contextAction = contextAction;
            }
        }

        private class StyleList
        {


            private static GUIStyle stl_buttonTxt;
            private static GUIStyle stl_horizontalBox_greyTr;
            private static GUIStyle txt_navHistory;
            private static GUIStyle stl_whiteText;
            private static GUIStyle stl_blue;
            private static GUIStyle stl_red;


            public static GUIStyle Stl_buttonTxt
            {
                get
                {
                    if (stl_buttonTxt == null)
                    {
                        stl_buttonTxt = new GUIStyle(EditorStyles.label);
                        stl_buttonTxt.normal.background = null;
                        stl_buttonTxt.richText = true;
                        stl_buttonTxt.alignment = TextAnchor.MiddleLeft;
                        stl_buttonTxt.margin = new RectOffset(0, 0, 0, 0);
                        stl_buttonTxt.padding = new RectOffset(0, 0, 0, 0);
                    }
                    return stl_buttonTxt;
                }
            }


            public static GUIStyle Stl_horizontalBox_greyTr
            {
                get
                {
                    if (stl_horizontalBox_greyTr == null)
                    {
                        stl_horizontalBox_greyTr = new GUIStyle();
                        stl_horizontalBox_greyTr.margin = new RectOffset(0, 0, 0, 0);
                        stl_horizontalBox_greyTr.padding = new RectOffset(0, 0, 0, 0);
                        stl_horizontalBox_greyTr.alignment = TextAnchor.MiddleLeft;
                        stl_horizontalBox_greyTr.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);

                    }
                    return stl_horizontalBox_greyTr;
                }
            }




            public static GUIStyle Txt_navHistory
            {
                get
                {
                    if (txt_navHistory == null)
                    {
                        txt_navHistory = new GUIStyle();
                        txt_navHistory.alignment = TextAnchor.MiddleRight;
                        txt_navHistory.normal.background = null;
                        txt_navHistory.margin = new RectOffset(0, 0, 2, 2);
                        txt_navHistory.padding = new RectOffset(0, 0, 0, 0);
                        txt_navHistory.richText = true;
                    }
                    return txt_navHistory;
                }
            }


            public static GUIStyle Stl_whiteText
            {
                get
                {
                    if (stl_whiteText == null)
                    {
                        stl_whiteText = new GUIStyle();
                        stl_whiteText.normal.textColor = Color.white;
                        stl_whiteText.alignment = TextAnchor.MiddleLeft;
                    }
                    return stl_whiteText;
                }
            }


            public static GUIStyle Stl_blue
            {
                get
                {
                    if (stl_blue == null)
                    {
                        stl_blue = new GUIStyle();
                        stl_blue.normal.background = null;
                        stl_blue.richText = true;
                        stl_blue.alignment = TextAnchor.MiddleLeft;
                        stl_blue.margin = new RectOffset(0, 0, 0, 0);
                        stl_blue.padding = new RectOffset(0, 0, 0, 0);
                        stl_blue.normal.textColor = Color.cyan;

                    }
                    return stl_blue;
                }
            }


            public static GUIStyle Stl_red
            {
                get
                {
                    if (stl_red == null)
                    {
                        stl_red = new GUIStyle(Stl_blue);
                        stl_red.normal.textColor = Color.red;
                    }
                    return stl_red;
                }
            }





        }

        private static class NavigationSectionSettings
        {
            public static Vector2 workspaceScrollView;
            public static bool isEditingWorkspace;
        }


        #endregion

        #region Core Methods


        [MenuItem("Window/Logiked Configuration Window")]
        [MenuItem("Logiked/Project Configuration Window", priority = 1000)]
        private static void OpenWindow()
        {
            LogikedConfigurationWindow window = GetWindow<LogikedConfigurationWindow>();
            window.titleContent = new GUIContent("Logiked configuration");
            window.Show();
        }


        private void OnFocus()
        {
            InitVars();
        }




        void InitVars()
        {
            //Debug.Log("helper:init");

            navHistory = new List<KeyValuePair<string, string>>();


            colorDefault = GUI.color;

            //Preservation des sections fenettres ouvertes

            var listSections = new List<DrawSectionMethod>();
            listSections.Add(new DrawSectionMethod("Navigation", false, DrawNavCtx));
            listSections.Add(new DrawSectionMethod("Logiked", false, DrawLogikedSettings));
            listSections.Add(new DrawSectionMethod("Scenes", false, DrawScenes));
            listSections.Add(new DrawSectionMethod("Globals", false, DrawGlobals));

#if History_prototype
                selectHistory = 0;
            asbDictionary.Clear();
            listSections.Add(new ContextBox("History", false, DrawHistoryCtx));
#endif



            if (menuList == null || menuList.Count != listSections.Count)
            {
                menuList = listSections;
            }
            else
            {
                for (int i = 0; i < menuList.Count; i++)
                    listSections[i].isOpen = menuList[i].isOpen;
                menuList = listSections;
            }

            //Search logiked plugins
            logikedAssembliesPath.Clear();
#if USING_PACKAGE_MANAGER
            logikedAssembliesPath = LogikedProjectConfig.GetActiveLogikedAssemblies().ToList();
#else
            logikedAssembliesFolderPath = LogikedProjectConfig.GetAllPluginPath().ToList();
#endif
            if (pluginEditOpen == null || pluginEditOpen.Length != logikedAssembliesPath.Count)
            {
                pluginEditOpen = new bool[logikedAssembliesPath.Count];
            }
        }




        private void OnInspectorUpdate()
        {
            Repaint();
        }



        private void OnGUI()
        {

            BeginWindows();

            scrollView = GUILayout.BeginScrollView(scrollView);


            foreach (var h in menuList)
                DrawCtxBox(h);


            GUILayout.EndScrollView();

            EndWindows();
        }


        private void DrawCtxBox(DrawSectionMethod box)
        {


            GUILayout.BeginVertical("box");
            if (GUILayout.Button((box.isOpen ? "▼ " : "► ") + box.name, StyleList.Stl_buttonTxt, GUILayout.MaxHeight(18), GUILayout.MaxWidth(position.width)))
            {
                box.isOpen = !box.isOpen;
            }

            if (box.isOpen)
            {
                box.contextAction.DynamicInvoke();
            }
            GUILayout.EndVertical();
        }

        #endregion

        #region Draw Section Methods


        private void DrawNavCtx()
        {

            GUI.color = colorGrey;
            GUILayout.BeginVertical();
            GUI.color = colorDefault;


            GUILayout.BeginHorizontal();

            NavCtx_DrawWorkspaceList();


            GUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            //  NavigationSectionSettings.workspaceScrollView = GUILayout.BeginScrollView(NavigationSectionSettings.workspaceScrollView, GUILayout.ExpandHeight(false));

            NavCtx_DrawCurrentWorkspace();


            // GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndVertical();

            CheckDragAndDrop();



            GUILayout.EndHorizontal();


            void CheckDragAndDrop()
            {

                if (projectConfig.CurrentWorkspace == null)
                    return;


                Rect boxPos = GUILayoutUtility.GetLastRect();
                bool mouseInsideDropZone = boxPos.Contains(Event.current.mousePosition);


                if (isDragingFiles)//Text indicator
                {

                    boxPos = GUILayoutUtility.GetLastRect();

                    if (mouseInsideDropZone)
                        GUI.color = Color.white - new Color(0, 0, 0, 0.3f);

                    GUI.Box(boxPos, "Add File", GUILogiked.Styles.Box_DragAndDrop);
                    GUI.color = Color.white;
                }


                //DragAndDrop.visualMode = DragAndDropVisualMode.None

                if (Event.current.type == EventType.DragUpdated)
                {
                    isDragingFiles = true;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (mouseInsideDropZone)
                        Event.current.Use();
                }
                else if (Event.current.type == EventType.DragExited)
                {
                    isDragingFiles = false;
                }
                else if (Event.current.type == EventType.DragPerform)
                {

                    if (!mouseInsideDropZone) return;

                    // To consume drag data.
                    DragAndDrop.AcceptDrag();


                    // Unity Assets including folder.
                    if (DragAndDrop.paths.Length == DragAndDrop.objectReferences.Length)
                    {
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            Object obj = DragAndDrop.objectReferences[i];
                            projectConfig.CurrentWorkspace.files.Add(obj);
                        }

                        projectConfig.SetDirtyNow();
                    }
                }
            }
        }
        private void NavCtx_DrawWorkspaceList()
        {
            GUILayout.BeginVertical(GUILogiked.Styles.Box_Border, GUILayout.Width(position.width / 3f));

            GUILayout.Label("Workspaces", GUILogiked.Styles.Text_BoldCentered, GUILayout.MaxWidth(position.width / 3f));

            var workspaces = LogikedProjectConfig.Instance.Workspaces;



            for (int i = 0; i < workspaces.Length; i++)
            {

                GUI.enabled = LogikedProjectConfig.Instance.selectedWorkspace != i;
                if (GUILayout.Button(workspaces[i].name, GUILayout.MaxWidth(position.width / 3f)))
                {
                    LogikedProjectConfig.Instance.selectedWorkspace = i;
                    NavigationSectionSettings.isEditingWorkspace = false;
                    GUI.FocusControl(null);

                }
                GUI.enabled = true;
            }




            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                {
                    GUILogiked.Panels.GUIDrawEditorIcon(CreateNewContextMenu, GUILogiked.Panels.EditorIconType.AddItem);
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();



            GUILayout.EndVertical();


            void CreateNewContextMenu()

            {

                LogikedProjectConfig.WorkSpaceFrame newWorkspace = new LogikedProjectConfig.WorkSpaceFrame();
                newWorkspace.name = "New Group";
                LogikedProjectConfig.Instance.Workspaces = LogikedProjectConfig.Instance.Workspaces.Append(newWorkspace).ToArray();
                EditorUtility.SetDirty(LogikedProjectConfig.Instance);
                GUI.FocusControl(null);

            }
        }
        private void NavCtx_DrawCurrentWorkspace()
        {

            var workspaces = LogikedProjectConfig.Instance.Workspaces;
            workspaces = LogikedProjectConfig.Instance.Workspaces;

            LogikedProjectConfig.WorkSpaceFrame currentWordkspace;

            currentWordkspace = projectConfig.CurrentWorkspace;

            if (currentWordkspace == null)
                return;



            GUILayout.BeginHorizontal();//Workspace First Line
            {
                GUILayout.Space(25);

                if (!NavigationSectionSettings.isEditingWorkspace)
                    GUILayout.Label(currentWordkspace.name, GUILogiked.Styles.Text_BoldCentered);
                else
                    currentWordkspace.name = GUILayout.TextField(currentWordkspace.name);

                GUILogiked.Panels.GUIDrawEditorIcon(ModifyWorkspace, NavigationSectionSettings.isEditingWorkspace ? GUILogiked.Panels.EditorIconType.ConfirmChecked : GUILogiked.Panels.EditorIconType.EditPen);

                if (NavigationSectionSettings.isEditingWorkspace)
                    GUILogiked.Panels.GUIDrawEditorIcon(RemoveCurrentWorkspace, GUILogiked.Panels.EditorIconType.RemoveCross, "Remove workspace");
            }
            GUILayout.EndHorizontal();


            //Conten Listing

            var files = currentWordkspace.files;
            files.RemoveAll(m => m == null);

            if (files.Count > 0)
            {

                for (int i = 0; i < files.Count; i++)
                {
                    DrawFile(files[i], i);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Drag & Drop files here", MessageType.Info, true);
            }



            #region Internal Funcs


            void DrawFile(Object asset, int index)
            {
                GenericMenu menu = new GenericMenu();

                var files = projectConfig.Workspaces[projectConfig.selectedWorkspace].files;


                void CallBackButton(IList files)
                {
                    projectConfig.Workspaces[projectConfig.selectedWorkspace].files = files as List<Object>;
                    projectConfig.SetDirtyNow();
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("", GUILogiked.Styles.Button_NoBackgroundButton, GUILayout.Height(20)))
                {
                    asset.SelectAssetInProjectWindow();
                }

                if (NavigationSectionSettings.isEditingWorkspace)
                    GUILogiked.Panels.DrawArrayElementContextButton(files, index, m => CallBackButton(m));

                GUILayout.EndHorizontal();

                Rect labPos = GUILayoutUtility.GetLastRect();

                labPos.x += 18;
                labPos.width -= 18;
                GUI.Label(labPos, asset.name);

                var path = AssetDatabase.GetAssetPath(asset);
                var icon = AssetDatabase.GetCachedIcon(path);

                Rect dest = new Rect();

                //dest = GUILayoutUtility.GetRect(20,20, GUILayout.ExpandWidth(false));
                dest = GUILayoutUtility.GetLastRect();
                dest.width = 20f;
                dest.height = 20f;

                GUI.DrawTexture(dest, icon);
            }


            void RemoveCurrentWorkspace()
            {
                if (LogikedWindowsAlert.Message_Box($"Supprimer \"{currentWordkspace.name}\" ?", "Confirmation", LogikedWindowsAlert.WindowsAlertType.YesNo) == LogikedWindowsAlert.WindowsAlertResult.YES)
                {
                    var lst = LogikedProjectConfig.Instance.Workspaces.ToList();
                    lst.RemoveAt(LogikedProjectConfig.Instance.selectedWorkspace);
                    LogikedProjectConfig.Instance.selectedWorkspace = -1;
                    LogikedProjectConfig.Instance.Workspaces = lst.ToArray();
                    NavigationSectionSettings.isEditingWorkspace = false;
                    EditorUtility.SetDirty(LogikedProjectConfig.Instance);
                    GUI.FocusControl(null);
                    GUIUtility.ExitGUI();
                }
            }


            void ModifyWorkspace()
            {
                NavigationSectionSettings.isEditingWorkspace = !NavigationSectionSettings.isEditingWorkspace;
                GUI.FocusControl(null);

            }

            #endregion

        }


        private void DrawLogikedSettings()
        {


            GUI.color = colorGrey;
            GUILayout.BeginVertical("box");
            GUI.color = colorDefault;
            GUILayout.Label("Active plugins");

            bool  errorAssembly;
            AssemblyDefinitionAsset asbAsset;
            var requieredAsb = new List<string>();


            var targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);


            ///List of active assembly
            for (int i = 0; i < logikedAssembliesPath.Count; i++)
            {
                var pluginPath = logikedAssembliesPath[i];
                var pluginAsbName = Path.GetFileNameWithoutExtension(logikedAssembliesPath[i]);
                asbAsset = null;


                errorAssembly = false;

                GUILayout.BeginHorizontal();



                if (!asbDictionary.ContainsKey(pluginPath))//Recuperation de l'asb Asset
                {
                    asbAsset = AssetDatabase.LoadMainAssetAtPath(pluginPath) as AssemblyDefinitionAsset;

                    string text = asbAsset.text;
                    JObject o = JObject.Parse(text);
                    var constraints = o.GetValue("defineConstraints");
                    requieredAsb = new List<string>();

                    foreach (var token in constraints)//Liste des contraintes de définition
                    {
                        string add = token.ToString();

                        if (add.Contains(pluginAsbName)) continue;

                        requieredAsb.Add(add.Replace("USING_", ""));
                    }

                    asbDictionary.Add(pluginPath, new Tuple<AssemblyDefinitionAsset, List<string>>(asbAsset, requieredAsb));

                }
                else
                {
                    errorAssembly = !asbDictionary.ContainsKey(pluginPath);
                    asbAsset = asbDictionary[pluginPath].Item1;
                    requieredAsb = asbDictionary[pluginPath].Item2;




                    foreach (var a in requieredAsb)
                    {
                        if (!symbols.Contains(a))
                        {
                            errorAssembly = true;
                            requieredAsb = requieredAsb.Where((m) => !symbols.Contains(m)).ToList();
                            break;
                        }
                    }

                    //asbAsset.

                }



                if (!errorAssembly)
                    GUILogiked.Panels.GUIDrawEditorIcon(() => asbAsset.SelectAssetInProjectWindow(), GUILogiked.Panels.EditorIconType.Folder);
                else
                    GUILogiked.Panels.GUIDrawEditorIcon(() => ProjectBrowserReflection.SelectObjectInUnityInspector(pluginPath), GUILogiked.Panels.EditorIconType.Folder);
                GUILogiked.Panels.GUIDrawEditorIcon(() => pluginEditOpen[i] = !pluginEditOpen[i]);



                GUIContent ct;


                if (errorAssembly)
                {
                    ct = new GUIContent($"{pluginAsbName}", $"Dépendances manquantes :\n{string.Join(", ", requieredAsb) }");
                    GUILayout.Label(ct, StyleList.Stl_red, GUILayout.MaxHeight(18));
                }
                else
                {
                    ct = new GUIContent($"{pluginAsbName}", $"{requieredAsb.Count} dépendances :\n{string.Join(",\n", requieredAsb) }");
                    GUILayout.Label(ct, StyleList.Stl_blue, GUILayout.MaxHeight(18));
                }




                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();



                //If plugin active, draw its params

                if (pluginEditOpen[i])
                {
                    DrawPluginParams(pluginAsbName);
                }

                GUILayout.Space(3);


                //PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, symbols);
            }
            GUILayout.EndVertical();
        }

        private void DrawPluginParams(string pluginFolderName)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical("box");


            ScriptableObject settings = null;
            // string path = AssetDatabase.GetAssetPath(logikedPlugin);
            // path = System.IO.Path.GetDirectoryName(path);
            string path = "Assets/Resources/Logiked";




            //Search setting file par le labbel
            var files = AssetDatabase.FindAssets("l:" + pluginFolderName.Replace("Logiked", ""), new string[] { path });

            // foreach (var f in files) { Debug.LogError(AssetDatabase.GUIDToAssetPath(f)); }

            if (files.Length > 0)
            {
                settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(files[0]));
            }

            if (settings == null)
            {
                EditorGUILayout.HelpBox("Nothing to show", MessageType.Info, true);
                //  GUILayout.Label("No plugin settings founded.");
            }
            else
            {
                Editor editor = Editor.CreateEditor(settings);
                editor.DrawDefaultInspector();

                // LogikedPluginSettings.DrawDefaultInspector();

                /*
                var fields = settings.GetType().GetFields();
                for (int j = 0; j < fields.Length; j++)
                {
                    object val = fields[j].GetValue(settings);

                    if (val is int)
                        val = EditorGUILayout.IntField(fields[j].Name, (int)val);
                    else if (val is string)
                        val = EditorGUILayout.TextField(fields[j].Name, (string)val);
                    else if (val is float)
                        val = EditorGUILayout.FloatField(fields[j].Name, (float)val);
                    else
                        GUILayout.Label("Unkown paramtype : "+ fields[j].Name);
                }*/

            }





            GUILayout.EndVertical();
            GUILayout.EndHorizontal();


        }

        private void DrawScenes()
        {

            GUI.color = colorGrey;
            GUILayout.BeginVertical("box");
            GUI.color = colorDefault;


            var p = LogikedProjectConfig.Instance;
            var ss = p.FrequentlyUsedScenes;



            int x = 0;

            foreach (var s in ss)
            {
                if (s == null) continue;
                x++;
                if (GUILayout.Button(new GUIContent($"  {s.name}", GUILogiked.Panels.GetIconContent(GUILogiked.Panels.EditorIconType.UnityIcon).image), GUILayout.Height(30)))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(s.GetAssetPath());
                }
            }

            if (x == 0)
                EditorGUILayout.HelpBox("Aucune scènes configurée pour la navigation rapide.", MessageType.Info, true);


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILogiked.Panels.GUIDrawEditorIcon(() => p.SelectAssetInProjectWindow());
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }

        private void DrawGlobals()
        {
            /*
            GUI.color = colorGrey;
            GUILayout.BeginVertical("box");
            GUI.color = colorDefault;
            GUILayout.EndVertical();
            */


            var p = LogikedProjectConfig.Instance;
            var vars = p.ReflectionGlobalVariable;
            int x = 0;

            bool modif = false;


            GUILayout.BeginVertical(GUILogiked.Styles.Box_Border);


            GUILayout.Label("Config", GUILogiked.Styles.Text_BoldCentered);



            GUILayout.BeginHorizontal();


            GUI.enabled = p.globalMode == LogikedProjectConfig.GlobalWindowConfig.Build;
            if (GUILayout.Button("Editor"))
            {
                p.EditorGlobals_ChangeMode(LogikedProjectConfig.GlobalWindowConfig.Editor);
                modif = true;
            }


            GUI.enabled = p.globalMode == LogikedProjectConfig.GlobalWindowConfig.Editor;
            if (GUILayout.Button("Build"))
            {
                p.EditorGlobals_ChangeMode(LogikedProjectConfig.GlobalWindowConfig.Build);
                modif = true;
            }
            GUI.enabled = true;


            if (modif)
            {
                EditorUtility.SetDirty(p);
                GUI.FocusControl(null);
            }


            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUI.color = colorGrey;
            GUILayout.BeginVertical("box");
            GUI.color = colorDefault;


            foreach (var v in vars)
            {
                var reflect = v.serializedObject.GetReflectedValueAtPath(v.fieldPath);



                if (reflect != null)
                {
                    var val = reflect.Value;


                    if (!reflect.ValueNotFound)
                    {
                        // Debug.Log(val);
                        x++;

                        EditorGUI.BeginChangeCheck();

                        val = PropertyDrawerFinder.DrawPropertyOject(val, new GUIContent(v.FieldName), reflect.Type);
                        // val = PropertyDrawerFinder.DrawPropertyOject(reflect.Value val, new GUIContent(v.FieldName), reflect.Type);



                        if (EditorGUI.EndChangeCheck())
                        {
                            try
                            {
                                reflect.SetValue(val);
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                            if (v.serializedObject != null)
                                EditorUtility.SetDirty(v.serializedObject);
                        }


                    }
                    else
                    {
                        EditorGUILayout.LabelField(v.FieldName, "Field  Not found");
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(v.FieldName, "Field  Not found");
                }

            }



            if (x == 0)
                EditorGUILayout.HelpBox("Aucune variable globale configurée.", MessageType.Info, true);


            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILogiked.Panels.GUIDrawEditorIcon(() => p.SelectAssetInProjectWindow());
            GUILayout.EndHorizontal();

        }



#if History_prototype

        void AddHistoryNav(string path)
        {
            //	Debug.Log("add:"+path);
            string txt = Regex.Replace(path, "/([^/]*)$", @"/<color=red>$1 </color>");

            if (selectHistory == 0)
                navHistory.Insert(0, new KeyValuePair<string, string>(txt, path));
            else
                navHistory[selectHistory--] = new KeyValuePair<string, string>(txt, path);

            if (navHistory.Count > maxHistoryLen)
                navHistory.RemoveAt(maxHistoryLen);

        }


        private void Update()
        {

            if (Selection.activeObject != activeObj && Selection.activeObject != null)
            {
                activeObj = Selection.activeObject;

                if (!AssetDatabase.Contains(activeObj))
                    return;

                if (inSelection)
                    return;

                AddHistoryNav(AssetDatabase.GetAssetPath(activeObj.GetInstanceID()));
                Repaint();
            }

            inSelection = false;
        }


        [Obsolete("Prototype pas trés pratique.")]
        private void DrawHistoryCtx()
        {


            GUI.color = colorGrey;
            GUILayout.BeginVertical("box");
            GUI.color = colorDefault;

            if (navHistory.Count == 0)
                GUILayout.Label("history is empty");


            for (int i = 0; i < navHistory.Count; i++)
            {
                if (GUILayout.Button(((selectHistory > i) ? "<color=grey>" + navHistory[i].Key.Replace("=red", "=white") + "</color>" : (selectHistory == i) ? navHistory[i].Key.Replace("=red", "=green") : navHistory[i].Key), StyleList.Txt_navHistory, GUILayout.MaxHeight(18), GUILayout.MaxWidth(position.width - 20)))
                {
                    selectHistory = i;
                    inSelection = true;
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(navHistory[i].Value);
                }
            }

            GUILayout.EndVertical();
        }
#endif


        #endregion

    }

}


#endif