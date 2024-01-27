#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using logiked.source.extentions;

using UnityEditor;
using Object = UnityEngine.Object;
using logiked.source.editor;
using static logiked.source.graphNode.GraphNodeUtils;

namespace logiked.source.graphNode
{

        /// <inheritdoc/>
    public abstract class ScripableObjectGraphicEditor<X> : ScripableObjectGraphicEditor<X, NodeTransition, GraphicNode> where  X : ScriptableObject, INodeStorage
    {

    }

    /// <summary>
    /// Base de fenêtre Unity pour l'édition de scriptable objects avec des graphiques. Par exemple : l'Animotron du LogikedPack2D ou alors 
    /// l'éditeur de dialogues 
    /// </summary>
    public abstract class ScripableObjectGraphicEditor<S, T, N> : Logiked_EditorWindow where T : NodeTransition where S : ScriptableObject, INodeStorage<N, T> where N : GraphicNode<T>
    {


        [NonSerialized]
        private bool checkReload = false;

        public virtual NodeLineDesign EditorLineDesign { get => editorLineDesign; set => editorLineDesign = value; }
        private NodeLineDesign editorLineDesign = NodeLineDesign.SexyBezier;

        /// <summary>
        /// Ouvrir automatiquement le fichier selectionné dans l'inspector ?
        /// </summary>
        public abstract bool OpenAutomacallySelectedFile { get; }

        /// <summary>
        /// Modification du type des nodes par défaut avec la fonction AddNode
        /// </summary>
        public virtual Type NewNodesType { get => null; }





        #region FIELDS Nodes
        /// <summary>
        /// Liste de nodes selectionnés
        /// </summary>
        protected HashSet<N> selectedNodes = new HashSet<N>();

        /// <summary>
        /// Retourne si un node ou plus sont séléctionnées
        /// </summary>
        protected bool HasSelecetdNodes => selectedNodes.Count > 0;

        /// <summary>
        /// Le dernier node cliqué
        /// </summary>
        protected N lastSelectedNode;

        /// <summary>
        /// Modifier le type des nodes instanciés
        /// </summary>
        protected N overrideNodeInstanceType;


        #endregion





#if EDITOR_PROPERTY_TEST
        /// <summary>
        /// Editor utilisé pour editer le fichier-ScriptableObject <see cref="currentFile"/> contenant les nodes.
        /// </summary>
        protected Editor cachedEditor;
#endif

        /// <summary>
        /// Propriété contenant les nodes sur le fichier-ScriptableObject <see cref="currentFile"/>.
        /// </summary>
        public SerializedProperty SerializedNodeArrayProperty;


        /// <summary>
        /// Stockage d'un guistyle et de sa taille de policd de base, afin de pouvoir lui appliquer le zoom de navigation de la fenetre
        /// </summary>
        private class DynamicFontScling
        {
            public GUIStyle style;
            public int baseFontSize;
            public DynamicFontScling(GUIStyle style)
            {
                this.style = style;
                baseFontSize = style.fontSize;
            }

            public void ModifyScale(float zoomCoef)
            {
                style.fontSize = (baseFontSize * zoomCoef).Rnd();
            }
        }

        /// <summary>
        /// Liste des style avec des taille de police variable, sensible au zoom
        /// </summary>
        List<DynamicFontScling> dynamicFontScaling = new List<DynamicFontScling>();


        /// <summary>
        /// Clone le style et retourne un modifié dynamiquement par l'edtieur en cas de zoom
        /// </summary>
        /// <param name="styleToAdd"></param>
        /// <returns>New automatic scaled type</returns>
        public GUIStyle RegisterFontForZoomScaling(GUIStyle styleToAdd)
        {
            GUIStyle newStyle = new GUIStyle(styleToAdd);
            dynamicFontScaling.Add(new DynamicFontScling(newStyle));
            return newStyle;
        }

        /// <summary>
        /// Mettre à jour les polices sensible au zoom
        /// </summary>
        public void FontScaleUpdate()
        {
            for (int i = 0; i < dynamicFontScaling.Count; i++)
            {
                dynamicFontScaling[i].ModifyScale(zoomCoef);
            }
        }







        /// <summary>
        /// Couleur de la banière en haut avec le nom du fichier édité
        /// </summary>
        protected Color TopBanner_Color = Color.black + Color.white * 0.1f;

        /// <summary>
        /// Rectangle de la banière en haut avec le nom du fichier édité
        /// </summary>
        protected Rect TopBannerRect => new Rect(0, 0, position.width, 25);


        /// <summary>
        /// Position visuelle au centre de la fenètre 
        /// </summary>
        public Vector2 CurrentScreenCenterPosition => GraphToScreenPosition(-ViewOffset);



        /// <summary>
        /// Mode d'édition actuel de la fenètre
        /// </summary>
        private EditMode editMode = EditMode.NoFile;




        private void OnEnable()
        {
            InitVals();
        }

        void InitVals()
        {
            Input_dragSelectionRect = new Rect();
            Input_isDragging = false;
            Input_isMouseArrowLineConnection = false;
            Input_actualMousePinConnector = null;
        }

#region FIELDS datas

        /// <summary>
        /// Le fichier edité par cette fenettre
        /// </summary>
        protected S currentFile;
        public S CurrentFile => currentFile;

        /// <summary>
        //Path de l'animator et de son dossier
        /// </summary>
        protected string currentFilePath;
        protected string dataPath;

#endregion

        protected Vector2 input_lastMousePos;

        protected Vector2 input_graph_startDragPosition;
        protected Vector2 Intput_screen_StartDragPosition => GraphToScreenPosition(input_graph_startDragPosition);

        /// <summary>
        /// Le pin survolé par la souris
        /// </summary>
       // protected NodeTransitionPin<T> Input_mouseOverPin;






        /// <summary>
        /// Le rectangle de selection quand la souris drag la zone d'édition
        /// </summary>
        protected Rect Input_dragSelectionRect { get; private set; }





        /// <summary>
        /// Est ce que la souris est en train de faire un Drag dans la zone d'édition ?
        /// </summary>
        protected bool Input_isDragging { private set; get; }




        /// <summary>
        /// Démarer une flèche qui suis la souris depuis un pin de connexion entre 2 nodes
        /// </summary>
        /// <param name="pin">Le boutton de connexion</param>
        protected void StartPinDragging(NodeTransitionPin<T> pin)
        {
            Input_actualMousePinConnector = pin;
            StartArrowMouseFolowing(pin.LastScreenPosition.center);
        }



        /// <summary>
        /// Démarer une flèche qui suis la souris (Disparait au clic). L'état de cette fleche est disponible sur le champ <see cref="Input_isMouseArrowLineConnection"/>
        /// </summary>
        /// <param name="startScreenPosition"></param>
        protected void StartArrowMouseFolowing(Vector2 startScreenPosition)
        {
            input_screen_startArrowPosition = startScreenPosition;
            Input_isMouseArrowLineConnection = true;
        }

        /// <summary>
        /// Stopper l'affichage de la flèche qui suis la souris
        /// </summary>
        protected void StopArrowPinMouseFolowing()
        {
            Input_isMouseArrowLineConnection = false;
            Input_actualMousePinConnector = null;
        }


        /// <summary>
        /// Position de départ de la flèche qui suis la souris
        /// </summary>
        protected Vector2 input_screen_startArrowPosition;
        /// <summary>
        /// Affichage d'une ligne qui va de <see cref="Intput_screen_StartDragPosition"/> à la position de la souris actuelle
        /// </summary>
        protected bool Input_isMouseArrowLineConnection { get; private set; }


