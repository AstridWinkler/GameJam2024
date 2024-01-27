using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;



namespace logiked.source.extentions
{
    /// <summary>
    /// Event Editeur qui permet de g�rer la supression de l'objet (eg: Pour suprimer des fichiers annexes automatiquement par exemple)
    /// Appel� par reflection � la supression du Scriptable Object de cette propri�t�, par l'exention ScriptableObject <see cref="RemoveScriptableObjectExtention.OnRemoveScriptableObject(ScriptableObject)"/>
    /// </summary>
    public interface OnRemoveObjectEvent
    {
#if UNITY_EDITOR
        /// <summary>
        /// [EDITOR ONLY] Fonction appel�e � la suppression du ScriptableObject associ�, via <see cref="RemoveScriptableObjectExtention.OnRemoveScriptableObject(ScriptableObject)"/>
        /// </summary>
        public void OnRemoveObjectEvent();
#endif

    }

    /// <summary>
    /// Extention pour la spression des scriptable objects
    /// </summary>
    public static class RemoveScriptableObjectExtention
    {

        /// <summary>
        /// [EDITOR ONLY] Fonction d'extention � appeler sur les ScriptableObject � supprimer
        /// </summary>
        public static void OnRemoveScriptableObject(this ScriptableObject obj)
        {
#if UNITY_EDITOR

            var t = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance);
            Type typ = typeof(OnRemoveObjectEvent);
            object val;

            for (int i = 0; i < t.Length; i++)
            {
                if (t[i].FieldType.GetInterfaces().Contains(typ))
                {
                    val = t[i].GetValue(obj);
                    typ.GetMethod("OnRemoveObjectEvent", BindingFlags.Public | BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Instance).Invoke(val, new object[0]);
                }
            }

            // Debug.LogError(t.ToStringArray());


#endif



        }

    }

}