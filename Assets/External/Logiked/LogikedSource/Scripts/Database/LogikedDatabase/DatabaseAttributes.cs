using UnityEngine;
using logiked.source.database;
using logiked.source.utilities;
using logiked.source.attributes;
using System;
using logiked.source.attributes.root;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif

namespace logiked.source.database
{
    /// <summary>
    /// Layer pour pouvoir gerer un champ categorie sur les objets de la bdd
    /// </summary>
    public class DatabaseCategoryNumberAttribue : FutureFieldAttribute
    {

#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            var obj = (DatabaseAbstractElement)property.serializedObject.targetObject;



            if (obj.Database != null && obj.Database.ItemCategories.Length > 0)
                property.intValue = EditorGUI.Popup(position, label, property.intValue, System.Array.ConvertAll(obj.Database.ItemCategories, m => new GUIContent(m)));
            else
            // property.intValue = EditorGUI.IntField(position, label, property.intValue);
            {
                var guival = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.MaskField(position, label, property.intValue, new string[] { "Nothing" });
                GUI.enabled = guival;
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);
        }

        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return SIZE_LINE;
        }




#endif
    }

    /// <summary>
    /// Layer pour pouvoir gerer des tags des elements de la database
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DatabaseTagNumberAttribute : FutureFieldAttribute
    {



#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            var obj = (DatabaseAbstractElement)property.serializedObject.targetObject;

            if (obj.Database != null && obj.Database.ItemTags.Length > 0)
                property.intValue = EditorGUI.MaskField(position, label, property.intValue, obj.Database.ItemTags);
            else
            {
                var guival = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.MaskField(position, label, property.intValue, new string[] { "Nothing"});
                GUI.enabled = guival;
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);


            //property.intValue = EditorGUI.IntField(position, label, property.intValue);

        }

        public override float GetPropertyLocalHeight(SerializedProperty property, GUIContent label)
        {
            return SIZE_LINE;
        }


#endif

    }


    /// <summary>
    /// [WIP] A mettre sur les champs des elements d'une BDD. Permet de les masquer si l'option "MaskField" de la catégorie est activée
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DatabaseCanMaskField : FutureFieldAttribute
    {
        /// <summary>
        /// Si il faut masquer la propriétée on met ce drawer en principal
        /// </summary>
        bool mask;

        public DatabaseCanMaskField( )
        {
            base.order = -100;
        }





#if UNITY_EDITOR

        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            // Debug.LogError(property.serializedObject.targetObject);
            DatabaseAbstractElement elem = property.serializedObject.targetObject as DatabaseAbstractElement;

            if (elem == null)// || !typeof(DatabaseAbstractElement).IsAssignableFrom(p.GetTypeReflection()))
            {
                Debug.LogError("Need to be on a field in a DatabaseAbstractElement");
                return;
            }

            mask = elem.Database != null && elem.Database.GetCategory(elem.CategoryId).MaskProperties;

            if (!mask)
            {
                CallNextAttribute(position, property, label);
            }


        }

    

#endif
    }

}