        /// <summary>
        /// Est-ce que la souris est en train de drag un Pin de connexion entre 2 nodes ?
        /// </summary>
        protected bool Input_isMouseDraggingPinConnector { get => Input_actualMousePinConnector != null; }


        /// <summary>
        /// Le Pin actuelle drag par la souris
        /// </summary>
        protected NodeTransitionPin<T> Input_actualMousePinConnector { get; private set; }




        /// <summary>
        /// La souris est en train de déplacer des nodes via un Drag
        /// </summary>
        protected bool Input_isDraggingMovingSelection { get; private set; }

        /// <summary>
        /// La souris est en train de selectioner des nodes via un Drag
        /// </summary>
        protected bool input_isDraggingSelectionBox;





#region FIELDS View

        /// <summary>
        /// Déplacement de la caméra dans la zone d'édition des nodes 
        /// </summary>
        protected Vector2 ViewOffset { get; private set; }

        /// <summary>
        /// Etat du zoom. Plus cette valeur est grande, plus l'utilisateur à zoomé
        /// </summary>
        protected float zoomCoef = 1f;
        /// <summary>
        /// Valeur min et max du zoom
        /// </summary>
        protected Vector2 zoomCoefRange = new Vector2(0.4f, 2f);
        /// <summary>
        /// Force de zoom de la ScrollWheel
        /// </summary>
        protected float zoomCoefStrength = 0.03f;

        /// <summary>
        //A passer a true quand un affichage est modifié, pour repeindre la fenetre à la fin de l'event actuel
        /// </summary>
        protected bool needRepaint;


        /// <summary>
        /// Liste de tout les rectangles de fenêtre qui mettent le curseur de la souris hors-selection
        /// </summary>
        private List<Rect> inspectorsRect = new List<Rect>();

        /// <summary>
        /// Faire en sorte que les nodes soient attachés à la grille ?
        /// </summary>
        protected bool ClampNodeToGrid { get => clampNodeToGrid; set => clampNodeToGrid = value; }
        private bool clampNodeToGrid = true;


#endregion



#region FIELDS INPUTS


        /// <summary>
        /// Position actuelle de la souris 
        /// </summary>
        protected MouseOver mouseOver = MouseOver.EditArea;


#endregion

#region FIELDS Styles
        [NonSerialized] protected Color col_grey_background = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        [NonSerialized] protected Color col_magenta = new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.4f);
        [NonSerialized] protected Color col_greybox = new Color(0.3f, 0.3f, 0.3f, 0.6f);


        public GUIStyle NodeStyleDefault { get; private set; }
        public GUIStyle NodeStyleDefaultSelected { get; private set; }

        public GUIStyle NodeStyleLime { get; private set; }
        public GUIStyle NodeStyleLimeSelected { get; private set; }

        public GUIStyle NodeStyleYellow { get; private set; }
        public GUIStyle NodeStyleYellowSelected { get; private set; }

        public GUIStyle NodeStyleRed { get; private set; }
        public GUIStyle NodeStyleRedSelected { get; private set; }

        public GUIStyle NodeStyleOrange { get; private set; }
        public GUIStyle NodeStyleOrangeSelected { get; private set; }

        public GUIStyle StyleFlyingLabelOverlay { get; private set; }


        /// <summary>
        /// Liste des Rectangles des fenètres de l'écran, pour différencier quand la souris est dans une fenetre ou dans l'espace de travail avec les nodes.
        /// </summary>
        public abstract List<Rect> InspectorRectList();





#endregion


        public virtual void Update()
        {

            if (OpenAutomacallySelectedFile)
                CheckActiveSelection();
        }

        void CheckActiveSelection()
        {
            if (Selection.activeObject is S)
            {
                var f = Selection.activeObject as S;
                if (currentFile != f)
                    SelectEditedFile(f);
            }
        }


#region METHOD ABSTRACT

        /// <summary>
        /// Nom du champs contenant les nodes sur le scriptable object actuellement édité (<see cref="currentFile"/>)
        /// </summary>
      //  public abstract string SerializedNodeArrayFieldName { get; }


        /// <summary>
        /// A Appeler quand un node est ajouté/suprimé. Peutetre en O(n²). Permet de reconstruire l'arbre dans le fichier
        /// </summary>
        public virtual void DirtyNodeUpdate()
        {

            int i, j, p;

            if (nodeDict != null)
            {
                nodeDict.Clear();

                var ns = currentFile.GetNodes();

                for (i = 0; i < ns.Count; i++)
                {
                    nodeDict.Add(ns[i].UniqueNodeId, ns[i]);
                    ns[i].UpdatePinAttributes();
                }
            }


            //Supression des transitions obsolettes 
            T[] transitions;
            HashSet<T> toRemove = new HashSet<T>();
            IList<NodeTransitionPin<T>> transitionPins;

            var nods = currentFile.GetNodes();


            for (i = 0; i < nods.Count; i++)
            {
                nods[i].CurrentEditor = this;


                transitionPins = nods[i].TransitionPins;
                toRemove.Clear();

                for (p = 0; p < transitionPins.Count; p++)
                {

                    transitions = transitionPins[p].Transitions;


                  
                     
                            toRemove.UnionWith(transitions.Where(m => m == null || currentFile.GetNodeById(m.NextNodeId) == null));
                    

                    if (toRemove.Count > 0)
                    {
                        transitionPins[p].Transitions = transitions.SkipWhile(m => toRemove.Contains(m)).ToArray();
                    }

                }
            }

            selectedNodes.RemoveWhere(m => m == null);

            OnDirtyNodeUpdated();
        }

        /// <summary>
        /// Appelé quand un node est ajouté/suprimé. Permet de reconstruire l'arbre dans le fichier
        /// </summary>
        abstract protected void OnDirtyNodeUpdated();

        /// <summary>
        /// Ongui appelé quand l'objet est édité
        /// </summary>
        protected abstract void OnGUiEditFile();

        /// <summary>
        /// Ongui appelé quand aucun objet n'est édité
        /// </summary>
        protected virtual void OnGUiNoFileWindows() { }
        

        /// <summary>
        /// Moment de dessiner des fenetres avec Gui.windows, seulement quand le fichier est selectionné
        /// </summary>
        protected virtual void DrawWindows() { }



        /// <summary>
        /// Dessiner le panneau de settings
        /// </summary>
        protected virtual void DrawGuiEditorSettings()
        {
            var menu = GetEditorSettingsContextMenu();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILogiked.Panels.GUIDrawEditorIcon(menu, GUILogiked.Panels.EditorIconType.GearWhite);
            GUILogiked.Panels.GUIDrawEditorIcon(() => currentFile.SelectAssetInProjectWindow(), GUILogiked.Panels.EditorIconType.FolderWhite);
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Retourne le ContextMenu utilisé pour les settings généraux
        /// </summary>
        protected virtual GenericMenu GetEditorSettingsContextMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Reveal file in explorer"), false, () => currentFile.SelectAssetInProjectWindow());
            menu.AddShortcut(KeyCode.F1, EventModifiers.Control);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Clamp nodes to grid"), ClampNodeToGrid, () => ClampNodeToGrid = !ClampNodeToGrid);
            menu.AddItem(new GUIContent("Connection Style/Bezier"), () => EditorLineDesign = NodeLineDesign.SexyBezier, false, EditorLineDesign == NodeLineDesign.SexyBezier);
            menu.AddItem(new GUIContent("Connection Style/Linear"), () => EditorLineDesign = NodeLineDesign.Linear, false, EditorLineDesign == NodeLineDesign.Linear);

            menu.AddItem(new GUIContent("Reset view"), false, ResetView);

            menu.AddSeparator("");

            FillContextEditMenu(menu, CurrentScreenCenterPosition, "Edit");

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Undo %Z"), false, Undo.PerformUndo);
            menu.AddItem(new GUIContent("Redo %Y"), false, Undo.PerformRedo);




            return menu;
        }

