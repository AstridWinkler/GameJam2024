#if UNITY_EDITOR
using logiked.source.editor;
using logiked.source.extentions;
using logiked.source.graphNode;
using logiked.Tool2D.animation;
using logiked.Tool2D.settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static logiked.source.graphNode.GraphNodeUtils;
using Object = UnityEngine.Object;

public class AnimatronWindow2D : ScripableObjectGraphicEditor<AnimatorController2DFile, Animator2DTransition, Animation2DNode>
{


    //Test pour un mode "workspace only" qui limite les animation utilisables aux animation du dossier dans lequel l'animator se trouve
    //#define TEST_WORKSPACE_MODE

    #region INSTANTIATION


    [MenuItem("Logiked/Animatron Window", priority = 1001)]
    public static void EnableAnimotron()
    {
        StartWindow<AnimatronWindow2D>();
    }

    protected override void OnStart()
    {
#if TEST_WORKSPACE_MODE
        if (currentFile != null && currentFile.FolderOnlyEditorMode)
            HardUpdateAnimatorWorkspace();
#endif
    }


    private const string contextPath = "Assets/Create AnimatorController from";
    [MenuItem(contextPath, true)]
    static bool CreateAnimatorController()
    {
        if (!(Selection.activeObject is Animation2DFile))
        {
            return false;
        }
        return true;
    }



#endregion

#region FIELDS Redefinition
    protected override string DatasLabel_UniquePrefix => "anim_";
    protected override string DatasLabel_Postfix => currentFile.name;



#endregion





#region FIELDS Editor

    /// <summary>
    /// Rectangle de la fenetre de l'inspecteur en haut à droite
    /// </summary>
    private Rect inspectorWindowRect { get => new Rect(new Vector2(position.width - 260, 30), new Vector2(255, (position.height - 35) / 1f)); }

    /// <summary>
    /// Rectangle de la fenetre des variables à en haut à gauche
    /// </summary>
    private Rect inspectorVariableWindowRect { get => new Rect(new Vector2(10, 30), new Vector2(140, 50 + currentFile.Variables.Count * 20)); }
    private Rect inspectorVariableWindowRect_includeSettingsButtons { get => new Rect(new Vector2(inspectorVariableWindowRect.position.x, 0), new Vector2(inspectorVariableWindowRect.width, inspectorVariableWindowRect.position.y + inspectorVariableWindowRect.height)); }


    /// <summary>
    /// Liste des nom de variables contenues dans l'animator
    /// </summary>
    public string[] VariableListName => currentFile.Variables.Select(m => m.Name).ToArray();

    /// <summary>
    /// Liste des variables contenues dans l'animator 
    /// </summary>
    public int[] VariableListId => currentFile.Variables.Select(m => m.UniqueId).ToArray();




    /// <summary>
    /// Le render de l'animator selectionné en jeu.
    /// </summary>
    private AnimatorRenderer2D playingObject;

    /// <inheritdoc/>
   // public override string SerializedNodeArrayFieldName => "animations";

    public override bool OpenAutomacallySelectedFile => true;


    /// <summary>
    /// Dictionaire d'association IdVariables/variables
    /// </summary>
    private Dictionary<int, Animator2DVariable> variableDictionary = new Dictionary<int, Animator2DVariable>();



    #endregion



    #region Functions File & Edit 



    /// <summary>
    /// Actualise les nodes pour le mode FolderOnlyEditoMode. (Suprime les nodes qui ne sont pas liés au dossier et ajoute ceux qui le sont)
    /// </summary>
    public void HardUpdateAnimatorWorkspace()
    {
#if TEST_WORKSPACE_MODE
         currentFile.FolderOnlyEditorMode = false;//Pour pouvoir ajouter des nodes le fichier
#endif

        HashSet<Animation2DFile> toRemove = new HashSet<Animation2DFile>();//Suprime les nodes qui ne sont pas liés au dossier

        foreach (var anim in currentFile.Animations)
        {
            if (anim.File != null && Path.GetDirectoryName(AssetDatabase.GetAssetPath(anim.File)) != dataPath)
                toRemove.Add(anim.File);
        }
        currentFile.Animations.RemoveAll((m) => m.File == null || toRemove.Contains(m.File));

        AddWorkspaceNodes();//Ajoute les nodes liés au dossier


#if TEST_WORKSPACE_MODE
        currentFile.FolderOnlyEditorMode = true;//Pour verrouiller l'ajout de nodes aprés ca
#endif
    }

#if TEST_WORKSPACE_MODE
    private void OnFocus()
    {
        if (currentFile != null && currentFile.FolderOnlyEditorMode)
            HardUpdateAnimatorWorkspace();
}


/// <summary>
/// Change la valeur de FolderOnlyEditoMode(Edition mode dossier uniquement) et fait les modifs qui vont avec
/// </summary>
/// <param name="val"></param>
void SetFolderOnlyMode(bool val)
    {


        if (!currentFile.FolderOnlyEditorMode && val)//Si le mode n'etait pas activé et qu'on l'active, supression  de tout les nodes qui n'appartiennent pas au dossier. 
        {
            if (!EditorUtility.DisplayDialog("FolderOnlyEditoMode", "Etes vous sur de vouloir réactiver le mode ANIMATION DOSSIER UNIQUEMENT ? Vous allez perdre les nodes ne provenant pas du dossiers " + dataPath,
        "Vasy", "Annule"))
            {
                return;
            }
            Undo.RecordObject(currentFile, "Change WorkSpace mode");

            HardUpdateAnimatorWorkspace();
        } else
            Undo.RecordObject(currentFile, "Change WorkSpace mode");

        currentFile.FolderOnlyEditorMode = val;



    }

#endif





