using logiked.source.attributes.root;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using logiked.source.extentions;
using System.Collections;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Liste de selection selon un array de String, dont le nom est à préciser en paramètre.
    /// </summary>
    public class PopupFieldAttribute : FutureFieldAttribute
    {
        string fieldName;


        /// <param name="fieldName">Le nom du champ/propriété qui sert de liste de string, relatif à cet objet.</param>
        public PopupFieldAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }





#if UNITY_EDITOR


        protected override void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context)
        {

            var reflect = property.GetParent().GetReflectedValueAtPath(fieldName);
            object currentValue = null;

            Context.AttributeHeight = SIZE_LINE;
            position.height = SIZE_LINE;
            Context.AttributeHeight = SIZE_LINE;

            if (reflect != null)
            {
                currentValue = reflect.Value;
            }


            void ShowErr(string msg)
            {

                Context.AttributeHeight =  2f * SIZE_LINE;


                position.height = Context.AttributeHeight;

                GUI.Label(position, label, EditorStyles.label);
                position.x += EditorGUIUtility.labelWidth;
                position.width -= EditorGUIUtility.labelWidth;
                GUI.Box(position, msg, GUILogiked.Styles.Box_ErrorBox1);


         
                property.intValue = 0;

            }



            if (currentValue == null)
            {
                ShowErr($"La propriété '{fieldName}' retourne NULL. Vérifier l'attribut {typeof(PopupFieldAttribute).Name} de la propiété {property.name}");
            }
            else
            {
                IEnumerable enumerable = currentValue as IEnumerable;

                if (enumerable == null)
                {
                    ShowErr($"La propriété '{fieldName}' ne peut pas etre convertie en liste de string !");
                }
                else
                {

                    List<string> convert = new List<string>();

                    
       
                    foreach (var e in enumerable)
                    {
                        convert.Add(e.ToString());
                    }

                    if (convert.Count == 0)
                    {
                        ShowErr($"La liste de string '{fieldName}' ne contient aucun élément !");
                    }
                    else
                    {
                        property.intValue = EditorGUI.Popup(position, label, property.intValue, convert.Select(c => new GUIContent(c)).ToArray());
                    }
                }
            }

            propertyAlreadyDrawn = true;
            CallNextAttribute(position, property, label);


        }
#endif
    }

}