        /// <summary>
        /// Reset la vue aux coord
        /// </summary>
        public void ResetView()
        {
            var nods = currentFile.GetNodes();
            if (nods.Count == 0)
                ViewOffset = Vector2.zero;
            else
                ViewOffset = -new Vector2(nods.Average(m => m.EditorPosition.x), nods.Average(m => m.EditorPosition.y));

            zoomCoef = 1;
        }


        /// <summary>
        /// Appelé quand un node est cliqué
        /// </summary>
        /// <param name="clickedNode"></param>
        protected virtual void Input_LeftclickNode(N clickedNode) { }

        /// <summary>
        /// Appelé quand la souris est relachée
        /// </summary>
        protected virtual void Input_MouseUp(Event evnt) { }


        /// <summary>
        /// Fonction apppelé par CoputeInput pour le Ctrl C +Ctrl V
        /// </summary>
        void ComputeInput_CheckCopyPaste()
        {
            bool copy = false;
            bool paste = false;

            if (Event.current.type == EventType.KeyDown)
            {

                copy = Event.current.control && Event.current.keyCode == KeyCode.C;
                paste = Event.current.control && Event.current.keyCode == KeyCode.V;
            }


            if (mouseOver == MouseOver.EditArea)
            {
                if (copy)
                    CopySelectedNodes();

                if (paste)
                    PasteNodes(CurrentScreenCenterPosition);
            }
        }


#endregion






        /// <summary>
        /// Copie les Ids des nodes séléctionnés et les enregistre dans le CopyBuffer en base64
        /// </summary>
        protected void CopySelectedNodes()
        {
            if (selectedNodes.Count > 0)
            {
                List<int> nodeIds = new List<int>();

                foreach (var n in selectedNodes)
                {
                    nodeIds.Add(n.UniqueNodeId);
                }

                byte[] serialized = nodeIds.SerializeToByteArray();
                string str = Convert.ToBase64String(serialized);
                Debug.Log($"Copying {nodeIds.Count} datas..");
                GUIUtility.systemCopyBuffer = str;
            }
        }

        /// <summary>
        /// Cherche dans le copyBuffer une liste d'ids de nodes en base64. Si les Ids sont trouvés, cette donction appelle <see cref="DuplicateNodes"/> avec les ids correspondants
        /// </summary>
        /// <param name="screenPosition"></param>
        protected void PasteNodes(Vector2? screenPosition = null)
        {
            try
            {
                string str = GUIUtility.systemCopyBuffer;
                byte[] serialized = Convert.FromBase64String(str);
                List<int> nodeArray = (List<int>)serialized.DeserializeToObject();
                Debug.Log($"Pasting {nodeArray.Count} datas..");

                if (nodeArray != null)
                {
                    var result = DuplicateNodes(nodeArray);

                    if (screenPosition != null)
                    {
                        Vector2 medianPosition = new Vector2(result.Average(m => m.EditorPosition.x), result.Average(m => m.EditorPosition.y));
                        Vector2 offset = ScreenToGraphPosition(screenPosition.Value) - medianPosition;
                        result.ForEach(m => m.EditorPosition += offset);
                    }

                }
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                Debug.LogException(e);
                //Nothing to paste
            }
        }


        /// <summary>
        /// Dupplique les nodes celon la liste d'Ids et leur applique un nouvel Id Unique. Cette fonction Utilise <see cref="ConfigureDuplicatedNode"/> pour la duplication
        /// </summary>
        /// <param name="nodeIds">Liste des Nodes à dupliquer</param>
        /// <returns>La liste de nouveaux nodes</returns>
        protected virtual List<N> DuplicateNodes(List<int> nodeIds)
        {
            List<N> result = new List<N>();
            Undo.RecordObject(currentFile, "Duplicate nodes");

            System.Random rand = new System.Random();
            Dictionary<int, int> nodeIdsConversionTable = new Dictionary<int, int>();

            for (int i = 0; i < nodeIds.Count; i++)
                nodeIdsConversionTable.Add(nodeIds[i], rand.Next());


            Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * 40f;

            N currentNode;

            List<GraphicNode<T>> pastedNodes = new List<GraphicNode<T>>();


            for (int i = 0; i < nodeIds.Count; i++)
            {
                currentNode = GetNodeById(nodeIds[i]);

                if (currentNode == null) continue;

                currentNode = (N)currentNode.Clone(nodeIdsConversionTable[currentNode.UniqueNodeId]);

                currentNode.EditorPosition += offset;

                ConfigureDuplicatedNode(currentNode, nodeIdsConversionTable);

                pastedNodes.Add(currentNode);
                result.Add(currentNode);
            }


            selectedNodes.Clear();
            selectedNodes.UnionWith(pastedNodes.Select(m => (N)m));
            lastSelectedNode = (N)pastedNodes.FirstOrDefault();

            var baseNodes = currentFile.GetNodes();
            pastedNodes.AddRange(baseNodes);
            currentFile.SetNodes(pastedNodes.Cast<N>().ToList());
            currentFile.SetDirtyNow();

            DirtyNodeUpdate();

            return result;
        }

        /// <summary>
        /// Permet de configurer un node dupliqué, pour modifier ses transitions par exemple
        /// </summary>
        /// <param name="clonedNode">Le node à dupliqué avec une fonction MemebrwiseClone()</param>
        /// <param name="conversionTable">La liste des Anciens-Nouveaux ids des nodes ajoutés</param>
        public virtual void ConfigureDuplicatedNode(N clonedNode, Dictionary<int, int> conversionTable) { }


        /// <summary>
        /// à appeler au démarage de fenètre par la classe enfant
        /// </summary>
        /// <typeparam name="W"></typeparam>
        public static void StartWindow<W>() where W : ScripableObjectGraphicEditor<S, T, N>
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            AssetDatabase.Refresh();
            W window = (W)GetWindow(typeof(W));
            window.SelectEditedFile(Selection.activeObject as S);

            window.Show();
            window.OnStart_();
        }


        /// <summary>
        /// Dé-selectionner les nodes 
        /// </summary>
        protected void DeselectNodes()
        {
            selectedNodes.Clear();
            lastSelectedNode = null;
        }


        protected void TryCreatetransition(NodeTransitionPin<T> startPin, Vector2 screenDestination)
        {
            if (startPin == null) return;
            N destNode = GetNodeAtScreenPos(screenDestination);
            var parentNode = GetNodeAtScreenPos(startPin.LastScreenPosition.center);

            //  if(startPin.AllowMultiple)

            if (startPin != null && destNode != null && parentNode != destNode)
            {

                Undo.RecordObject(currentFile, "Create Node Connection");

                var newTransition = (T)Activator.CreateInstance(typeof(T), destNode.UniqueNodeId);
                //newTransition.NextNodeId = destNode.UniqueNodeId;

                startPin.AddTransition(newTransition);
                needRepaint = true;
                DirtyNodeUpdate();
            }
        }