    #endregion


/// <inheritdoc/>
    public override bool Input_DragAndDropAllow(Object[] selection)
    {
        return selection != null && selection.Length >= 1 && selection.Where(m => m != null && m.GetType().Is<Animation2DFile>()).Count() > 0;
    }
/// <inheritdoc/>
    public override void Input_DragAndDropPreform(Object[] selection)
    {
        var obj = selection.Where(m => m != null && m.GetType().Is<Animation2DFile>()).ToArray();
        var pos = input_lastMousePos;

        for (int i = 0; i < obj.Length; i++)
        {
            AddNode(false, pos, (Animation2DFile)obj[i]);
            pos += Vector2.one * 50f;
        }
        DirtyNodeUpdate();
    }


#if TEST_WORKSPACE_MODE

#region FUNC GUI

    bool Draw_AlertFolderOnly()
    {
        if (EditorUtility.DisplayDialog("Remove Nodes", "L'animator est en mode ANIMATION DOSSIER UNIQUEMENT, Impossible d'ajouter ou de supprimer d'autres nodes que ceux déja présent dans le dossier.",
            "Désactivez moi cette daube", "Garder un projet clean"))
        {
            SetFolderOnlyMode(false);
        }
        return currentFile.FolderOnlyEditorMode;
    }
#endregion

#endif



#region FUNC Nodes


#if TEST_WORKSPACE_MODE
    protected override bool RemoveNodeCondition(params Animation2DNode[] nodes)
    {
        if (currentFile.FolderOnlyEditorMode && Draw_AlertFolderOnly())  //Si l'anim est en mode FolderOnlyEditoMode 
            return false;

        foreach (var e in nodes)
            if (e.File != null)
                e.File.RemoveLabel(GeneratedObjectsLabel);

        return true;
    }
#endif






    /// <summary>
    /// Ajoute un node dans l'animator
    /// </summary>
    /// <param name="position">Position du node dans la preview (mouseposition?)</param>
    /// <param name="file">Fichier de l'animation 2D</param>
    /// <param name="dirtyUpdate">Checker/Update le fichier. Ne pas activer pendant les ajout massifs de nodes, mais juste 1 fois apres via <see cref="ScripableObjectGraphicEditor{T, N}.DirtyNodeUpdate"/>.</param>
    protected void AddNode(bool dirtyUpdate, Vector2 position, Animation2DFile file)
    {

#if TEST_WORKSPACE_MODE
        if (currentFile.FolderOnlyEditorMode && Draw_AlertFolderOnly())  //Si l'anim est en mode FolderOnlyEditoMode 
            return;
#endif

        var node = AddEmptyNode(dirtyUpdate, position);
        node.File = file;



        if (file != null)
            UpdateAssetLabel(file);


    }









    protected sealed override void OnDirtyNodeUpdated()
    {
        int i, j, k;
        if (currentFile.Animations.Count == 0) return;

        EditorUtility.SetDirty(currentFile);
        playingObject = null;


        //Actualisation du point de départ de l'animator
        if (currentFile.Animations.FirstOrDefault(anim => anim.UniqueNodeId == currentFile.AnimationEntryPoint) == null)
            currentFile.AnimationEntryPoint = currentFile.Animations[0].UniqueNodeId;

        IList<Animator2DTransition> transitions;

        currentFile.Variables = currentFile.Variables.Distinct(m => m.UniqueId);

        var vars = VariableListId;


        //Supression des variables incohérentes dans les Rules des transitions

        for (i = 0; i < currentFile.Animations.Count; i++)
        {
            transitions = currentFile.Animations[i].AllTransitions;

            for (j = 0; j < transitions.Count; j++)
            {
                for (k = 0; k < transitions[j].Rules.Count; k++)
                {
                    if (!vars.Contains(transitions[j].Rules[k].VariableId))//Si la variable n'hexiste pas on la remplace par IDL                    
                        transitions[j].Rules[k].VariableId = 0;
                }
            }
        }


    }

