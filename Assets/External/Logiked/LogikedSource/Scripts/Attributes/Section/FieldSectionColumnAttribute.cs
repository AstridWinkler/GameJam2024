using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace logiked.source.attributes
{

    

    /// <summary>
    /// [Décorateur qui se dessine avant la propriété] Ajoute une colonne à la section actuelle<br/> 
    /// Ne fonctionne pas sur les arrays !!
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FieldSectionColumn : FieldSectionGui
    {
        public FieldSectionColumn() : base(FieldSectionType.SectionSwitchColumn) { }

#if UNITY_EDITOR

        public override float GetPropertyHeightRecursive(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

#endif

    }



}