#region MEHTODS FILE 

        /// <summary>
        /// Selectionne un fichier à editer
        /// </summary>
        /// <param name="file">Le nouveau fichier</param>
        public void SelectEditedFile(S file)
        {

            if (currentFile != null)
                SaveCurrentFile();

            if (file == null) return;

            Debug.Log($"Open file {file.name}");

            currentFile = file;
            currentFilePath = AssetDatabase.GetAssetPath(currentFile);
            dataPath = Path.GetDirectoryName(currentFilePath);

            UpdateAssetLabel(file);
            DeselectNodes();
            SetEditMode(EditMode.Editing);
            DirtyNodeUpdate();

            ResetView();

            OnFileOpen();
            //if (!file.EditorInit)
            //    UpdateAnimatorWorkspace();

        }

        protected virtual void OnFileOpen() { }

        private void SaveCurrentFile()
        {
            //  Debug.Log("Save File");
            EditorUtility.SetDirty(currentFile);
            AssetDatabase.SaveAssets();
            // AssetDatabase.ImportAsset(currentFilePath, ImportAssetOptions.ForceUpdate);
        }


        /// <summary>
        /// Change le mode d'édition de la fenètre
        /// </summary>
        /// <param name="mode">Le nouveau mode</param>

        private void SetEditMode(EditMode mode)
        {
            editMode = mode;
        }

#endregion


#region MEHTODS EVENTS 




        private void OnStart_()
        {

            Undo.undoRedoPerformed += OnUndoRedo;

            DirtyNodeUpdate();

#if EDITOR_PROPERTY_TEST
            RefreshCachedEditor();
#endif
            //Debug.Log("On start");

            ResetView();

            zoomCoefRange = new Vector2(0.4f, 2f);
            zoomCoefStrength = 0.03f;

            dynamicFontScaling.Clear();

            NodeStyleDefault = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleDefault);
            NodeStyleDefaultSelected = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleDefaultSelected);

            NodeStyleLime = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleLime);
            NodeStyleLimeSelected = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleLimeSelected);

            NodeStyleYellow = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleYellow);
            NodeStyleYellowSelected = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleYellowSelected);

            NodeStyleOrange = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleOrange);
            NodeStyleOrangeSelected = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleOrangeSelected);

            NodeStyleRed = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleRed);
            NodeStyleRedSelected = RegisterFontForZoomScaling(GUILogiked.StylesNodeBox.NodeStyleRedSelected);


            StyleFlyingLabelOverlay = new GUIStyle(GUILogiked.Styles.Box_OpaqueWindowDark);
            StyleFlyingLabelOverlay.fontSize = 10;
            StyleFlyingLabelOverlay.richText = true;
            StyleFlyingLabelOverlay = RegisterFontForZoomScaling(StyleFlyingLabelOverlay);


            RegisterNewGuiStyles();

            OnStart();
        }

        protected virtual void OnStart() { }




        /// <summary>
        /// Founction called when the windows start. Declare used GuiStyle here and make them affected by the navigation zoom with <see cref="RegisterFontForZoomScaling(GUIStyle)"/>
        /// </summary>
        public virtual void RegisterNewGuiStyles() { }






        private void OnUndoRedo()
        {
            DirtyNodeUpdate();
        }




#if EDITOR_PROPERTY_TEST
        public void RefreshCachedEditor()
        {
            Editor.CreateCachedEditor(currentFile, null, ref cachedEditor);
            SerializedNodeArrayProperty = cachedEditor.serializedObject.FindProperty(currentFile.GetNodeArrayPropertyPath);
            if (SerializedNodeArrayProperty == null) Debug.LogError($"Property SerializedNodeArrayFieldName={currentFile.GetNodeArrayPropertyPath} can't be found on the object {currentFile.name}");
            OnRefreshCachedEditor();
        }

        public virtual void OnRefreshCachedEditor() { }

        public void UpdateCachedEditor()
        {
            cachedEditor.serializedObject.Update();
        }

        public void SaveCachedEditor()
        {
            cachedEditor.serializedObject.ApplyModifiedProperties();
        }
#endif







        private void OnLostFocus()
        {
            if (currentFile != null)
                SaveCurrentFile();
        }

        public void OnGUI()
        {



            if (currentFile == null)
                SetEditMode(EditMode.NoFile);


            ///Drawing

            EditorGUI.DrawRect(new Rect(1, 1, position.width, position.height), col_grey_background);

            float gridUnit = 25f;

            DrawGrid(gridUnit, 0.15f, Color.gray);
            DrawGrid(gridUnit * 2, 0.3f, Color.gray);
            DrawGrid(gridUnit * 4, 0.4f, Color.gray);





            ///Inputs
            if (currentFile != null)
            {
#if EDITOR_PROPERTY_TEST
                RefreshCachedEditor();
#endif

                FontScaleUpdate();

                if (!checkReload)
                {
                    checkReload = true;
                    OnStart_();
                }


                ComputeInputs();

                DrawGuiEditorSettings();
            }



            switch (editMode)
            {

                case EditMode.NoFile:

                    BeginWindows();
                    GUI.Window(48, new Rect(position.size.x / 2f - 100, position.size.y / 2f - 37, 200, 75), (i) =>
                    {
                        EditorGUILayout.HelpBox("No file selected. Please select a file in the project window to edit it.", MessageType.Error);
                    }, "No file");

                    OnGUiNoFileWindows();

                    EndWindows();


                    break;

                case EditMode.Editing:


                    OnGUiEditFile();

#if EDITOR_PROPERTY_TEST
                    UpdateCachedEditor();
#endif
                    DrawNodes();
#if EDITOR_PROPERTY_TEST
                    SaveCachedEditor();
#endif

                    DrawPinConnectionHighlight();
                    DrawAndApplySelectionRect();
                    DrawDraggingMouseArrow();

                    OnGuiAfterDrawingNodes();

                    BeginWindows();


                    GUI.color = TopBanner_Color;
                    GUI.Window(789, TopBannerRect, DrawHeaderBarWindow, new GUIContent(), GUILogiked.Styles.Box_OpaqueWhite);
                    GUI.color = Color.white;

                    DrawWindows();
                    EndWindows();


                    if (lastSelectedNode != null)
                    {
                        if (!nodeDict.ContainsKey(lastSelectedNode.UniqueNodeId))
                        {
                            DeselectNodes();
                        }
                    }

                    break;
            }



            if (needRepaint)
                Repaint();




        }