    /// <summary>
    /// Est ce que l'animation est présent dans l'animator?
    /// </summary>
    private bool ContainsNode(Animation2DFile file)
    {
        foreach (var anim in currentFile.Animations)
            if (anim.File == file) return true;
        return false;
    }


    /// <summary>
    /// Ajoute tout les nodes du workspace à l'animator (seulement ceux qui ne sont pas présent dans l'animator)
    /// </summary>
    private void AddWorkspaceNodes()
    {

        var paths = AssetDatabase.FindAssets("t:" + typeof(Animation2DFile).Name, new string[] { dataPath });
        Animation2DFile[] files = new Animation2DFile[paths.Length];
        for (int i = 0; i < paths.Length; i++)
            files[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(paths[i]), typeof(Animation2DFile)) as Animation2DFile;

        // Debug.LogError(files.Length);



        for (int i = 0; i < files.Length; i++)
        {
            if (!ContainsNode(files[i]))
                AddNode(false, new Vector2((i % 4) * 170 * 1.4f, (i / 4) * 90 * 1.4f), files[i]);
        }
        DirtyNodeUpdate();


        //dataPath path 
    }



 




    #endregion


    public override List<Rect> InspectorRectList()
    {
        List<Rect> inspectorsRect = new List<Rect>();
        inspectorsRect.Add(inspectorVariableWindowRect_includeSettingsButtons);
        if (HasSelecetdNodes && !Input_isDraggingMovingSelection) inspectorsRect.Add(inspectorWindowRect);
        return inspectorsRect;
    }



   public  override void Update()
    {
        base.Update();
        CheckActiveObject();
    }

    void CheckActiveObject()
    {
        //Realtime Ingame Visualization & selected animator

 
         if (Selection.activeObject is GameObject)
        {

            if (!Application.isPlaying && !LogikedPlugin_2DTools.Instance.PlayAnimatorRenderersInSceneView)
            {
                playingObject = null;
                return;
            }

            var selectedPlayingObject = (Selection.activeObject as GameObject).GetComponent<AnimatorRenderer2D>();

            if (selectedPlayingObject != null && selectedPlayingObject.AnimatorFile != null)
            {
                if (selectedPlayingObject.AnimatorFile != currentFile)
                    SelectEditedFile(selectedPlayingObject.AnimatorFile);


                playingObject = selectedPlayingObject;
            }
        }
        else
        {
            playingObject = null;
        }
    }


    /// <inheritdoc/>
    protected override void OnGUiEditFile()
    {
        ClampNodeToGrid = LogikedPlugin_2DTools.Instance.NodeGridPlacement;
        EditorLineDesign = LogikedPlugin_2DTools.Instance.NodeTransitionStyleDesign;

    }


    /// <inheritdoc/>
    protected override void DrawWindows()
    {

        GUI.Window(14, inspectorVariableWindowRect, DrawVariableInspectorWindow, "Variables");

        if (selectedNodes.Count > 0)
        {
            GUI.Window(11, inspectorWindowRect, DrawInspectorWindow, "Inspector");
        }
    }



    /// <inheritdoc/>
    protected override void Input_LeftclickNode(Animation2DNode clickedNode)
    {
        /*
        if (Event.current.modifiers.HasFlag(EventModifiers.Shift))
        {
            if(lastSelectedNode != null)
            TryCreatetransition( lastSelectedNode.LastScreenPosition, clickedNode.LastScreenPosition);
        }
        */


        if (Input_isMouseDraggingPinConnector)
        {
            TryCreatetransition(Input_actualMousePinConnector, clickedNode.LastScreenPosition);
            StopArrowPinMouseFolowing();
        }



    }





    protected override void Input_MouseDrag(Event evnt) { }

    protected override void Input_MouseUp(Event evnt) { }
    
 
    





