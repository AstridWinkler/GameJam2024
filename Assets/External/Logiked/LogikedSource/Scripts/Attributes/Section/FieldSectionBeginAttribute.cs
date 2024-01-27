using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logiked.source.attributes
{


#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
#endif


    /// <summary>
    /// [Décorateur qui se dessine en premier]<br/> Ouvre une nouvelle section avec un titre pour l'organisation des champs dans l'inspector.<br/> 
    /// Ne fonctionne pas sur les arrays !!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldSectionBegin : FieldSectionGui
    {
        public FieldSectionBegin(string title = "", bool doubleSection = false) : base(doubleSection ? FieldSectionType.NewSectionDouble : FieldSectionType.NewSection, title)
        {
            order = -15;
        }

        public override AttributeArrayInteraction ArrayInteractionMode => AttributeArrayInteraction.ApplyOnFirstElement;




#if UNITY_EDITOR

        public override float GetPropertyHeightRecursive(SerializedProperty property, GUIContent label)
        {

            return (property.depth == 0) ? SIZE_LINE + GetContext(property).AttributeHeight: 32 + (base.GetPropertyHeightRecursive(property, label));
            //  return base.GetPropertyHeightRecursive(property, label);

            // GetContext(property).GetData<float>("SectionHeight");
            //Cest le GuiLayout et le GetProperty.base qui gère réelement la taille de la section
        }

     




#endif
    }


}