#endregion

        /// <summary>
        /// Fonction appelée aprés aoir dessiné tous les nodes et leur transitions
        /// </summary>
        protected virtual void OnGuiAfterDrawingNodes() { }

        private void DrawHeaderBarWindow(int winId)
        {

            GUILayout.BeginHorizontal();

            GUILogiked.Panels.GUIDrawEditorIcon(GetEditorSettingsContextMenu());


            if (GUILayout.Button(currentFilePath, GUILogiked.Styles.Button_NoBackgroundButton, GUILayout.Height(16)))
            {
                currentFile.SelectAssetInProjectWindow();
            }
            GUILayout.EndHorizontal();

        }



        /// <summary>
        /// Dessine une boite de texte volante 
        /// </summary>
        /// <param name="topLeftPosition">position du texte</param>
        /// <param name="text">Texte</param>
        /// <returns>Rectangle d'affichage du texte</returns>
        protected Rect DrawFlyingTextBox(Vector2 topLeftPosition, string text)
        {
            Vector2 center;
            GUI.color = Color.white * .9f;
            GUIContent message = new GUIContent(text); ;
            Rect guiRect;

            center = topLeftPosition;
            guiRect = new Rect(center, StyleFlyingLabelOverlay.CalcSize(message));


            GUI.Label(guiRect, text, StyleFlyingLabelOverlay);

            /*
            if (mouseOver == MouseOver.EditArea && guiRect.Contains(Event.current.mousePosition))
            {
               foreach (var e in transitionVisual.concernedTransitions)
                    e.IsPointerOver = true;
            }*/


            GUI.color = Color.white;
            return guiRect;
        }




        /// <summary>
        /// Applique les attributs des nodes & des pins 
        /// </summary>
        /*
        private void ApplyPinConnectionAttributes()
        {
            var nodes = currentFile.GetNodes();

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].UpdatePinAttributes();
            }
        }
        */

        /// <summary>
        /// Encadre le node en vert si il peut recevoir une connexion depuis un pin précedement drag
        /// </summary>
        private void DrawPinConnectionHighlight()
        {


            var node = GetNodeAtScreenPos(input_lastMousePos);

            if (node == null) return;

            if (Input_isMouseDraggingPinConnector)
            {
                if (!node.LastScreenRect.Contains(Intput_screen_StartDragPosition))
                {
                    GUI.color = Color.green - new Color(0, 0, 0, .5f);
                    GUI.Box(node.LastScreenRect, "", GUILogiked.Styles.Box_OpaqueWindowWhite);
                    GUI.color = Color.white;
                }
            }


        }

        private void DrawAndApplySelectionRect()
        {
            if (!input_isDraggingSelectionBox || Input_dragSelectionRect.width.Abs() + Input_dragSelectionRect.height.Abs() < 8f)
                return;

            Handles.DrawSolidRectangleWithOutline(Input_dragSelectionRect, new Color(0, 1f, 0f, 0.2f), new Color(0, 1f, 0f, 1f));

            var nodes = currentFile.GetNodes();
            Rect r;

            for (int i = 0; i < nodes.Count; i++)
            {
                r = nodes[i].LastScreenRect;

                if (Input_dragSelectionRect.Overlaps(r, true))
                {
                    if (selectedNodes.Add((N)nodes[i]))
                        lastSelectedNode = (N)nodes[i];
                }

            }

        }


        private void DrawDraggingMouseArrow()
        {
            if (Input_isMouseArrowLineConnection)
            {
                DrawLine(input_screen_startArrowPosition, Event.current.mousePosition, Color.white, 1, EditorLineDesign);
            }
        }





#region MEHTODS INPUTS

        /// <summary>
        /// Fonction d'autorisation du dragAndDrop de nodes, au dessus de la zone d'édition.
        /// </summary>
        /// <param name="selection">Les objets actuellement détenus par la souris</param>
        /// <returns>Le dragAndDrop est-il possible ?</returns>
        public abstract bool Input_DragAndDropAllow(Object[] selection);


        /// <summary>
        /// Fonction appelé une fois que le dragAndDrop de nodes est effectué
        /// </summary>
        /// <param name="selection">Les objets à traiter / ajouter</param>
        public abstract void Input_DragAndDropPreform(Object[] selection);


        /// <summary>
        /// Méthodes qui gère les des inputs dans la zone d'édition avec les nodes. (clics souris, Drag de nodes..)
        /// </summary>
        private void ComputeInputs()
        {
            var ev = Event.current;

            var newMouseOver = MouseOver.EditArea;

            int controlId = GUIUtility.GetControlID(FocusType.Passive);


            Vector2 dragDelta = (ev.mousePosition - input_lastMousePos) / zoomCoef;

            if (!Input_isDragging)
                input_lastMousePos = ev.mousePosition;


            //if (inspectorWindowRect.Contains(ev.mousePosition) && selectedNodes.Count > 0 || inspectorVariableWindowRect_includeSettingsButtons.Contains(ev.mousePosition))

            inspectorsRect = InspectorRectList();
            inspectorsRect.Add(TopBannerRect);


            for (int i = 0; i < inspectorsRect.Count; i++)
            {
                if (inspectorsRect[i].Contains(ev.mousePosition))
                {
                    newMouseOver = MouseOver.Inspector;
                    break;
                }
            }



            mouseOver = newMouseOver;


            if (input_isDraggingSelectionBox)
                Input_dragSelectionRect = new Rect(Intput_screen_StartDragPosition, ev.mousePosition - Intput_screen_StartDragPosition);

            ComputeInput_CheckCopyPaste();


            switch (ev.GetTypeForControl(controlId))
            {


                case EventType.DragUpdated:
                    if (Input_DragAndDropAllow(DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                        ev.Use();
                    }
                    break;


                case EventType.DragPerform:
                    if (Input_DragAndDropAllow(DragAndDrop.objectReferences))
                    {
                        Input_DragAndDropPreform(DragAndDrop.objectReferences);
                        ev.Use();
                    }
                    break;


                case EventType.MouseDown:

                    GUIUtility.keyboardControl = 0;


                    if (mouseOver != MouseOver.EditArea) break;

                    if (Input_isDragging)
                    {
                        Input_isDragging = false;
                        ev.Use();
                        break;
                    }

                    switch (ev.button)
                    {

                        case 0:
                            input_graph_startDragPosition = ScreenToGraphPosition(ev.mousePosition);
                            input_lastMousePos = ev.mousePosition;
                            Input_dragSelectionRect = new Rect(ev.mousePosition, Vector2.zero);

                            GUIUtility.hotControl = controlId;


                            Input_Leftclick();

                            if (!Input_isDraggingMovingSelection)
                                input_isDraggingSelectionBox = true;

                            Event.current.Use();
                            Input_isDragging = true;
                            break;


                        case 1:
                        case 2:
                            input_graph_startDragPosition = ScreenToGraphPosition(ev.mousePosition);
                            input_lastMousePos = ev.mousePosition;
                            Input_dragSelectionRect = new Rect(ev.mousePosition, Vector2.zero);

                            GUIUtility.hotControl = controlId;
                            Event.current.Use();
                            Input_isDragging = true;
                            break;
                    }



                    needRepaint = true;


                    break;




                case EventType.MouseDrag:

                    input_lastMousePos = ev.mousePosition;

                    Input_MouseDrag(ev);

                    if (!Input_isDragging) return;

                    switch (ev.button)
                    {
                        case 0:
                            if (Input_isDraggingMovingSelection && !Input_isMouseArrowLineConnection)
                            {
                                for (int i = 0; i < selectedNodes.Count; i++)
                                {
                                    Undo.RecordObject(currentFile, "Node Movement");
                                    selectedNodes.ElementAt(i).EditorPosition += dragDelta;
                                }
                            }

                            break;



                        case 1:
                        case 2:
                            ViewOffset += dragDelta;
                            break;
                    }

                    needRepaint = true;
                    break;






                case EventType.MouseUp:

                    Input_MouseUp(ev);


                    if (Input_isMouseArrowLineConnection)
                        TryCreatetransition(Input_actualMousePinConnector, ev.mousePosition);


                    Input_isDragging = false;

                    StopArrowPinMouseFolowing();

                    Input_dragSelectionRect = new Rect();
                    input_isDraggingSelectionBox = false;

                    if (ClampNodeToGrid)
                        selectedNodes.ForEach(m => m.EditorPosition = ClampPosition(m.EditorPosition));



                    if (mouseOver == MouseOver.EditArea)
                    {

                        if (ev.button == 1)
                        {
                            if (Vector2.Distance(ev.mousePosition, Intput_screen_StartDragPosition) < 10)//Si l'utilisateur a quasiement pas bougé et a fait un clic droit
                            {
                                Input_RightContextClick();
                            }
                        }
                    }


                    Input_isDraggingMovingSelection = false;

                    break;


                case EventType.ScrollWheel:


                    if (mouseOver != MouseOver.EditArea) break;

                    zoomCoef -= ev.delta.y * zoomCoefStrength * zoomCoef;
                    zoomCoef = Mathf.Clamp(zoomCoef, zoomCoefRange.x, zoomCoefRange.y);




                    needRepaint = true;
                    break;
            }
        }


        private void Input_Leftclick()
        {
            bool controlKey = Event.current.modifiers.HasFlag(EventModifiers.Control);


            N node = GetNodeAtScreenPos(Event.current.mousePosition);
            if (node == null)
            {
                if (!controlKey)
                    DeselectNodes();

                return;
            }

            Input_LeftclickNode(node);



            var buttons = node.TransitionPins;


            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].isMouseInsideDragLinkTransition)
                {
                    StartPinDragging(buttons[i]);
                }
            }




            if (!selectedNodes.Contains(node) && !controlKey)
                DeselectNodes();
            else if (selectedNodes.Contains(node) && controlKey)
            {
                selectedNodes.Remove(node);
                return;
            }



            if (node.LastScreenRect.Contains(Event.current.mousePosition))
            {
                Input_isDraggingMovingSelection = true;
                selectedNodes.Add(node);
                lastSelectedNode = node;
            }
        }


        /// <summary>
        /// Fonction appelée quand la souris effectue un drag. Les Gui ne sont pas affichable dans cet event.
        /// </summary>
        protected virtual void Input_MouseDrag(Event evnt) { }

        /// <summary>
        /// Fonction appelée quand la souris effectue un click droit. Les Gui ne sont pas affichable dans cet event.
        /// </summary>
        protected virtual void Input_RightContextClick()
        {
            var pos = Event.current.mousePosition;
            N node = GetNodeAtScreenPos(pos);

            if (node != null)
            {
                if (!selectedNodes.Contains(node))
                {
                    selectedNodes.Clear();
                }
                selectedNodes.Add(node);


                Input_actualMousePinConnector = null;
                var buttons = node.TransitionPins;

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i].isMouseInsideDragLinkTransition)
                    {
                        Input_actualMousePinConnector = buttons[i];
                    }
                }

            }

            GenericMenu menu = new GenericMenu();
            FillContextEditMenu(menu, pos);



            menu.ShowAsContext();
        }

