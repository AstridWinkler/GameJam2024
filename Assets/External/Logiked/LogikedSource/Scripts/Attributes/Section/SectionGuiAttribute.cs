using UnityEngine;
using System;
using System.Reflection;
using logiked.source.attributes.root;
using logiked.source.extentions;

#if UNITY_EDITOR
using UnityEditor;
using logiked.source.editor;
#endif


namespace logiked.source.attributes
{




    /// <summary>
    /// Classe abstraite pour des décorateur de section. Voir <seealso cref="FieldSectionBegin"/>,  <seealso cref="FieldSectionEnd"/> et <seealso cref="FieldSectionColumn"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Property)]
    abstract public class FieldSectionGui : FutureFieldAttribute
    {

   
        internal static GUIStyle BoxStyle
        {
            get
            {
                if (boxStyle == null)
                {
                    boxStyle = new GUIStyle("box");
                    boxStyle.margin = new RectOffset(-25,10, boxStyle.margin.top, boxStyle.margin.bottom);
                    boxStyle.padding = new RectOffset(15,10, boxStyle.padding.top, boxStyle.padding.bottom);
                }
                return boxStyle;
            }
        }
        internal static GUIStyle boxStyle;


        /// <summary>
        /// Type de section (ouvrir/Fermer/Nouveau panneau)
        /// </summary>
        internal enum FieldSectionType { NewSection = 1, CloseSection = 2, SectionSwitchColumn = 4, NewSectionDouble=8 }
        internal FieldSectionType type;

        private string title;


        /// <summary>
        /// Compteur statique qui valide le nombre d'ouverture/fermetures de sections.
        /// </summary>
        private static int validator;
        private static string lastProperty;


        internal FieldSectionGui(FieldSectionType sectionType, string title = "")
        {
            this.type = sectionType; 
            this.title = title;
        }
        public FieldSectionGui(string title = "") : this(FieldSectionType.NewSection, title) { }


#if UNITY_EDITOR

        private Rect position;
        private SerializedProperty property;
        private GUIContent label;
        private AttributeContext context;





        bool CheckBugs(int indentLevel)
        {
        return indentLevel < 0 || indentLevel > 5;      
        }


         static int sec = 10;


        void PrintAndFixBugs()
        {
            Debug.LogWarning(validator+ " Error"); ;
            string text = "Error on <b>{0}.{1}</b> : {2}".Format(property.serializedObject.targetObject.name, lastProperty, validator > 0 ? "FieldSectionBegin attribute called but never closed!" : "FieldSectionEnd attribute called but never open!"); ;

        
            while (validator > 0)
            {
                ChangeSection(false, true);
                validator--;
            }

            while (validator < 0)
            {
                ChangeSection(true, true);
                validator++;
            }




            while (validator != 0 && sec-- >0)
            {
              //  ChangeSection(validator < 0);
            }

            if (sec == 0)
                Debug.LogError("Stack overflow");
            sec = 10;


            Debug.LogError(text);
            position.height = 30;
            EditorGUILayout.Space(20);


            EditorGUI.HelpBox(position, text, MessageType.Error);

        }





