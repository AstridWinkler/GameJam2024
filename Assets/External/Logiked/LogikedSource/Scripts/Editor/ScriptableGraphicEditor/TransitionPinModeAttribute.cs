using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logiked.source.graphNode
{

    /// <summary>
    /// Modification du comportement des <see cref="NodeTransitionPin"/>, pour l'editeur visuel. Permet de limiter le nombre de liaisons d'un pin, etc.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple =false)]
    public class TransitionPinModeAttribute : Attribute
    {

        /// <summary>
        /// Autoriser plusiers connexion depuis ce pin ?
        /// </summary>
        public bool AllowMultipeTransitions => allowMultipeTransitions;
        private bool allowMultipeTransitions = true;

        /// <summary>
        /// Message affiché au dessus du pin quand le node est selectionné
        /// </summary>
        public string OverlaySelectionText => overlaySelectionText;
        private string overlaySelectionText;
        public TransitionPinModeAttribute(bool allowMultipeTransitions, string overlaySelectionText = null)
        {
            this.allowMultipeTransitions = allowMultipeTransitions;
            this.overlaySelectionText = overlaySelectionText;
        }
        // public Type destinationType = true;



    }
}