    protected override void Input_RightContextClick()
    {
        var pos = Event.current.mousePosition;
        Animation2DNode anim = GetNodeAtScreenPos(pos);
        GenericMenu menu = new GenericMenu();



        if (anim != null)
        {
        selectedNodes.Add(anim);
            menu.AddItem(new GUIContent("Set as startup node"), currentFile.AnimationEntryPoint == anim.UniqueNodeId, () => { currentFile.AnimationEntryPoint = anim.UniqueNodeId; });
            menu.AddItem(new GUIContent("Create Transition"), false, () => { StartPinDragging(anim.AnimationTransition);});
            menu.AddSeparator("");
        }

            FillEditGenericMenu(menu, input_lastMousePos);


#if TEST_WORKSPACE_MODE

        if (!currentFile.FolderOnlyEditorMode)
            {
                if (selectedNodes.Contains(anim))//remove all selected
                    menu.AddItem(new GUIContent("Remove Nodes"), false, () => { RemoveNodes(selectedNodes.ToArray()); });
                else//remove only the right clicked one
                    menu.AddItem(new GUIContent("Remove Node"), false, () => { RemoveNodes(anim); });
                menu.AddShortcut(KeyCode.Delete);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Remove Node    (Locked by WorkspaceMode)"));
            }
            
            if (!currentFile.FolderOnlyEditorMode)
            {
                menu.AddItem(new GUIContent("Add Animation"), false, () => { AddNode(pos); });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Add     (Locked by WorkspaceMode)"));
            }
#endif

        

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Reset view"), false, ResetView);

        menu.ShowAsContext();
    }




    /// <summary>
    /// Obtenir le nom d'une variable via son ID
    /// </summary>
    /// <param name="variableId">L'id de la variable</param>
    /// <returns>le nom de la variable</returns>
    private string GetVariableName(int variableId)
    {
        if (variableDictionary.Count != currentFile.Variables.Count || !variableDictionary.ContainsKey(variableId))
        {
            variableDictionary.Clear();
            currentFile.Variables.ForEach(m => variableDictionary.AddOrUpdate(m.UniqueId, m));
        }

        return variableDictionary.GetOrDefault(variableId)?.Name ?? "unkown";
    }



#region DRAW Void




    /// <summary>
    /// Classe qui stoque des informations pour le tracage des nodes et transitions à l'écran
    /// </summary>
    private class TransitionCounterDrawing
    {
        public int transitionCount;
        public Animation2DNode node;
        public bool isPointerOver;
        public Color transitionColor;
        public StringBuilder boxTextBuilder = new StringBuilder();
        public List<Animator2DTransition> concernedTransitions = new List<Animator2DTransition>();
    }


    TransitionCounterDrawing[] DrawTransitions(Animation2DNode node)
    {

        Handles.color = Color.white;
        Dictionary<int, TransitionCounterDrawing> idToDic = new Dictionary<int, TransitionCounterDrawing>(); //transition counter for arrow placement

        Animation2DNode destNode;
        Color col;
        int i, j;
        Animator2DTransition t;
        bool isSelected = selectedNodes.Count == 1 && lastSelectedNode == node;

        Handles.color = Color.white;

       


        for (j = 0; j < node.AllTransitions.Length; j++)
        {
            t = node.AllTransitions[j];
            if (currentFile.GetNodeById(t.NextNodeId) == null)//If dic not contains the next, there is a bug somwehere. Then refresh.
            {
                //Debug.LogWarning("Node " + t.NextNodeId + " not found. Removing transition...");
                node.AnimationTransition.RemoveTransition(t);
                DirtyNodeUpdate();
                break;
            }


            if (isSelected)
            {
                if (t.IsPointerOver)
                {
                    col = Color.magenta;
                }
                else
                    col = Color.cyan;
            }
            else
                col = Color.white;


            destNode = currentFile.GetNodeById(t.NextNodeId);

            var counter = idToDic.GetOrDefault(t.NextNodeId) ?? new TransitionCounterDrawing();
            counter.transitionCount++;
            counter.node = destNode;
            counter.concernedTransitions.Add(t);

            if (!counter.isPointerOver)
                counter.transitionColor = col;

            counter.isPointerOver |= t.IsPointerOver;




            if (isSelected)
            {

                if (idToDic.ContainsKey(t.NextNodeId))
                    counter.boxTextBuilder.AppendLine("\n<color=cyan>======= OR</color>");


                for (i = 0; i < t.Rules.Count; i++)
                {
                    if (i > 0)
                        counter.boxTextBuilder.Append("<color=yellow>AND</color>    ");
                    else
                        counter.boxTextBuilder.Append("<color=yellow>WHEN</color> ");

                    counter.boxTextBuilder.Append(t.Rules[i].ToString(GetVariableName(t.Rules[i].VariableId)));
                }

                if (t.Rules.Count > 0)
                    counter.boxTextBuilder.Append("<color=yellow>AND</color>    ");
                else
                    counter.boxTextBuilder.Append("<color=yellow>WHEN</color> ");

                counter.boxTextBuilder.Append(Regex.Replace(t.EndRuleInteraction.ToString(), "([a-z])([A-Z])", "$1 $2"));

            }

            idToDic.AddOrUpdate(t.NextNodeId, counter);
        }

        


        foreach (var toDraw in idToDic.Values)
        {
            DrawNodesConnection(node, toDraw.node, toDraw.transitionColor, toDraw.transitionCount, EditorLineDesign);
        }

        return idToDic.Values.ToArray();
    }




