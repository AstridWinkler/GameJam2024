using System;
using UnityEngine;
using logiked.source.extentions;
using logiked.source.attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif
namespace logiked.source.graphNode
{


    /// <inheritdoc/>
    [Serializable]
    public class GraphicNode : GraphicNode<NodeTransition>
    {
        public GraphicNode() : this(new Vector2(150, 75)) { }
        public GraphicNode(Vector2 rectSize) : this(rectSize, new System.Random().Next()) { }
        public GraphicNode(Vector2 rectSize, int wrapperId) : base(rectSize, wrapperId) { }


        public virtual NodeTransitionPin[] TransitionPinsProp { get => new NodeTransitionPin[0]; }
        //set; }
        //public override NodeTransition[] AllTransitions => TransitionPinsProp.SelectMany(m => m.Transitions).ToArray();



        public sealed override NodeTransitionPin<NodeTransition>[] TransitionPins
        {
            get => TransitionPinsProp?.Select(m => (NodeTransitionPin<NodeTransition>)m).ToArray() ?? new NodeTransitionPin<NodeTransition>[0];
          //  set => TransitionPinsProp = value.Cast<NodeTransitionPin>().ToList();
        }

    }





    /// <summary>
    /// Generic GraphicNode wich handle connexion between nodes. <br></br>
    /// </summary>
    /// <typeparam name="T">Transitions types. Set to <see cref="NodeTransition"/> for default usage</typeparam>
    [Serializable]
    public abstract class GraphicNode<T>  where T : NodeTransition
    {
        // public abstract T[] AllTransitions { get; }
         public T[] AllTransitions { get => TransitionPins.SelectMany(m => m.Transitions).ToArray(); }
        public abstract NodeTransitionPin<T>[] TransitionPins { get; }// set; }



        /// <summary>
        /// Duplique le node en surface, avec les même membres. Duplique les transitions
        /// </summary>
        /// <param name="newNodeId">Unique ID du nouveau node généré</param>
        /// <returns>Le nouveau node</returns>
        public GraphicNode<T> Clone(int newNodeId)
        {
             var obj = (GraphicNode<T>)MemberwiseClone();

           var type = this.GetType();

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.DeclaredOnly | BindingFlags.FlattenHierarchy);

            Debug.Log(fields.Length);

            fields = fields.Where(m => m.FieldType.Is<NodeTransitionPin<T>>() ).ToArray();


            fields.ForEach( f => f.SetValue(obj, ( f.GetValue(obj) as NodeTransitionPin<T>).Clone() ) );

            Debug.Log($"{fields.Count()} pins replaced");

            /*
            for (int i = 0; i < TransitionPins.Length; i++)
                TransitionPins[i] = TransitionPins[i].Clone();
            */

            // var obj = (GraphicNode<T>)this.SerializeToByteArray().DeserializeToObject();

            obj.uniqueNodeId = newNodeId;

            OnBeingCloned();

            return obj;
        }

        /// <summary>
        /// Fonction appelée lorseque le node est cloné. Permet de modifier les champs de votre coté afin de gerer les dépendances.
        /// </summary>
        public virtual void OnBeingCloned() { }



        [NonSerialized]
        bool pinAttributesSet = false;


#if UNITY_EDITOR

        /// <summary>
        /// Applique les attribts <see cref="TransitionPinModeAttribute"/> sur les pins définis dans ce node
        /// </summary>
        public void UpdatePinAttributes()
        {
            if (pinAttributesSet) return;
            pinAttributesSet = true;



            var props = this.GetType().GetFields( BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default).Where(m => m.FieldType.Is<INodeTransitionPin>() );

            

            foreach (FieldInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes( typeof(TransitionPinModeAttribute)  ,true);
             
                foreach (object attr in attrs)
                {
                    TransitionPinModeAttribute authAttr = attr as TransitionPinModeAttribute;
                
                    if (authAttr != null)
                    {
                        var val = ((INodeTransitionPin)prop.GetValue(this));
                        if(val != null)
                            val.ApplyAttributes(authAttr); 
                    }
                }
            }
        }
#endif



        #region CONSTRUCTORS
        public GraphicNode(Vector2 rectSize) : this(rectSize, new System.Random().Next()) { }
        public GraphicNode(Vector2 rectSize, int wrapperId)
        {
#if UNITY_EDITOR
            RectSize = rectSize;
#endif
            uniqueNodeId = wrapperId;
            //UpdatePinAttributes();
        }

        #endregion




        #region FIELDS - Editor 


        [GreyedField]
        [SerializeField] protected int uniqueNodeId;
        /// <summary>
        /// Id Unique du node
        /// </summary>
        public int UniqueNodeId { get => uniqueNodeId; }// set => uniqueNodeId = value; }

        [SerializeField] public string text;



#if UNITY_EDITOR


        /// <summary>
        /// L'editeur actuellement utilisé pour modifier les nodes
        /// </summary>
        [NonSerialized]
        public Logiked_EditorWindow CurrentEditor;


