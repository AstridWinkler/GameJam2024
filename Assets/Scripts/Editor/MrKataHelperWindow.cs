using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEditor.SceneManagement;
using logiked.source.editor;
using FunkyCode;
using FunkyCode.LightingSettings;

[System.Serializable]
public class MrKataHelperWindow : EditorWindow
{



    StyleList styleList;


    [SerializeField]
    List<KeyValuePair<string, string>> navHistory;

    private readonly int maxHistoryLen = 8;

    [SerializeField]
    private bool openCtx0 = false;
    [SerializeField]
    private bool openCtx1 = false;
     [SerializeField]
     private bool openCtx2 = false;
    // [SerializeField]
    //private bool openCtx3 = false;
    // [SerializeField]
    //private bool openCtx4 = false;
    // [SerializeField]
    //private bool openCtx5 = false;

    //*pour ajouter de nouveaux menus


    [SerializeField]
    bool isPainting = false;
    [SerializeField]
    bool inPainMenu = false;


    UnityEngine.Object activeObj;


    [SerializeField]
    private int selectHistory = 0;


    bool inSelection;






 //   [Serializable]
    class StyleList
    {
        [SerializeField]
        public GUIStyle stl_buttonTxt;
        [SerializeField]
        public GUIStyle stl_horizontalBox_greyTr;
        [SerializeField]
        public GUIStyle txt_navHistory;
        [SerializeField]
        public GUIStyle stl_whiteText;
        [SerializeField]
        public GUIStyle rich;
        [SerializeField]
        public GUIStyle txt_tilelst;
        [SerializeField]
        public GUIStyle txt_tilebox;

        [SerializeField]
        public Texture2D blank;

        //public GUIStyle win_gizmo;




        //des styles en bordel

        public StyleList()
        {


            //	stl_buttonTxt = new GUIStyle(GUI.skin.button);
            stl_buttonTxt = GUILogiked.Styles.Text_Bold;
            stl_buttonTxt.normal.background = null;
            stl_buttonTxt.richText = true;
            stl_buttonTxt.alignment = TextAnchor.MiddleLeft;
            stl_buttonTxt.margin = new RectOffset(0, 0, 0, 0);
            stl_buttonTxt.padding = new RectOffset(0, 0, 0, 0);
            



            stl_horizontalBox_greyTr = new GUIStyle();
            stl_horizontalBox_greyTr.margin = new RectOffset(0, 0, 0, 0);
            stl_horizontalBox_greyTr.padding = new RectOffset(0, 0, 0, 0);
            stl_horizontalBox_greyTr.alignment = TextAnchor.MiddleLeft;
            stl_horizontalBox_greyTr.normal.textColor = new Color(0.3f, 0.3f, 0.3f, 1f);



            //	txt_navHistory = new GUIStyle(GUI.skin.button);
            txt_navHistory = new GUIStyle();
            txt_navHistory.alignment = TextAnchor.MiddleRight;
            txt_navHistory.normal.background = null;
            txt_navHistory.margin = new RectOffset(0, 0, 2, 2);
            txt_navHistory.padding = new RectOffset(0, 0, 0, 0);
            txt_navHistory.richText = true;

            txt_tilelst = new GUIStyle();
            txt_tilelst.alignment = TextAnchor.MiddleCenter;
            txt_tilelst.normal.background = null;
            txt_tilelst.margin = new RectOffset(0, 0, 2, 2);
            txt_tilelst.padding = new RectOffset(0, 0, 0, 0);
            txt_tilelst.richText = true;


            txt_tilebox = new GUIStyle();           
            txt_tilebox.normal.background = blank;


            /*
            win_gizmo = new GUIStyle();
            txt_navHistory.normal.background = null;
   */
            stl_whiteText = new GUIStyle();
            stl_whiteText.normal.textColor = Color.white;
            stl_whiteText.alignment = TextAnchor.MiddleLeft;

            rich = new GUIStyle();
            rich.richText = true;

        }

    }








    [MenuItem("Window/MrKata Helper window")]
    [MenuItem("MrKata/Helper Window")]
    private static void OpenWindow()
    {
        MrKataHelperWindow window = GetWindow<MrKataHelperWindow>();
        window.titleContent = new GUIContent("Helper window");


    }