    /// <summary>
    /// Aide Visuelle : Dessine une petite boite de dialogue au dessus des nodes pour afficher les contiditions de transition pour y acceder.
    /// </summary>
    /// <param name="transitionVisual"></param>
    private void DrawTransitionTextBoxOverlay(TransitionCounterDrawing transitionVisual)
    {
        Vector2 center;

        GUI.color = Color.white * .9f;

        GUIContent text;
        Rect guiRect; 


        if (transitionVisual.boxTextBuilder.Length > 0)
        {
    
            if (transitionVisual.transitionColor == Color.magenta)
                GUI.color = Color.magenta;


      
            //Handles.Label(toDraw.node.LastScreenPosition, toDraw.text.ToString(), TransitionBoxOverlay);

            text = new GUIContent(transitionVisual.boxTextBuilder.ToString());
            center = (transitionVisual.node.LastScreenPosition + transitionVisual.node.LastScreenPosition) / 2f;
            guiRect = new Rect(center, StyleFlyingLabelOverlay.CalcSize(text));


            GUI.Label(guiRect, text, StyleFlyingLabelOverlay);        
            GUI.color = Color.white * .9f;

        
            if (mouseOver == MouseOver.EditArea && guiRect.Contains(Event.current.mousePosition))
            {
                foreach (var e in transitionVisual.concernedTransitions)
                    e.IsPointerOver = true;
            }

        }
        
        GUI.color = Color.white;
    }


    protected override void DrawNodes()
    {

        int i;

        var anims = currentFile.Animations.OrderBy(m => selectedNodes.Contains(m)).ToArray();//Oroner en mettant en premier les nodes selectionnés

        List<TransitionCounterDrawing> transitions = new List<TransitionCounterDrawing>();


        for (i = 0; i < anims.Length; i++)
            transitions.AddRange(DrawTransitions(anims[i]));

        for (i = 0; i < anims.Length; i++)
            DrawSingleNode(anims[i]);

        for (i = 0; i < transitions.Count; i++)
            DrawTransitionTextBoxOverlay(transitions[i]);

    }






    /// <summary>
    /// Dessiner un node Animator (avec son animation et tout)
    /// </summary>
    void DrawSingleNode(Animation2DNode node)
    {

        GUIStyle style;


   


        bool selected = selectedNodes.Contains(node);


        if (node.UniqueNodeId == currentFile.AnimationEntryPoint)
        { //If entry point

            if (node.TransitionsMode == Animation2DNode.AnimationTransitionMode.FromThisState)
                style = (selected ? NodeStyleLimeSelected : NodeStyleLime);
            else
                style = (selected ? NodeStyleOrangeSelected : NodeStyleOrange);
        }
        else
        {

            if (node.TransitionsMode == Animation2DNode.AnimationTransitionMode.FromThisState)
                style = (selected ? NodeStyleDefaultSelected : NodeStyleDefault);
            else
                style = (selected ? NodeStyleRedSelected : NodeStyleRed);

        }


        if (playingObject != null && playingObject.CurrentNodeId == node.UniqueNodeId)
        {
            style = (selected ? NodeStyleYellowSelected : NodeStyleYellow);
            node.isRealtimePlaying = true;
            node.percentPlaying = playingObject.percentStateFinished;
        }
        else
            node.isRealtimePlaying = false;

        node.NodeStyle = style;
        var newPos = GraphToScreenPosition(node.EditorPosition, ClampNodeToGrid);


        node.DrawNode(newPos, zoomCoef, null);





    }





#endregion




#region PANNELS
    protected override GenericMenu GetEditorSettingsContextMenu()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Reveal file in explorer"), false, () => ProjectBrowserReflection.SelectAssetInProjectWindow(currentFile));
        menu.AddShortcut(KeyCode.F1, EventModifiers.Control);
#if TEST_WORKSPACE_MODE
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Lock animations to workspace folder"), currentFile.FolderOnlyEditorMode, () => SetFolderOnlyMode(!currentFile.FolderOnlyEditorMode));
#endif
        var pluginSetting = LogikedPlugin_2DTools.Instance;