        /// <summary>
        /// Position dans le repère Graph-Editor
        /// </summary>
        [SerializeField] protected Vector2 editorPosition;

        /// <summary>
        /// Taille du rectangle dans le repère Graph-Editor
        /// </summary>
        [SerializeField]
        protected Vector2 RectSize = new Vector2(150, 75);


        /// <summary>
        /// Derniere position visuelle où l'objet à été tracé à l'écran (dans le repère où le zoom et l'offet de la vue est prise en compte)
        /// </summary>
        public Vector2 LastScreenPosition { get; private set; }
      
        /// <summary>
        /// Derniere position visuelle où l'objet à été tracé à l'écran (dans le repère où le zoom et l'offet de la vue est prise en compte)
        /// </summary>
        public Rect LastScreenRect { get; private set; }

        /// <summary>
        /// Style actuel du node. Utiliser les styles Unity par défaut disponibles dans <see cref="GUILogiked.StylesNodeBox"/>, et pour une prise en compte du zoome, les passer au préalable dans <see cref="ScripableObjectGraphicEditor{T, N}.RegisterFontForZoomScaling(GUIStyle)"/>
        /// </summary>
        public GUIStyle NodeStyle { get => nodeStyle; set => nodeStyle = value; }
       
        [NonSerialized]
        private GUIStyle nodeStyle;


        /// <summary>
        /// Position solide du centre du node dans l'éditeur (dans un repère sans zoom ni offset)
        /// </summary>
        public Vector2 EditorPosition
        {
            get { return editorPosition; }
            set { editorPosition = value; }
        }

#endif

        #endregion

#if UNITY_EDITOR

        #region METHODS - Utils

        /// <summary>
        /// Retourne un point sur le rectangle du node en fonction d'une direction. pratique pour pour faire des flèches design qui partent du node.
        /// </summary>
        /// <param name="dir">Direction du point d'attache par rapport au centre du node</param>
        /// <returns>Point d'attache du node</returns>
        public Vector2 GetVisualAttachPoint(Vector2 dir)
        {
            var pos = LastScreenPosition;

            if (dir.x.Abs() > dir.y.Abs())
                pos.x += dir.x.Sign() * (-10 + LastScreenRect.width / 2f);      //-10 to put move point inside the node
            else
                pos.y += dir.y.Sign() * (-10 + LastScreenRect.height / 2f);

            return pos;
        }


 

        #endregion

        #region METHODS - Render & Scaling


        /// <summary>
        /// Retourne la taille du rectangle à l'écran en fonction du zoom
        /// </summary>
        /// <param name="zoomSize"></param>
        /// <returns></returns>
        private Vector2 GetScreenRectSize(float zoomSize = 1f)
        {
            return RectSize * zoomSize;
        }

        /// <summary>
        /// Retourne la position à l'écran du node 
        /// </summary>
        /// <param name="zoomSize"></param>
        /// <returns></returns>
        private Rect GetScreenRect(float zoomSize)
        {
            return new Rect(LastScreenPosition - GetScreenRectSize(zoomSize) * .5f, GetScreenRectSize(zoomSize));
        }   


        /// <summary>
        /// Applique un zoom et une position aux coordonnées d'affichage
        /// </summary>
        /// <param name="newScreenPosition">La position du node à l'écran</param>
        /// <param name="zoomSize">Le zoom de la caméra</param>
        public void UpdateLastScreenCoordinates(Vector3 newScreenPosition, float zoomSize)
        {
            LastScreenPosition = newScreenPosition;
            LastScreenRect = GetScreenRect(zoomSize);
        }



        /// <summary>
        /// Fonction de dessin du node. Pour info il faut utiliser le serialized property pour modifier les valeurs.
        /// Et elles seront updates de manières super propres !
        /// </summary>
        /// <param name="zoomSize">La vue dans la fenettre</param>
        /// <param name="serialized">Cet objet sérializé</param>
        public virtual void DrawNode(Vector3 newScreenPosition, float zoomSize, SerializedProperty serialized)
        {
            UpdateLastScreenCoordinates(newScreenPosition, zoomSize);

            if (nodeStyle == null)
                nodeStyle = GUILogiked.StylesNodeBox.NodeStyleDefault;

            if (text.IsNullOrEmpty())
                text = "node";

            //Le GUI style de la box fait de la merde a cause de ses bordures
            //Debug qui revele le problème :
            //Handles.DrawSolidRectangleWithOutline(LastScreenRect, Color.red, Color.red);

            GUI.Box(LastScreenRect, text, nodeStyle);

        }


        /*
         [Test because Unity Default GuiStyle for node have transparent Borders]
        public Rect GuiStyleFixSize(Rect ScreenRect)
        {
            Vector2 newSize = ScreenRect.size;

            return ScreenRect;

            ScreenRect.width += 16;
            ScreenRect.height += 16;

            ScreenRect.x -= 8;
            ScreenRect.y -= 8;

            return ScreenRect;
        }*/

        #endregion

#endif



    }
}