        void ChangeSection(bool open , bool repair = false)
        {

            void SecModif(bool open)
            {
                if (open)
                {
                    GUI.color = Color.grey;
                    Vector2 scroll;
                    scroll = context.GetData<Vector2>(nameof(scroll));
                    scroll = EditorGUILayout.BeginScrollView(scroll );


                    if (type == FieldSectionType.NewSectionDouble)
                    {
                     //   GUILayoutOption[] options = { GUILayout.MaxWidth(200.0f), GUILayout.MinWidth(200.0f), GUILayout.ExpandWidth(true)};

                       GUILayout.BeginHorizontal(BoxStyle);
                        GUILayout.BeginVertical();
              //          EditorGUIUtility.labelWidth = 100;
               //         EditorGUIUtility.fieldWidth = 100;
                    }
                    else
                    {
                        GUILayout.BeginHorizontal(BoxStyle);
                        GUILayout.BeginVertical();
                    }


                    context.SetData<Vector2>(nameof(scroll), scroll);
                    GUI.color = Color.white;

                }
                else
                {
                    EditorGUILayout.EndVertical();
   
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndScrollView();

                    GUILayout.Space(10);

                }
            }


            if (repair)
                Debug.LogError(validator + "Fix");

            if (open)
            {

                lastProperty = property.name;

                //Debug.Log("Open");

                /*
                if (CheckBugs(validator + 1))
                {
                    Debug.Log(validator + " checkbug");

                    PrintAndFixBugs();
                    return;
                }
                */



                validator++;
                SecModif(true);



                // context.SetData<float>("SectionHeight", height);



            }
            else
            {
                lastProperty = property.name;




                //Debug.Log("close");


                /*

                if (CheckBugs(validator-1))
                {
                    Debug.Log(validator + " checkbug");

                    PrintAndFixBugs();
                    return;
                }
                */




              


                if (validator <= 0)
                {
                    string text = $"Error on <b>{property.serializedObject.targetObject.name}.{lastProperty}</b> : FieldSectionEnd attribute called but never open! Nested Sections : {validator}";
                    Debug.LogError(text);
                    validator = 0;
                }
                else
                {
                    validator--;
                    SecModif(false);
                }






            }
}


        protected override void OnGUIRecursive(Rect nextPosition, SerializedProperty property, GUIContent label, AttributeContext Context)
        {



            this.property = property;
            this.label = label;
            this.position = nextPosition;
            this.context = Context;


            Context.AttributeHeight = 0;
            bool inArray = property.depth > 0;

        

            if (!inArray)
            {





                if (type.HasFlag(FieldSectionType.NewSection) || type.HasFlag(FieldSectionType.NewSectionDouble))
                {


                    if (!title.IsNullOrEmpty())
                    {
                        EditorGUILayout.Space(11);
                        var textPos = position;
                        textPos.y += 18;           
                        EditorGUI.LabelField(textPos, title, GUILogiked.Styles.Text_BigBold);
                    }

                    ChangeSection(true);

                    position = EditorGUILayout.GetControlRect(true, NextDrawerHeight(property, label, propertyAlreadyDrawn));
                    //base.GetPropertyHeightRecursive(property, label));
                    CallNextAttribute(position, property, label);

                }


                if (type.HasFlag(FieldSectionType.SectionSwitchColumn))
                {
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(10);
                    EditorGUILayout.BeginVertical( );
                    position = EditorGUILayout.GetControlRect(true, NextDrawerHeight(property, label, propertyAlreadyDrawn));
                    CallNextAttribute(position, property, label);

                }


                if (type.HasFlag(FieldSectionType.CloseSection))
                {
                    position = CallNextAttribute(position, property, label, IncrementRectMode.Afterdrawing);
                    ChangeSection(false);
                }


                if (property.isExpanded)
                {
               //     Context.AttributeHeight = 32;
                        EditorGUILayout.Space(-11);

                    var p2 = EditorGUILayout.GetControlRect(false, 48);
             
                    GUI.Box(p2, "Il est n'est pas recommandé d'utiliser les attributs SectionBegin/SectionClose sur des classes, array, et autres champs expandables dans l'inspecteurs. (Il y a des bugs quand on essaie de set des valeurs)", GUILogiked.Styles.Box_Warning1);
                }


            }
            else
            {

                #region In Array



                if (type.HasFlag(FieldSectionType.NewSection) || type.HasFlag(FieldSectionType.NewSectionDouble))
                {

                    position.height = 22;
                    // position.y += 6;

                    var textPos = position;
                    textPos.y += 24;
                    textPos.width -= 10;
                    textPos.x += 10;

                    // EditorGUI.indentLevel++;
                    var c = GUI.color;
                    GUI.color = Color.grey;
                    GUI.Box(textPos, "", BoxStyle);
                    GUI.color = Color.white;
                    textPos.x += 4;
                    textPos.y -= 2;
                    GUI.Label(textPos, title, GUILogiked.Styles.Text_BigBold);
                    GUI.color = c;


                    CallNextAttribute(position, property, label);
                }


                if (type.HasFlag(FieldSectionType.CloseSection))
                {
                    CallNextAttribute(position, property, label);
                    //  EditorGUI.indentLevel--;

                }

                #endregion


            }





        }







#endif

}
}



