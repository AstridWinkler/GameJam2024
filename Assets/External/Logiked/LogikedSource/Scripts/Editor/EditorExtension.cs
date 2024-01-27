#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;


namespace logiked.source.editor
{
    /// <summary>
    /// [Logiked]Fonction d'extension pour l'editeur et l'inspecteur
    /// </summary>
    public static class EditorExtension
    {
        /// <summary>
        /// Permet de get la valeur d'un champ SerializedProperty en object
        /// </summary>
        // Gets value from SerializedProperty - even if value is nested
        public static object GetValue(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            if (obj == null)
            {

                Debug.LogError("inpossible de set la valeur");
                return null;
            }

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                obj = field?.GetValue(obj);
            }
            return obj;
        }

        /// <summary>
        /// Permet de set la valeur d'un champ SerializedProperty un object
        /// </summary>
        // Sets value from SerializedProperty - even if value is nested
        public static void SetValue(this SerializedProperty property, object val)
        {
            object obj = property.serializedObject.targetObject;

            List<KeyValuePair<FieldInfo, object>> list = new List<KeyValuePair<FieldInfo, object>>();

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                list.Add(new KeyValuePair<FieldInfo, object>(field, obj));
                obj = field.GetValue(obj);
            }

            // Now set values of all objects, from child to parent
            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].Key.SetValue(list[i].Value, val);
                // New 'val' object will be parent of current 'val' object
                val = list[i].Value;
            }
        }

        /// <summary>
        /// Raccourcis vers les fonctions <see cref="GenericMenu.AddItem"/> et  <see cref="GenericMenu.AddDisabledItem"/>
        /// </summary>
        /// <param name="menu">Le menu à modifier</param>
        /// <param name="content">Nom de la fonction</param>
        /// <param name="isDisabledMenu">L'entrée est-t-elle désactivée ?</param>
        /// <param name="checkmark">Il y à t-il un checkmark devant l'entrée ?</param>
        /// <param name="func">Fonction à appeler quand on clic sur l'entrée</param>
        public static void AddItem(this GenericMenu menu, GUIContent content, GenericMenu.MenuFunction func, bool isDisabledMenu = false, bool checkmark = false)
        {
            UnityEngine.Assertions.Assert.IsNotNull(menu, "AddItem can't be called on a NULL menu !");

            if (isDisabledMenu)
                menu.AddDisabledItem(content, checkmark);
            else
                menu.AddItem(content, checkmark, func);
        }

        /// <summary>
        /// Raccourcis vers les fonctions <see cref="GenericMenu.AddItem"/> et  <see cref="GenericMenu.AddDisabledItem"/>
        /// </summary>
        /// <param name="menu">Le menu à modifier</param>
        /// <param name="content">Nom de la fonction</param>
        /// <param name="isDisabledMenu">L'entrée est-t-elle désactivée ?</param>
        /// <param name="checkmark">Il y à t-il un checkmark devant l'entrée ?</param>
        /// <param name="func">Fonction à appeler quand on clic sur l'entrée</param>
        public static void AddItem(this GenericMenu menu, GUIContent content, GenericMenu.MenuFunction2 func, object obj, bool isDisabledMenu = false, bool checkmark = false)
        {
            UnityEngine.Assertions.Assert.IsNotNull(menu, "AddItem can't be called on a NULL menu !");

            if (isDisabledMenu)
                menu.AddDisabledItem(content, checkmark);
            else
                menu.AddItem(content, checkmark, func, obj);
        }




    }

}

#endif // UNITY_EDITOR
