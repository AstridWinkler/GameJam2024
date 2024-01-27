using logiked.source.attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using logiked.source.editor;
using static logiked.source.editor.GUILogiked.Panels;
#endif

namespace logiked.source.graphNode
{

    /// <inheritdoc/>
    [Serializable]
    public class NodeTransitionPin : NodeTransitionPin<NodeTransition> { }

    public interface INodeTransitionPin
    {
#if UNITY_EDITOR

        /// <summary>
        /// Appliquer les attibuts de cette transitions.
        /// </summary>
        /// <param name="attribute">Les attributs définis</param>
        public void ApplyAttributes(TransitionPinModeAttribute attribute);
#endif
    }


    /// <summary>
    /// Point d'attache de connexion entre deux nodes <br></br>
    /// Utiliser <see cref="TransitionPinModeAttribute"/> pour spécifier les paramètres du boutton (interdire les connexion mutliples, (wip)pin de destination...)
    /// </summary>
    [Serializable]
    public class NodeTransitionPin<T> : INodeTransitionPin where T : NodeTransition
    {
        /*
        public NodeTransitionPin(GraphicNode<T> parentNode)
        {
            this.parentNode = parentNode;
        }*/

        public NodeTransitionPin() {
        }



#if UNITY_EDITOR
    public Rect LastScreenPosition => lastScreenPosition;
        [NonSerialized] private Rect lastScreenPosition;
#endif


        public bool AllowMultipleTransitions { get => allowMultipleTransition; set => allowMultipleTransition = value; }
        [SerializeField][GreyedField][Tooltip("Use attribute \"TransitionPinModeAttribute(...)\" on the pin field to modify this bool.")]
        private bool allowMultipleTransition = true;
        [SerializeField]
        [GreyedField]
        [Tooltip("Use attribute \"TransitionPinModeAttribute(...)\" on the pin field to modify this string.")]
        private string overlaySelectionMessage = null;
        public string OverlaySelectionMessage { get => overlaySelectionMessage; set => overlaySelectionMessage = value; }

        /*
        [SerializeReference]
        private GraphicNode<T> parentNode;
        public GraphicNode<T> ParentNode => parentNode;*/


        [SerializeField]
        private List<T> transitions = new List<T>();
       
        /// <summary>
        /// Liste des transitions sortantes de ce pin. Utilisation dun array pour controller le nombre d'entrée et ne pas permettre une modification en-place de l'exterieur.
        /// </summary>
        public T[] Transitions
        {
            get => transitions.ToArray();
            set
            {
                if (allowMultipleTransition)
                    transitions = value.ToList();
                else
                    transitions = new List< T>() { value.LastOrDefault() };
            }
        }

        public void AddTransition(T transition)
        {
            if (allowMultipleTransition)
                transitions.Add(transition);
            else
                transitions = new List<T>() { transition };
        }
        public void RemoveTransition(int destinationId)
        {
            transitions.RemoveAll(m => m.NextNodeId == destinationId);
        }

        public bool RemoveTransition(T transition)
        {
           return transitions.Remove(transition);
        }

        public void RemoveAllTransitions()
        {
             transitions.Clear();
        }



        //   public List<NodeTransition> GetTransitions() => transitions.Cast<NodeTransition>().ToList();


        public NodeTransitionPin<T> Clone()
        {
            NodeTransitionPin<T> end = (NodeTransitionPin < T > )this.MemberwiseClone();
            end.transitions = transitions.Select(e => (T)e.Clonetransition()).ToList();

            return end;
        }

        /// <summary>
        /// Todo : node type connection
        /// </summary>


        /// <summary>
        /// Todo : Unique Pin ID
        /// </summary>





#if UNITY_EDITOR
        [NonSerialized]
        public bool IsPointerOver;
      
        [NonSerialized]
        public bool isMouseInsideDragLinkTransition;
        //[NonSerialized]
        //public bool AllowMultiple;


        ///<inheritdoc/>
        public void ApplyAttributes(TransitionPinModeAttribute attribute)
        {
         //   Debug.Log("Attribute applied : "+ attribute.);

            allowMultipleTransition = attribute.AllowMultipeTransitions;
            overlaySelectionMessage = attribute.OverlaySelectionText;

        }

        public virtual void DrawConnexionPin(float zoomIconSize, Vector2 screenPosition, EditorIconType iconType = EditorIconType.NodeConnectBubble2, float iconSize = 20f)
        {

          //  if (!checkAttributes)
          //      CheckAttributes();


            var DragIconRect = new Rect();
            DragIconRect.width = iconSize * zoomIconSize;
            DragIconRect.height = iconSize * zoomIconSize;
            //  Vector2 positionIcon = new Vector2(screenPosition.x - DragIconRect.width * 0.5f, screenPosition.y - DragIconRect.height * 0.5f);
            Vector2 positionIcon = screenPosition;
            positionIcon.x -= 8;
            positionIcon.y -= 8;
        
            DragIconRect.center = positionIcon;


            lastScreenPosition = DragIconRect;

            //Place Drag Icon on the bottom Right of the node

            var DragIconRectCast = DragIconRect;
            DragIconRectCast.size *= 1.5f;
            DragIconRectCast.center = DragIconRect.center;


            if (GUI.enabled)
            {
                GUILogiked.Panels.GUIDrawEditorIcon(iconType, DragIconRect, "Drag to create new transition");
                isMouseInsideDragLinkTransition = DragIconRectCast.Contains(Event.current.mousePosition);

                if (isMouseInsideDragLinkTransition)
                    GUI.color = Color.grey;
            }
            else
            {
                var g = GUI.enabled;
                GUI.enabled = true;

                GUILogiked.Panels.GUIDrawEditorIcon(iconType, DragIconRect);
            

                var rect2 = DragIconRect;
                rect2.width -= 2f * zoomIconSize;
                rect2.height -= 2f * zoomIconSize;
                rect2.center = DragIconRect.center;
                rect2.x -= zoomIconSize * 5f;
                GUILogiked.Panels.GUIDrawEditorIcon(EditorIconType.LockerDark, rect2);
                GUI.enabled = false;


            }

            GUI.color = Color.white;
        }
#endif

    }


 

}