#endregion




#region DRAW METHODS


        /// <summary>
        /// Dessine la grille en background, en prenant en compte le zoom
        /// </summary>
        /// <param name="gridSpacing">Espacement en pixel de la grille (pour un zoom = 1)</param>
        /// <param name="gridOpacity">Opacité de la grille</param>
        /// <param name="gridColor">Couleur de la grillle</param>
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {


            Rect visualPosition = new Rect(position.center, position.size / zoomCoef);

            int widthDivs = Mathf.CeilToInt(visualPosition.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(visualPosition.height / gridSpacing);




            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector2 gridOffset = ViewOffset;

            float unzoomDelta = zoomCoef - 1f;

            int res;

            gridSpacing *= zoomCoef;


            gridOffset += position.size * 0.5f + gridOffset * unzoomDelta;

            gridOffset = new Vector3(gridOffset.x % gridSpacing, gridOffset.y % gridSpacing);


            for (int i = 0; i <= widthDivs; i++)
            {
                res = i;
                Handles.DrawLine(new Vector3(gridSpacing * res + gridOffset.x, -gridSpacing, 0), new Vector3(gridSpacing * res + gridOffset.x, position.height, 0f));
            }

            for (int j = 0; j <= heightDivs; j++)
            {
                res = j;
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * res + gridOffset.y, 0), new Vector3(position.width, gridSpacing * res + gridOffset.y, 0f));
            }



            Handles.color = Color.white;
            Handles.EndGUI();
        }


        /// <summary>
        /// Draw a design line between two nodes
        /// </summary>
        /// <param name="node1">Start node</param>
        /// <param name="node2">Destination node</param>
        /// <param name="col">Line Color</param>
        /// <param name="drawArrowsCount">Draws tinny dircetional arrows if greater than 0</param>
        /// <param name="lineDesign">Draw this line like a bezier curve of linear ?</param>
        protected void DrawNodesConnection(N node1, N node2, Color col, int drawArrowsCount = 0, NodeLineDesign lineDesign = NodeLineDesign.SexyBezier)
        {
            Vector2 startPos;
            Vector2 destPos;

            startPos = node1.EditorPosition;
            destPos = node2.EditorPosition;
            Vector2 lineDir = (destPos - startPos).normalized;

            Vector2 directionNormal;

            const float offsetSize = 5f;

            switch (lineDesign)
            {


                case NodeLineDesign.SexyBezier:

                    BezierGrowDirection bezierTangentDir = BezierGrowDirection.Horizontal;


                    //Faire que les fleches parent des bordures du node
                    startPos = node1.GetVisualAttachPoint(lineDir);
                    destPos = node2.GetVisualAttachPoint(-lineDir);

                    //Direction de la courbe de bezier
                    bezierTangentDir = lineDir.x.Abs() > lineDir.y.Abs() ? BezierGrowDirection.Horizontal : BezierGrowDirection.Vertical;

                    //On rajoute un petit offset pour ne pas confondre la direction entre les nodes
                    if (bezierTangentDir == BezierGrowDirection.Horizontal) lineDir = new Vector2(lineDir.x.Sign(), 0);
                    else lineDir = new Vector2(0, lineDir.y.Sign());

                    directionNormal = new Vector2(-lineDir.y, lineDir.x);
                    startPos += directionNormal * offsetSize * zoomCoef;
                    destPos += directionNormal * offsetSize * zoomCoef;

                    DrawLine(startPos, destPos, col, drawArrowsCount, lineDesign, bezierTangentDir);
                    break;

                case NodeLineDesign.Linear:


                    startPos = node1.LastScreenPosition;
                    destPos = node2.LastScreenPosition;


                    directionNormal = new Vector2(-lineDir.y, lineDir.x);
                    startPos += directionNormal * offsetSize * zoomCoef;
                    destPos += directionNormal * offsetSize * zoomCoef;




                    DrawLine(startPos, destPos, col, drawArrowsCount, lineDesign);
                    break;
            }
        }



        protected void DrawPintransition(NodeTransitionPin<T> pin, NodeLineDesign lineDesign = NodeLineDesign.SexyBezier)
        {
            if (pin == null) return;

            Vector2 startPos;
            Vector2 destPos;

            startPos = pin.LastScreenPosition.center;

            var transGroup = pin.Transitions.SkipWhile(m => m==null).GroupBy(m => m.NextNodeId).ToList();

            int j;

            for (j = 0; j < transGroup.Count; j++)
            {

                var destNode = GetNodeById(transGroup[j].Key);

                if (destNode == null)
                {
                    Debug.LogWarning($"Destination node not found : {transGroup[j].Key}, removing transition..");
                    pin.RemoveTransition(transGroup[j].Key);
                    DirtyNodeUpdate();
                    break;
                }
                // destPos = destNode.GetVisualAttachPoint(startPos - destNode.LastScreenPosition);

                destPos = destNode.GetVisualAttachPoint(lineDesign == NodeLineDesign.SexyBezier ? Vector2.left : startPos - destNode.LastScreenPosition);


                DrawLine(startPos, destPos, Color.white, transGroup[j].Count(), lineDesign, BezierGrowDirection.Right);
            }





        }





        /// <summary>
        /// Dessine une ligne entre deux points 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="destPos"></param>
        /// <param name="col"></param>
        /// <param name="drawArrowsCount"></param>
        /// <param name="lineDesign"></param>
        /// <param name="bezierDircetion"></param>
        protected void DrawLine(Vector2 startPos, Vector2 destPos, Color col, int drawArrowsCount = 0, NodeLineDesign lineDesign = NodeLineDesign.SexyBezier, BezierGrowDirection bezierDircetion = BezierGrowDirection.Horizontal)
        {
            Handles.BeginGUI();
            Vector2 dirVector = Vector2.zero;

            if (lineDesign == NodeLineDesign.SexyBezier)
            {

                //Direction des tangentes de la courbe
                dirVector = destPos - startPos;
                if (dirVector.magnitude < 3f) return;

                float sign;

                if (BezierGrowDirection.Vertical.HasFlag(bezierDircetion))
                {
                    dirVector.x = 0;
                    sign = dirVector.y.Sign();
                    if (bezierDircetion != BezierGrowDirection.Vertical)
                        sign = bezierDircetion == BezierGrowDirection.Left ? -1f : 1f;

                    dirVector.y = sign * Mathf.Min(150f * zoomCoef, dirVector.y.Abs());
                }
                else
                {
                    sign = dirVector.x.Sign();
                    if (bezierDircetion != BezierGrowDirection.Horizontal)
                        sign = bezierDircetion == BezierGrowDirection.Top ? -1f : 1f;
                    dirVector.x = sign * Mathf.Min(150f * zoomCoef, dirVector.x.Abs());
                    dirVector.y = 0;
                }


                Handles.DrawBezier(startPos, destPos, startPos + dirVector, destPos - dirVector, col, null, 5f * zoomCoef);
            }
            else
            {
                Handles.DrawBezier(startPos, destPos, startPos, destPos, col, null, 5f * zoomCoef);
            }


            //  Vector2 center = (destPos + startPos) / 2f;

            var p1 = startPos;
            var p2 = startPos + dirVector;
            var p3 = destPos - dirVector;
            var p4 = destPos;

            int maxCnt = drawArrowsCount + 1;

            for (int i = 1; i < maxCnt; i++)
            {
                DrawArrow(((float)i / maxCnt).Remap(0, 1, 0.3f, 0.7f));//Center arrows
            }



            ///t = [0;1] point for arrow placement
            Vector2 GetBezierCoord(float t)
            {
                //4 points bezier formula
                Vector2 center = Mathf.Pow(1f - t, 3) * p1 + 3 * Mathf.Pow(1f - t, 2) * t * p2 + 3 * (1f - t) * Mathf.Pow(t, 2) * p3 + Mathf.Pow(t, 3) * p4;
                return center;
            }


            void DrawArrow(float t)
            {
                var bez1 = GetBezierCoord(t + 0.01f);
                var bez2 = GetBezierCoord(t - 0.01f);


                float arrowSize = 7f * zoomCoef;

                Vector2 center = (bez1 + bez2) / 2f;

                Vector2 dotTangent = (bez1 - bez2).normalized;

                Vector2 dir = dotTangent.normalized * arrowSize;

                Vector2 normal = new Vector2(-dir.y, dir.x);


                Vector2 cap1 = center + normal - dir * 2f;
                Vector2 cap2 = center - normal - dir * 2f;


                Handles.color = col;
                Handles.DrawAAConvexPolygon(cap1 + dir, cap2 + dir, center + dir);
            }

            Handles.EndGUI();

        }



