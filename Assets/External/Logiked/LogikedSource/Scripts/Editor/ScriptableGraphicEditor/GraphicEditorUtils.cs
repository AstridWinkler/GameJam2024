
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.source.graphNode
{

    public static class GraphNodeUtils
    {

        #region Enums


        /// <summary>
        /// Mode d'édition actuel de la fenètre
        /// </summary>
        public enum EditMode
        {
            /// <summary>
            /// Aucun fichier n'est édité
            /// </summary>
            NoFile,
            /// <summary>
            /// Un fichier est en train d'étre édité
            /// </summary>
            Editing
        }

        /// <summary>
        /// Modes d'affichages des connections visuelles dans l'editeur
        /// </summary>
        public enum NodeLineDesign
        {
            /// <summary>
            /// Tracés en lignes droites
            /// </summary>
            Linear = 0,
            /// <summary>
            /// Tracés design en courbes de bezier 
            /// </summary>
            SexyBezier = 1
        }

        /// <summary>
        /// Mode d'affichage pour les courbes de Bezier de <see cref="NodeLineDesign.SexyBezier"/>
        /// </summary>
        public enum BezierGrowDirection
        {
            /// <summary>
            /// Aficher avec une tangeante à gauche
            /// </summary>
            Left = 0x0001,

            /// <summary>
            /// Aficher avec une tangeante à Droite
            /// </summary>
            Right = 0x0010,


            /// <summary>
            /// Aficher avec une tangeante en haut
            /// </summary>
            Top = 0x0100,

            /// <summary>
            /// Aficher avec une tangeante en bas
            /// </summary>
            Bottom = 0x1000,

            /// <summary>
            /// Aficher avec une tangeante à l'horizontal
            /// </summary>
            Horizontal = 0x0011,
            /// <summary>
            /// Aficher avec une tangeante à la vertical
            /// </summary>
            Vertical = 0x1100,
        }


        /// <summary>
        /// Position de la souris 
        /// </summary>
        public enum MouseOver
        {
            /// <summary>
            /// Au dessus de l'édition de Nodes
            /// </summary>
            EditArea,
            /// <summary>
            /// Au dessus d'une fenètre quelconque, qui à un rectangle enregistré dans <see cref="inspectorsRect"/>
            /// </summary>
            Inspector
        }

    }
    #endregion

}



#endif
