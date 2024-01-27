using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace logiked.source.attributes
{


    /// <summary>
    /// Ferme une section<br/> 
    /// [Décorateur qui se dessine aprés la propriété de base] <br/>
    /// Ne fonctionne pas sur les arrays !!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldSectionEnd : FieldSectionGui
    {

        public override AttributeArrayInteraction ArrayInteractionMode => AttributeArrayInteraction.ApplyOnLastElement;


        public FieldSectionEnd() : base(sectionType: FieldSectionType.CloseSection)
        {
           order = -10;
        }


    }
}