#endregion




        [NonSerialized]
        private Dictionary<int, GraphicNode<T>> nodeDict = new Dictionary<int, GraphicNode<T>>();



#region FUNC Nodes


        /// <summary>
        /// Retourne le node via son ID
        /// </summary>
        /// <param name="id">Id du node</param>
        /// <returns>Le node</returns>
        protected N GetNodeById(int id)
        {

            if (nodeDict.Count == 0)
            {
                DirtyNodeUpdate();
            }

            return (N)nodeDict.GetValueOrDefault(id);
        }


        /// <summary>
        /// Condition de suppression d'un node
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected virtual bool RemoveNodeCondition(params N[] nodes)
        {
            return true;
        }


        /// <summary>
        /// O(n), retourne le node à la position de l'écran 
        /// </summary>
        /// <param name="pos">Posdition à l'écran</param>
        /// <returns>Node à la position</returns>
        protected N GetNodeAtScreenPos(Vector2 pos)
        {
            foreach (var node in currentFile.GetNodes())
                if (node.LastScreenRect.Contains(pos)) return (N)node;
            return null;
        }


        /// <summary>
        /// Clamp la position à la grille
        /// </summary>
        /// <param name="pos">La position fixe du node (pas visuelle)</param>
        /// <returns>Position à l'écran fixé à la grille</returns>
        protected Vector2 ClampPosition(Vector2 pos)
        {
            return new Vector2(pos.x.Rnd(25), pos.y.Rnd(25));
        }


        /// <summary>
        /// Convertis la position visuelle en coordonnée solides dans le graph
        /// </summary>
        /// <param name="screenPosition">Position visuelle à convertir, exemple : mousePosition</param>
        /// <returns>Position convertie</returns>
        public Vector2 ScreenToGraphPosition(Vector2 screenPosition)
        {
            return (screenPosition - position.size * 0.5f) / zoomCoef - ViewOffset;
        }


        /// <summary>
        /// Applique le zoome et l'offset de navigation à la position
        /// /// </summary>
        /// <param name="screenPosition">Coordonnée à convertir, exemple : position d'un node</param>
        /// <returns>Position convertie</returns>
        public Vector2 GraphToScreenPosition(Vector2 graphPosition, bool clampToGrid = false)
        {
            if (clampToGrid) graphPosition = ClampPosition(graphPosition);

            var calcVec = graphPosition * zoomCoef + ViewOffset * zoomCoef + position.size * 0.5f;

            return calcVec;
        }




        /// <summary>
        /// Suprime les nodes de l'animator
        /// </summary>
        /// <param name="nodes">Les nodes à suprimer</param>
        /// <param name="dirtyUpdate">Faut-t-il faire un calcul en O(n²) pour checker/Update toute les references ? Ne pas activer pendant les ajout massifs de nodes, mais juste 1 fois apres.</param>
        protected void RemoveNodes(bool dirtyUpdate, params N[] nodes)
        {
            Undo.RecordObject(currentFile, "Remove Nodes");

            if (!RemoveNodeCondition(nodes))  //Si l'anim est en mode FolderOnlyEditoMode 
                return;


            var n = currentFile.GetNodes();
            n.RemoveAll((m) => nodes.Contains(m));
            currentFile.SetNodes(n);


            if (dirtyUpdate)
                DirtyNodeUpdate();

#if EDITOR_PROPERTY_TEST
            cachedEditor.serializedObject.Update();
#endif

        }


        /// <summary>
        /// Suprime les nodes de l'animator    
        /// </summary>
        /// <param name="nodes">Les nodes à suprimer</param>
        protected void RemoveNodes(params N[] nodes)
        {
            RemoveNodes(true, nodes);
        }




        /// <summary>
        /// Ajoute un nouveau node vide dans l'animator
        /// </summary>
        /// <param name="screenPosition">Position du node dans la preview (mouseposition?)</param>
        /// <param name="dirtyUpdate">Faut-t-il faire un calcul en O(n²) pour checker/Update toute les references ? Ne pas activer pendant les ajout massifs de nodes, mais juste 1 fois apres.</param>
        /// <param name="overrideDefaultType">Modifier le type par défaut des classes Node instanciés. Utile pour faire un code générik</param>
        /// <returns>Le node ajouté</returns>
        protected N AddEmptyNode(bool dirtyUpdate, Vector2 screenPosition, Type overrideDefaultType = null)
        {
            Undo.RecordObject(currentFile, "Add Node");

            // if (!TryAddNodeCondition(file))  //Si l'anim est en mode FolderOnlyEditoMode 
            //     return null;

            N node = null;



            if (overrideDefaultType == null)
            {
                if (NewNodesType != null)
                    overrideDefaultType = NewNodesType; //currentFile.NewNodeTypes();
                else
                    overrideDefaultType = typeof(N);
            }


            if (overrideDefaultType != null)
            {
                if (typeof(N).IsAssignableFrom(overrideDefaultType))
                    node = (N)Activator.CreateInstance(overrideDefaultType);
                else
                    Debug.LogError($"Le type {typeof(N)} n'est pas assignable depuis le type générique de la classe {overrideDefaultType}. Modifiez l'argument passé dans la fonciton.");
            }
            else
            {
                Debug.LogError("Node type nul!");
                return null;
            }

            /*
        if(node == null)
             node = new N();
            */

            node.EditorPosition = ScreenToGraphPosition(screenPosition);

            var n = currentFile.GetNodes();

            n.Add(node);
            currentFile.SetNodes(n);



            if (dirtyUpdate)
                DirtyNodeUpdate();

#if EDITOR_PROPERTY_TEST
            cachedEditor.serializedObject.Update();
#endif

            return node;
        }

        /// <summary>
        /// Ajoute un nouveau node vide dans l'animator
        /// </summary>
        /// <param name="screenPosition">Position du node dans la preview (mouseposition?)</param>
        private N AddEmptyNode(Vector2 screenPosition)
        {
            return AddEmptyNode(true, screenPosition);
        }