        menu.AddSeparator("");

        if(mouseOver != MouseOver.Inspector)
        FillEditGenericMenu(menu, CurrentScreenCenterPosition, "Edit");

        menu.AddSeparator("Edit/");
        menu.AddItem(new GUIContent($"Edit/Undo %Z"), false, Undo.PerformUndo);
        menu.AddItem(new GUIContent($"Edit/Redo %Y"), false, Undo.PerformRedo);

        menu.AddSeparator("");

        //  menu.AddItem(new GUIContent("Settings/Show popup WorkspaceMode related"), pluginSetting.ShowEditorDeletePopups, () => pluginSetting.ShowEditorDeletePopups = !pluginSetting.ShowEditorDeletePopups);
        menu.AddItem(new GUIContent("Visual Settings/Clamp nodes to grid"), pluginSetting.NodeGridPlacement, () => pluginSetting.NodeGridPlacement = !pluginSetting.NodeGridPlacement);
        menu.AddItem(new GUIContent("Visual Settings/Play animation preview"), pluginSetting.PlayPreviewInEditor, () => { pluginSetting.PlayPreviewInEditor = !pluginSetting.PlayPreviewInEditor; EditorUtility.SetDirty(pluginSetting); }) ;
        menu.AddSeparator("Visual Settings/");
        menu.AddItem(new GUIContent("Visual Settings/Bezier Transitions (sexy)"), pluginSetting.NodeTransitionStyleDesign == NodeLineDesign.SexyBezier, () => { pluginSetting.NodeTransitionStyleDesign = NodeLineDesign.SexyBezier; EditorUtility.SetDirty(pluginSetting); }) ;
        menu.AddItem(new GUIContent("Visual Settings/Linear Transitions"), pluginSetting.NodeTransitionStyleDesign == NodeLineDesign.Linear, () => { pluginSetting.NodeTransitionStyleDesign = NodeLineDesign.Linear; EditorUtility.SetDirty(pluginSetting); }) ;

        menu.AddItem(new GUIContent("Reset view"), false, ResetView);