    void InitVars()
    {
        Debug.Log("helper:init");
        styleList = new StyleList();
        navHistory = new List<KeyValuePair<string, string>>();
        selectHistory = 0;


    }







    Vector2 scroll;


    private void OnGUI()
    {


        minSize = new Vector2(150f, 150f);

        if (styleList == null)
            InitVars();

        if(styleList.blank == null)
        {


            styleList.blank = new Texture2D(2, 2);
            styleList.blank.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });

        }


        if (updateObj)
        {
            Selection.activeGameObject = saveObj;
            updateObj = false;
        }

            BeginWindows();

        scroll = GUILayout.BeginScrollView(scroll);

        if (GUILayout.Button("Locate..."))
        {
            ContextMenu_Locate();
        }

        inPainMenu = false;

        DrawCtxBox(DrawHistoryCtx, "Navigation History", ref openCtx0);
        DrawCtxBox(LevelNavigationCtx, "Levels list", ref openCtx1);
        DrawCtxBox(LevelEditionCtx, "Levels edit", ref openCtx2);


        GUILayout.EndScrollView();





        EndWindows();

        Repaint();
    }


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
        if (navHistory == null)
            InitVars();



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







    private void DrawCtxBox(Action drawHistoryCtx, string boxName, ref bool isOpen)
    { 
        GUILayout.BeginVertical("box");
        if (GUILayout.Button(((isOpen) ? "▼ " : "► ") + boxName, styleList.stl_buttonTxt, GUILayout.MaxHeight(18), GUILayout.MaxWidth(position.width)))
        {
            isOpen = !isOpen;
        }

        if (isOpen)
        {
            drawHistoryCtx.DynamicInvoke();
        }
        GUILayout.EndVertical();
    }





    private void DrawHistoryCtx()
    {
        Color savedColor = GUI.color;
        Color greyCol = new Color(0.93f, 0.93f, 0.93f);



        GUI.color = greyCol;
        GUILayout.BeginVertical("box");
        GUI.color = savedColor;

        if (navHistory.Count == 0)
            GUILayout.Label("history is empty");


        for (int i = 0; i < navHistory.Count; i++)
        {
            if (GUILayout.Button(((selectHistory > i) ? "<color=grey>" + navHistory[i].Key.Replace("=red", "=white") + "</color>" : (selectHistory == i) ? navHistory[i].Key.Replace("=red", "=green") : navHistory[i].Key), styleList.txt_navHistory, GUILayout.MaxHeight(18), GUILayout.MaxWidth(position.width - 20)))
            {
                selectHistory = i;
                inSelection = true;
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(navHistory[i].Value);
            }
        }

        GUILayout.EndVertical();
    }



    string[] lstWorlds;
    string CurrentWorldPath => "Assets/Scenes/" + lstWorlds[selectedWorld] ;
    [SerializeField]
    int selectedWorld;
    string[] sceneList;
    string[] sceneNameList = new string[0];


    void UpdateSceneDir()
    {
        List<string> lst = Directory.GetDirectories("Assets/Scenes").ToList();
        lstWorlds = new string[lst.Count];
        int c = 0;            
        lst.ForEach ((st) => lstWorlds[c++] = Path.GetFileNameWithoutExtension(st)) ;
    }

    void UpdateSceneList(int newVal)
    {
        lstWorlds = lstWorlds.ToList().Where((m) => m != null).ToArray();
        newVal = ReMath.Clamp(newVal, 0, lstWorlds.Length-1);        
        selectedWorld = newVal;

        List<string> lst = Directory.GetFiles("Assets/Scenes/" + lstWorlds[newVal] ).ToList();
        lst = lst.Where((o) => o.Contains(".unity") && !o.Contains(".meta") ).ToList();
        int c = 0;
        sceneNameList = new string[lst.Count];
        sceneList = lst.ToArray();
        lst.ForEach((st) => sceneNameList[c++] = Path.GetFileNameWithoutExtension(st));
    }


    long waitUpdate = 0;


    void LevelNavigationCtx()
    {
        Color savedColor = GUI.color;
        Color greyCol = new Color(0.93f, 0.93f, 0.93f);

     

        if ((DateTime.Now.Ticks - waitUpdate) / TimeSpan.TicksPerMillisecond > 1000)
        {
            UpdateSceneDir();
            UpdateSceneList(selectedWorld);
            waitUpdate = DateTime.Now.Ticks;
        }



        // EditorGUILayout.EnumFlagsField(lstWorlds);
        GUILayout.BeginHorizontal();
        GUILayout.Label("World :");

        int sel = EditorGUILayout.Popup(selectedWorld, lstWorlds);
        if (sel != selectedWorld)
            UpdateSceneList(sel);
  

        GUILayout.EndHorizontal();
        

        if (sceneNameList.Length > 0)
        {

            GUI.color = greyCol;
            GUILayout.BeginVertical("box");
            GUI.color = savedColor;


            for (int i = 0; i < sceneNameList.Length; i++)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(sceneNameList[i]))
                {
                    if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        return;


                    EditorSceneManager.OpenScene(CurrentWorldPath + "/"+  sceneNameList[i] + ".unity");
                    LinkSceneToWorld();


                }


                if (GUILayout.Button("->", GUILayout.Width(20)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(sceneList[i]);
                }
                GUILayout.EndHorizontal();
            }




            GUILayout.EndVertical();
        }


        if (GUILayout.Button("New scene"))
        {

            if (!Application.isPlaying)
            {
                NewScene();
            }
        }

    }

    void LinkSceneToWorld()
    {
        LevelBlock block = GameObject.FindObjectOfType<LevelBlock>();

        if (block != null)
        {

            string[] world = AssetDatabase.FindAssets("", new[] { CurrentWorldPath });


            foreach (var item in world)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                /*
                            }
                            if (world.Length > 0)
                            {*/
                if (path.Contains("asset"))
                {

                    GameWorldFile t = (GameWorldFile)AssetDatabase.LoadAssetAtPath(path, typeof(GameWorldFile));
                    if (block.LinkedWorld != t)
                    {
                        block.LinkedWorld = t;
                        EditorUtility.SetDirty(block);
                    }
                    return;
                    //   lstWorlds[selectedWorld]
                }
            }
        }

    }



    void NewScene()
    {

        // var currentName = SceneManager.GetActiveScene().name;



        string currentName = "level0";

        if(sceneNameList.Length > 0)
        currentName = sceneNameList.Last();



        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;



        string tx, path;
        Scene sc;


        do {
            sc = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            tx = Regex.Replace(currentName, "([0-9]+)$", (m) =>
           {
               var i = int.Parse(m.Groups[1].Value);
               return (i + 1).ToString();
           });

            path  = "Assets/Scenes/"+ lstWorlds[selectedWorld] + "/" + tx + ".unity";


        } while (File.Exists(path));


        // Debug.LogError("save " + path);


        GameObject o = GameObject.Instantiate(((GameObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Prefabs/LevelStructure/LevelBasePack.prefab", typeof(GameObject))), Vector3.zero, new Quaternion());
        o.name = o.name.Replace("(Clone)", "");


        EditorSceneManager.SaveScene(sc, path);
        //Instantiate default obj

        //save
        UpdateSceneDir();
        UpdateSceneList(selectedWorld);

    }


    private void ContextMenu_Locate()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Levels"), false, () => SelectInspectorAtPath("Assets/Scenes/world1"));
        genericMenu.AddItem(new GUIContent("Inputs settings"), false, () => SelectInspectorAtPath("Assets/Resources/Config/DefaultInputs.asset"));


        genericMenu.ShowAsContext();
    }



    private void SelectInspectorAtPath(string path)
    {
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
    }


    int selectedTilemap = 0;

    bool updateObj;
    GameObject saveObj;

    bool map_hasCollision;
    float map_positionZ = 0;
    float tilemapAlpha;


    [InitializeOnLoadAttribute]
    public static class PlayModeStateChangedTilemapColor
    {
        static PlayModeStateChangedTilemapColor()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            Tilemap[] maps = GameObject.FindObjectsOfType<Tilemap>();
            foreach (var t in maps)  
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);
            }
        }




    void LevelEditionCtx()
    {
        int i = 0;
        Tilemap selectedObj = null;

        if (Selection.activeGameObject != null)
            selectedObj = Selection.activeGameObject.GetComponent<Tilemap>();

        Color savedColor = GUI.color;
        Color greyCol = new Color(0.95f, 0.95f, 0.95f);
        Color greyCol2 = new Color(0.85f, 0.85f, 0.85f);

        GUILayout.Label("Tilemaps", GUILogiked.Styles.Text_Bold);

        GUI.color = greyCol;
        GUILayout.BeginVertical("box");
        GUI.color = savedColor;

        Tilemap[] tilemaps = GameObject.FindObjectsOfType<Tilemap>();//.ToList().OrderBy((m) => m.transform.position.z).ToArray();

        selectedTilemap = ReMath.Clamp(selectedTilemap, 0, tilemaps.Length - 1);

        // if (selectedObj == null)
        //     selectedObj = tilemaps[selectedTilemap];
        if (tilemaps.Length <= 0)
        {
            GUILayout.Label("No tilemap were found !");
            GUILayout.EndVertical();
        }
        else {
            foreach (var t in tilemaps)
            {

                if (Application.isPlaying)
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);




                if (i != selectedTilemap)
                {
                    if (!Application.isPlaying)
                        t.color = new Color(t.color.r, t.color.g, t.color.b, 1f - tilemapAlpha);


                    if (i % 2 == 0)
                        GUI.color = greyCol;
                    else
                        GUI.color = greyCol2;
                }
                else
                {
                    GUI.color = Color.green;


                    if (!Application.isPlaying)
                        t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);

                }

                //Debug.LogError(styleList.txt_tilebox.normal.background);


                styleList.txt_tilebox.normal.background = styleList.blank;

                GUILayout.BeginHorizontal(styleList.txt_tilebox);

                if (GUILayout.Button(t.name, styleList.txt_tilelst))
                {
                    saveObj = Selection.activeGameObject;
                    if (saveObj != null && ((GameObject)saveObj).GetComponent<Tilemap>() != null)
                        saveObj = null;
                    updateObj = true;

                    selectedTilemap = i;
                    Selection.activeGameObject = tilemaps[i].gameObject;

                    map_hasCollision = tilemaps[i].gameObject.GetComponent<TilemapCollider2D>();
                }

                GUILayout.EndHorizontal();

                GUI.color = greyCol;


                i++;
            }

            GUILayout.EndVertical();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+"))
            {
                GameObject g = new GameObject("new tilemap");
                g.transform.SetParent(GameObject.FindObjectsOfType<Grid>().FirstOrDefault((m) => m.gameObject.layer == 9).transform);
                g.gameObject.layer = 9;
                g.AddComponent<Tilemap>();
                g.AddComponent<TilemapRenderer>();
            }
            GUILayout.EndHorizontal();




            GUILayout.Label("Selection", GUILogiked.Styles.Text_Bold);



            GUILayout.BeginVertical("box");

            Undo.RecordObject(tilemaps[selectedTilemap].gameObject, "tilemapGb");
            tilemaps[selectedTilemap].gameObject.name = EditorGUILayout.TextField("Tilemap name ", tilemaps[selectedTilemap].gameObject.name);

            var b = EditorGUILayout.Toggle("Has collision", map_hasCollision);

            if (b != map_hasCollision)
            {
                if (b)
                    Undo.AddComponent<TilemapCollider2D>(tilemaps[selectedTilemap].gameObject);
                else
                    Undo.DestroyObjectImmediate(tilemaps[selectedTilemap].gameObject.GetComponent<TilemapCollider2D>());
            }
            map_hasCollision = b;


            Undo.RecordObject(tilemaps[selectedTilemap].gameObject.transform, "tilemapTrf");
            tilemaps[selectedTilemap].transform.position = new Vector3(0, 0, EditorGUILayout.FloatField("Z position ", tilemaps[selectedTilemap].transform.position.z));



            GUILayout.EndVertical();
        }


        GUILayout.Label("Settings", GUILogiked.Styles.Text_Bold);



        ////   GUILayout.BeginVertical("box");
        // 
        //   GUILayout.EndVertical();

        GUILayout.BeginVertical("box");

        GUILayout.Label("Mask other tilemaps");

        GUILayout.BeginHorizontal();
        tilemapAlpha = EditorGUI.Slider(GUILayoutUtility.GetRect(30, 20), tilemapAlpha, 0, 1f);
        GUILayout.EndHorizontal();

        GUILayout.Label("Components");

        var tile = tilemaps[selectedTilemap];


        var tilemapCollider = tile.GetComponent<TilemapCollider2D>();
        var composite = tile.GetComponent<CompositeCollider2D>();
        var light2dCollider = tile.GetComponent<LightTilemapCollider2D>();
        var light2dOcclusion = tile.GetComponent<LightTilemapOcclusion2D>();


        EditorGUI.indentLevel++;
        bool useCollider = GUILayout.Toggle(tilemapCollider != null, "Use collider");
        bool useComposite = false;
        bool useLight2D = false;
        bool useOcclusion = false;

        if (useCollider)
        {
            useComposite = GUILayout.Toggle(composite != null, "Use composite collider");
            useLight2D = GUILayout.Toggle(light2dCollider != null, "Use Light2D effect");
            useOcclusion = GUILayout.Toggle(light2dOcclusion != null, "Use Occlusion");
        }

        EditorGUI.indentLevel--;



        void Savechanges()
        {
            Undo.RecordObject(tile.gameObject, "tilemap modification");
            EditorUtility.SetDirty(tile.gameObject);
        }


        if (tilemapCollider == null && useCollider)
        {
            tilemapCollider = Undo.AddComponent<TilemapCollider2D>(tile.gameObject);
            Savechanges();
        }

        if (tilemapCollider != null && !useCollider)
        {
            DestroyImmediate(tilemapCollider);
            Savechanges();
        }

        if(tilemapCollider != null)
            tilemapCollider.usedByComposite = useComposite;



        if (composite == null && useComposite)
        {
            composite = Undo.AddComponent <CompositeCollider2D>(tile.gameObject);
           var rig =  Undo.AddComponent <Rigidbody2D>(tile.gameObject);
            if (rig == null)
                rig = tile.GetComponent<Rigidbody2D>();
            rig.bodyType = RigidbodyType2D.Static;

            Savechanges();

        }

        if (composite != null && !useComposite)
        {
            DestroyImmediate(composite);
            DestroyImmediate(tile.GetComponent<Rigidbody2D>());
            useLight2D = false;

            Savechanges();
        }



        if (light2dCollider == null && useLight2D)
        {
            light2dCollider = Undo.AddComponent<LightTilemapCollider2D>(tile.gameObject);

            light2dCollider.mapType =  FunkyCode.LightTilemapCollider.MapType.UnityRectangle;
            
            light2dCollider.shadowTileType = ShadowTileType.ColliderOnly;
            light2dCollider.shadowLayer = 0;


            light2dCollider.rectangle.shadowOptimization = true; 
            light2dCollider.rectangle.shadowType = FunkyCode.LightTilemapCollider.ShadowType.CompositeCollider;

          
            light2dCollider.rectangle.maskType = FunkyCode.LightTilemapCollider.MaskType.Grid;
            light2dCollider.maskLayer = 0;


            Savechanges();
        }

        if (light2dCollider != null && !useLight2D)
        {
            DestroyImmediate(light2dCollider);
            Savechanges();

            useOcclusion = false;
        }



        if (light2dOcclusion == null && useOcclusion)
        {
            light2dOcclusion = Undo.AddComponent<LightTilemapOcclusion2D>(tile.gameObject);
            light2dOcclusion.onlyColliders = true;
            light2dOcclusion.sortingLayer = new FunkyCode.LightingSettings.SortingLayer();
            light2dOcclusion.sortingLayer.Name = "Shadow";
            light2dOcclusion.Initialize();


            Savechanges();
        }



        if (light2dOcclusion != null && !useOcclusion)
        {

            DestroyImmediate(light2dOcclusion);
           var comps = tile.gameObject.GetComponentsInChildren<MeshRenderer>();

            foreach (var c in comps)
                DestroyImmediate(c);
            Savechanges();
        }











        GUILayout.EndVertical();



    }