#endregion



        /// <summary>
        /// Rempli le menu Edit (click droit) pour les nodes
        /// </summary>
        /// <param name="menu">Le menu à remplir</param>
        /// <param name="mousePosition">position de la souris actuelle</param>
        /// <param name="path">Chemin où remplir le menu (genre ajouter Edit/ par exemple)</param>
        protected virtual void FillContextEditMenu(GenericMenu menu, Vector2 mousePosition, string path = "")
        {
            if (path.Length > 0 && path.Last() != '/') path += '/';

            menu.AddItem(new GUIContent($"{path}Add Empty Node"), () => { AddEmptyNode(true, mousePosition); }, false);

            menu.AddSeparator($"{path}");

            menu.AddItem(new GUIContent($"{path}Remove"), () => { RemoveNodes(selectedNodes.ToArray()); }, selectedNodes.Count == 0, false);
            if(mouseOver == MouseOver.EditArea)
            menu.AddShortcut(KeyCode.Delete);

            menu.AddItem(new GUIContent($"{path}Copy %C"), CopySelectedNodes, selectedNodes.Count == 0);
            menu.AddItem(new GUIContent($"{path}Paste %V"), () => PasteNodes(mousePosition), false);

            menu.AddSeparator($"{path}");
            menu.AddItem(new GUIContent($"{path}Select All"), false, () => selectedNodes.UnionWith(currentFile.GetNodes().Select(m => (N)m)));
            if(mouseOver == MouseOver.EditArea)
            menu.AddShortcut(KeyCode.A, EventModifiers.Control);

            menu.AddSeparator($"{path}");
            menu.AddItem(new GUIContent($"{path}Remove Connections"), () => Input_actualMousePinConnector.Transitions = new T[0], Input_actualMousePinConnector == null);




        }



        /// <summary>
        /// Parcoure et affiche les nodes à l'écran
        /// </summary>
        protected virtual void DrawNodes()
        {

            // if (needRepaint) return;

            //Debug center
            //Handles.DrawWireDisc(position.size * 0.5f, Vector3.forward, 10f);




            SerializedProperty prop;
            var nods = currentFile.GetNodes();


            Vector3 newPosition;



            List<KeyValuePair<NodeTransitionPin<T>, List<T>>> transitionsLists = new List<KeyValuePair<NodeTransitionPin<T>, List<T>>>();


            ///Draw transitions

            int nodId, pinId, TransId;
            IList<NodeTransitionPin<T>> pins;
            //T[] trans;

            for (nodId = 0; nodId < nods.Count; nodId++)
            {
                if (nods[nodId] == null)
                {
                    Debug.LogError("Null node found..");
                    currentFile.SetNodes(nods.SkipWhile(m => m == null).ToList());
                    break;
                }
            
                pins = nods[nodId].TransitionPins;

                for (pinId = 0; pinId < pins.Count; pinId++)
                {
                    // trans = pins[pinId].Transitions;
                    DrawPintransition(pins[pinId], EditorLineDesign);
                    /*
                    for (TransId = 0; TransId < trans.Count; TransId++)
                    {
                        DrawLine();
                    }*/
                }
            }





            //   Debug.Log($"Draw {SerializedNodeArrayProperty.arraySize} nodes");

#if EDITOR_PROPERTY_TEST
            
            for (int i = 0; i < SerializedNodeArrayProperty.arraySize; i++)
            {
                prop = SerializedNodeArrayProperty.GetArrayElementAtIndex(i);
                var obj = nods[i];

                newPosition = GraphToScreenPosition(obj.EditorPosition, ClampNodeToGrid);

                ApplyNodeStyle(obj);

                obj.DrawNode(newPosition, zoomCoef, prop);
            }
#endif


            for (int i = 0; i < nods.Count; i++)
            {
                var obj = nods[i];

                newPosition = GraphToScreenPosition(obj.EditorPosition, ClampNodeToGrid);

                ApplyNodeStyle(obj);

                obj.DrawNode(newPosition, zoomCoef, null);
            }
            




            if (selectedNodes.Count == 1 && lastSelectedNode != null)
            {
                pins = lastSelectedNode.TransitionPins;

                for (int i = 0; i < pins.Count; i++)
                {
                    if (!pins[i].OverlaySelectionMessage.IsNullOrEmpty())
                        DrawFlyingTextBox(new Vector2(pins[i].LastScreenPosition.xMax, pins[i].LastScreenPosition.yMin), pins[i].OverlaySelectionMessage);
                }
            }



        }



        /// <summary>
        /// Fonction appelée par <see cref="DrawNodes"/> pour modifier le style des nodes
        /// </summary>
      protected virtual void ApplyNodeStyle(N node)
        {
            node.NodeStyle = selectedNodes.Contains(node) ? NodeStyleDefaultSelected : NodeStyleDefault;
        }

    }

}
#endif