        return menu;
    }


    Vector2 InspectorScrollViewTransitions;


    void FillEditGenericMenu(GenericMenu menu, Vector2 mousePosition, string path="")
    {
        if (path.Length > 0 && path.Last() != '/') path += '/';

        menu.AddItem(new GUIContent($"{path}Add Empty Node"), () => { AddEmptyNode(true, mousePosition); }, false);

        menu.AddSeparator($"{path}");

        menu.AddItem(new GUIContent($"{path}Remove"), () => { RemoveNodes(selectedNodes.ToArray()); }, selectedNodes.Count == 0, false);
        menu.AddShortcut(KeyCode.Delete);

        menu.AddItem(new GUIContent($"{path}Copy %C"), CopySelectedNodes, selectedNodes.Count == 0);
        menu.AddItem(new GUIContent($"{path}Paste %V"), () => PasteNodes(mousePosition), false);

        menu.AddSeparator($"{path}");
        menu.AddItem(new GUIContent($"{path}Select All"), false, () => { foreach (var e in currentFile.Animations) selectedNodes.Add(e); });
        menu.AddShortcut(KeyCode.A, EventModifiers.Control);


    }

    void DrawVariableInspectorWindow(int id)
    {
        Undo.RecordObject(currentFile, "Variable edit");

        GUI.enabled = !Application.isPlaying;

        Animator2DVariable v;

        if (currentFile.Variables.Count == 0 || currentFile.Variables[0].UniqueId != 0)
        {
           currentFile.Variables.Insert(0, new Animator2DVariable("idl", 0));
        }

        DrawVariable(currentFile.Variables[0], 0, false);


        // InspectorVariablesScrollView = GUILayout.BeginScrollView(InspectorVariablesScrollView, false, false);

        for (int i = 1; i < currentFile.Variables.Count; i++)
        {

            v = currentFile.Variables[i];

            DrawVariable(v, i);

        }


        GUI.enabled = true;

        //GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (!Application.isPlaying) {
            GUILayout.Label("New", GUILogiked.Styles.Text_BigBold);
            GUILogiked.Panels.GUIDrawEditorIcon(() => {
                Undo.RecordObject(currentFile, "Add variable");
                currentFile.Variables.Add(new Animator2DVariable("new var ", new System.Random().Next()));
            }, GUILogiked.Panels.EditorIconType.AddItem);
        } else
            GUILayout.Label("Playmode is active", GUILogiked.Styles.Text_BigBold);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        void DrawVariable(Animator2DVariable variable, int id, bool canDelete = true)
        {
            var ret = true;
            GUILayout.BeginHorizontal();

            GUI.enabled = canDelete && !Application.isPlaying;
            variable.Name = EditorGUILayout.TextField(variable.Name, GUILayout.Width(80));
            GUI.enabled = !Application.isPlaying;

            variable.Value = EditorGUILayout.IntField(variable.Value, GUILayout.Width(30));
            if (canDelete)
                GUILogiked.Panels.DrawArrayElementContextButton(currentFile.Variables, id, ApplyVariableChanges, false);

            GUILayout.EndHorizontal();



            void ApplyVariableChanges(IList vars)
            {
               var cast = (List<Animator2DVariable>)vars;
                if (cast.Count == 0 || cast[0].UniqueId != 0) return;//Apply only if the user not messup
                Undo.RecordObject(currentFile, "Variable Modify");

                currentFile.Variables = cast;

                DirtyNodeUpdate();
            }
        }
    }





    void DrawInspectorWindow(int id)
    {



        string[] variableListName = VariableListName;
        int[] variableListId = VariableListId;

        Animator2DTransition t;

        if (selectedNodes.Count > 1)
        {
            GUILayout.Label("Multi editing Nodes is not supported yet");
            return;
        }

        if (selectedNodes.Count == 0)
        {
            return;
        }

        bool forceExitLoop = false;
        var node = selectedNodes.ElementAt(0);
        Animation2DNode dest;


        InspectorScrollViewTransitions = GUILayout.BeginScrollView(InspectorScrollViewTransitions);


        GUILayout.Space(10);

        Undo.RecordObject(currentFile, "File Modify");



        GUILayout.Label("File", GUILogiked.Styles.Text_Bold);
#if TEST_WORKSPACE_MODE
        GUI.enabled = !currentFile.FolderOnlyEditorMode;
#endif

        node.File = (Animation2DFile)EditorGUILayout.ObjectField(node.File, typeof(Animation2DFile), false);
        GUI.enabled = true;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Speed Modifier");
        node.SpeedModifier = EditorGUILayout.FloatField(node.SpeedModifier);
        GUILayout.EndHorizontal();



        GUILayout.Space(10);

        GUI.enabled = node.File is Animation2DFileVariations;
        GUILayout.Label("Variations", GUILogiked.Styles.Text_Bold);
        GUILayout.BeginHorizontal();
        GUILayout.Label("driver variable");

        var currentV = node.VariationVariableId;
        if (!variableListId.Contains(currentV))
            currentV = 0;
        else for (int i = 0; i < variableListId.Length; i++)
            {
                if (currentV == variableListId[i])
                {
                    currentV = i;
                    break;
                }
            }
    

        node.VariationVariableId = variableListId[EditorGUILayout.Popup(currentV, variableListName, GUILayout.Width(60))];

        GUI.enabled = true;

        GUILayout.EndHorizontal();




        GUILayout.Space(10);
        GUILayout.Label("Transitions", GUILogiked.Styles.Text_Bold);

        node.TransitionsMode = PropertyDrawerFinder.DrawPropertyOject(node.TransitionsMode, new GUIContent(nameof(Animation2DNode.TransitionsMode), "Define how the transition are computed : when the current state is this node, or any nodes ? Works like the \"Any state\" node of the Unity Animator"));

        var transitions = node.AllTransitions;

        GUIStyle selectedTransitionStyle;

        if (transitions.Length == 0)
        {
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(inspectorWindowRect.width - 5));
            GUILayout.Label("No transitions found");
            GUILayout.EndHorizontal();
        }
        else for (int transId = 0; transId < node.AllTransitions.Length; transId++)
            {
                t = node.AllTransitions[transId];

                if (t.IsPointerOver)
                {
                    GUI.color = col_magenta;
                    selectedTransitionStyle = GUILogiked.Styles.Box_OpaqueWindowWhite;
                }
                else
                {
                    GUI.color = col_greybox;
                    selectedTransitionStyle = GUILogiked.Styles.Box_OpaqueWindowWhite;

                }

                GUILayout.BeginVertical(selectedTransitionStyle, GUILayout.MaxWidth(inspectorWindowRect.width - 5));


                GUI.color = Color.white;
                 
                GUILayout.BeginHorizontal();
                // GUILayout.Label();
                dest = currentFile.GetNodeById(t.NextNodeId);
                if (dest == null)
                {
                    Debug.LogWarning("Fixing node dictionnary");
                    DirtyNodeUpdate();
                    break;
                }



                GUILayout.Label("To {0}".Format((dest.ShortAnimationName)),GUILogiked.Styles.Text_BoldCentered, GUILayout.MaxWidth(inspectorWindowRect.width)  );

                GUILayout.FlexibleSpace();

                //if (transId != 0) GUILogiked.Panels.GUIDrawEditorIcon(() => { forceExitLoop = true; node.Transitions.Remove(t); node.Transitions.Insert(transId-1, t); }, GUILogiked.Panels.EditorIconType.ArrowUp);
                // GUILogiked.Panels.GUIDrawEditorIcon(() => { forceExitLoop = true; node.Transitions.Remove(t); }, GUILogiked.Panels.EditorIconType.RemoveCross);

                GUILogiked.Panels.DrawArrayElementContextButton(node.AllTransitions, transId, OnListModified,  false);

                void OnListModified(IList changed)
                {
                    node.AnimationTransition.Transitions = (Animator2DTransition[]) changed;
                }


                if (forceExitLoop) break;
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Space(15);//Margin
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Rules");
                GUILogiked.Panels.GUIDrawEditorIcon(() => { t.Rules.Add(new Animator2DRule()); }, GUILogiked.Panels.EditorIconType.AddItem);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                foreach (var r in t.Rules)
                {
                    if (!DrawRule(r, variableListName, variableListId))
                    {
                        t.Rules.Remove(r);
                        break;
                    }
                }





                GUILayout.BeginHorizontal();
                GUILayout.Label("When", GUILayout.Width(70));
                t.EndRuleInteraction = (Animator2DTransition.AnimationRuleEnd)EditorGUILayout.EnumPopup(t.EndRuleInteraction);
                GUILayout.Space(20);

                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();


                if (Event.current.type == EventType.Repaint)
                    t.IsPointerOver = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
                // lastRect.position += inspectorWindowRect.position;



            }

        GUILayout.FlexibleSpace();

        GUI.enabled = false;
        GUILayout.Label("Unique id : " + node.UniqueNodeId);
        GUI.enabled = true;

        GUILayout.EndScrollView();





   
    }



 
    bool DrawRule(Animator2DRule rule, string[] availableVariables, int[] availableVariablesIds )
    {
        var ret = true;
        GUILayout.BeginHorizontal();


        ///Get the variable ID
     
        
        var currentV = 0;

        for (currentV = 0; currentV < availableVariablesIds.Length; currentV++)        
            if (availableVariablesIds[currentV] == rule.VariableId) break;

        if (currentV == availableVariablesIds.Length)
            currentV = 0;


        rule.VariableId = availableVariablesIds[EditorGUILayout.Popup(currentV, availableVariables, GUILayout.Width(60))];
        rule.Comparator = (Animator2DRule.AnimationRuleComparaison)EditorGUILayout.EnumPopup(rule.Comparator);
        rule.Result = EditorGUILayout.IntField(rule.Result, GUILayout.Width(30) );
    
        GUILogiked.Panels.GUIDrawEditorIcon(() => ret = false, GUILogiked.Panels.EditorIconType.RemoveCross);

        GUILayout.EndHorizontal();
        return ret;
    }

    /*
    public override void ConfigureDuplicatedNode(Animation2DNode clonedNode, Dictionary<int, int> conversionTable)
    {
        var transitions = clonedNode.AllTransitions;

        foreach (var t in transitions)
        {
            if (conversionTable.ContainsKey(t.NextNodeId))
            {
                t.NextNodeId = conversionTable[t.NextNodeId];
            }
        }


    }

    */

#if CustomPropertyDrawer
    [CustomPropertyDrawer(typeof(Animation2DNode))]
    class MicroNodeEditor : PropertyDrawer
    {
        private void OnEnable()
        {

        }



        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);

            /*
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var effectRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var secondRect = new Rect(position.x, position.y + 20f, position.width, EditorGUIUtility.singleLineHeight);

            var effect = property.FindPropertyRelative("Effect");
            var name = property.FindPropertyRelative("ActionName");

            effect.intValue = EditorGUI.Popup(effectRect, "Effect", effect.intValue, effect.enumNames);
            
            EditorGUI.indentLevel = indent;
    */

            EditorGUI.EndProperty();
        }

        //This will need to be adjusted based on what you are displaying
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (20 - EditorGUIUtility.singleLineHeight) + (EditorGUIUtility.singleLineHeight * 2);
        }
    }

#endif





#endregion
}

#endif