#if false

    List<BlockReference> allBlocks;
    bool sortOrder = false;



    //Recherche de tout les blocks valides dans le projet
    void RefreshBlocks()
    {
        allBlocks = new List<BlockReference>();
        BlockReference currentBlock;
        string assetPath;

        string[] guids = AssetDatabase.FindAssets(string.Format("t:" + typeof(BlockReference).ToString()));
        for (int i = 0; i < guids.Length; i++)
        {
            assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            currentBlock = AssetDatabase.LoadAssetAtPath<BlockReference>(assetPath);


            if (!currentBlock.IsTransitionBlock)
            {
                allBlocks.Add(currentBlock);
            }

        }
    }







    BlockUtilities.BlockTextureType currentBlockPreview = BlockUtilities.BlockTextureType.Solo;
    [SerializeField]
    BlockReference paintRef;

    private void BlockList()
    {
        inPainMenu = true;
        Color backCol = GUI.color;
        Texture2D blockSpr;
        float margin = position.width - ((position.width - 5) - 65);



        GUILayout.BeginHorizontal();

        if (allBlocks == null || GUILayout.Button("Refresh List"))
        {
            RefreshBlocks();
        }

        if (allBlocks == null || GUILayout.Button("Refresh map"))
        {
            GameBlock[] bl = GameObject.FindObjectsOfType<GameBlock>();
            foreach (GameBlock b in bl)
                b.UpdateBlockConnect(false, true);
        }


        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();

        currentBlockPreview = (BlockUtilities.BlockTextureType)EditorGUILayout.EnumPopup((Enum)currentBlockPreview, GUILayout.MaxWidth(margin / 2f));
        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal("box", GUILayout.MaxWidth(position.width - margin));
        if (GUILayout.Button("<color=blue>Name</color>", styleList.stl_buttonTxt, GUILayout.MaxHeight(18), GUILayout.MaxWidth((position.width - margin) / 3f)))
        {
            sortOrder = !sortOrder;
            allBlocks.Sort((x, y) => (sortOrder) ? string.Compare(x.BlockName, y.BlockName) : string.Compare(y.BlockName, x.BlockName));
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("<color=blue>Connection id</color>", styleList.stl_buttonTxt, GUILayout.MaxHeight(18), GUILayout.MaxWidth((position.width - margin) / 3f)))
        {
            sortOrder = !sortOrder;
            allBlocks.Sort((x, y) => (sortOrder) ? x.OrderConnection - y.OrderConnection : y.OrderConnection - x.OrderConnection);
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("<color=red>Enum id</color>", styleList.stl_buttonTxt, GUILayout.MaxHeight(18), GUILayout.MaxWidth((position.width - margin) / 3f)))
        {
            OpenCSharpBlockUtilities();

        }
    
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();


        foreach (BlockReference block in allBlocks)
        {
            if (paintRef == block && Application.isPlaying)
            {
                GUI.color = new Color(0.7f, 1f, 0.7f, 1f);
                GUILayout.BeginHorizontal("box", GUILayout.MaxWidth(position.width - 15));
                GUI.color = backCol;

            } else
                GUILayout.BeginHorizontal();


            //Un peu laggy mais osef. Zone de danger. ne pas toucher
            blockSpr = textureFromSprite(block.GetRandomSprite(currentBlockPreview, 0).FirstSprite);




            GUI.DrawTexture(GUILayoutUtility.GetRect(32, 32), blockSpr, ScaleMode.ScaleToFit, true);

            if (GUILayout.Button("?", GUILayout.Width(16)))
            {
                paintRef = block;
                Selection.activeObject = block;
                EditorUtility.FocusProjectWindow();
            }


            DestroyImmediate(blockSpr);



            block.BlockName = EditorGUILayout.TextField(block.BlockName, GUILayout.Width((position.width - margin) / 3.15f));

            block.OrderConnection = EditorGUILayout.IntField(block.OrderConnection, GUILayout.Width((position.width - margin) / 3.15f));
            //EditorGUILayout.EnumFlagsField.Label(block.ItemName, GUILayout.Width(position.width/3.1f));
            block.BlockType = (BlockUtilities.BlockList)EditorGUILayout.EnumPopup((Enum)block.BlockType, GUILayout.Width((position.width - margin) / 3.15f));

            EditorUtility.SetDirty(block);
  

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

        }





    }

    void OpenCSharpBlockUtilities()
    {

        string[] fileNames = Directory.GetFiles(Application.dataPath, "BlockUtilities.cs", SearchOption.AllDirectories);

        string finalFileName = Path.GetFullPath(fileNames[0]);
        //Debug.Log("File Found:" + fileNames[0] + ". Converting forward slashes: to backslashes" + finalFileName );

        //  System.Diagnostics.Process.Start("devenv", " /edit \"" + finalFileName + "\" /command \"edit.goto 16 \" ");
        System.Diagnostics.Process.Start("C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/Common7/IDE/devenv.exe", " /edit \"" + finalFileName + "\" /command \"edit.goto 16 \" ");

    }

    Texture2D textureFromSprite(Sprite sprite)
    {

        Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        newText.filterMode = FilterMode.Point;

        int y;
        for (int x = 0; x < sprite.rect.width; x++)
            for (y = 0; y < sprite.rect.height; y++)
                newText.SetPixel(x, y, sprite.texture.GetPixel(x + (int)sprite.rect.x, y + (int)sprite.rect.y));

        newText.Apply();
        return newText;
    }
























    // Window has been selected
    void OnFocus()
    {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.duringSceneGui -= this.DrawBlockPainting;
        // Add (or re-add) the delegate.
        SceneView.duringSceneGui += this.DrawBlockPainting;
    }

    void OnDestroy()
    {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.duringSceneGui -= this.DrawBlockPainting;
    }


    Rect paintRectWin = new Rect(10, 20, 150, 30);



 

    void DrawBlockPainting(SceneView sceneView)
    {

        if (!Application.isPlaying)
            return;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        int controlID2 = GUIUtility.GetControlID(FocusType.Passive);

        Color col = GUI.color;
        Handles.BeginGUI();
        Event e = Event.current;

        GUI.color = new Color(0.4f, 0.4f,0.4f,0.6f);
         if(inPainMenu && inPainMenu)
        paintRectWin = GUILayout.Window(controlID2, paintRectWin, PaintWin, "Block paint");
        paintRectWin = new Rect(paintRectWin.position, new Vector2(paintRectWin.width, 41));
        GUI.color = col;

        if (paintRef != null && isPainting)
        {


            if ((e.type == EventType.MouseDown || (e.control || paintLockControl) && e.type == EventType.MouseDrag) && e.button == 0)
            {
                GameManager.MapGenerator.GenerateInGameBlock(Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x,  Camera.current.pixelHeight - Event.current.mousePosition.y) ).ToVector2Int(), paintRef as BlockReference,e.shift || paintLockShift);
                GUIUtility.hotControl = controlID;
                Event.current.Use();
            }

            if ((e.type == EventType.MouseDown || (e.control || paintLockControl) && e.type == EventType.MouseDrag) && e.button == 1)
            {
                GameManager.MapGenerator.DestroyBlockAtPosition(Camera.current.ScreenToWorldPoint(new Vector2(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y)).ToVector2Int(), e.shift || paintLockShift);
                GUIUtility.hotControl = controlID;
                Event.current.Use();
            }

        }

        Handles.EndGUI();
    }

    [SerializeField]
    bool paintLockShift = false;
    [SerializeField]
    bool paintLockControl = false;



    void PaintWin (int windowID)
    {
        if (styleList == null || styleList.stl_whiteText == null)
            return;

        GUILayout.BeginHorizontal();
        isPainting = GUILayout.Toggle(isPainting, "", GUILayout.Width(20) );
        GUILayout.Label("Paint selected block", styleList.stl_whiteText);
        GUILayout.EndHorizontal();


        if (isPainting)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(25);
            GUILayout.Label("[Right click]: erase", styleList.stl_whiteText);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            paintLockShift = GUILayout.Toggle(paintLockShift, "", GUILayout.Width(20));
            GUILayout.Label("[Shift]: Background", styleList.stl_whiteText);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            paintLockControl = GUILayout.Toggle(paintLockControl, "", GUILayout.Width(20));
            GUILayout.Label("[Control]: instant add", styleList.stl_whiteText);
            GUILayout.EndHorizontal();

        }




        GUI.DragWindow();



    }

#endif
}















