#if UNITY_EDITOR


using logiked.language.translate;
using logiked.source.editor;
using logiked.source.extentions;
using logiked.source.utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace logiked.language.editor
{
    /// <summary>
    /// Propiété utiliséé pour dessiner le type Lstring dans l'éditeur. Se réferer à la section tutorial pour pour la prise en main de l'éditeur.
    /// </summary>
    [CustomPropertyDrawer(typeof(lstring))]
    public class Property_Lstring : PropertyDrawer
    {
      private enum EditMode { None, TextExitChanged }

        private class lstringDrawerDatas
        {
            public bool open;
            public bool untracked;
            public bool isRenamingKey;

            public bool initTranslate;
            public EditMode currentEdit = EditMode.None;

            public string textBackup;
            public string textModified;

            public string origKey;

            public SerializedProperty currentProp;

            public bool warningSent;
        }

        //Workaround de daube car en fait les Drawers sont réutilisés un peu partout dans le code, donc pour avoir des données PAR/Drawer c'est une méthode qui fonctionne...
        lstringDrawerDatas d;
        Dictionary<string, lstringDrawerDatas> m_PerPropertyViewData = new Dictionary<string, lstringDrawerDatas>();






        private void WarningString(SerializedProperty property)
        {
            if (d.warningSent) return;
            d.warningSent = true;

            LogikedPlugin_Language.Log($"LSTRING key not found on {property.GetParent()}/{property.name}", DebugC.ErrorLevel.Warning, property.serializedObject.targetObject);
        }


        private SerializedProperty content;
        private SerializedProperty translateId;



       /// <inheritdoc/>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            content = property.FindPropertyRelative(nameof(content));
            translateId = property.FindPropertyRelative(nameof(translateId));



            string dataString = property.propertyPath.ToString();
            bool removeAction = false;

            d = m_PerPropertyViewData.GetOrDefault(dataString);
            if (d == null) m_PerPropertyViewData.Add(dataString, d = new lstringDrawerDatas());

            d.currentProp = property;
            d.origKey = translateId.stringValue;

            string modifKey = null;

            var unityStyle = GUI.skin.GetStyle("Label");
            unityStyle.richText = true;


            bool untracked = d.untracked;


            GUIContent UntrackedLabel() => new GUIContent(label.text + "<Color=green><size=9>[untracked]</size></Color>", "This key is untracked.");
            GUIContent GetLabel() => new GUIContent(label.text + " <Color=magenta><size=9>[Loc]</size></Color>", "This string can be translated.");
            GUIContent ErrorLabel() => new GUIContent("<Color=red>" + label.text + " <size=9>[not exist]</size></Color>", "Translation failed. Check the key of this Localized String.");




            if (TranslationManager.DefaultTranslate == null)
            {
                position = EditorGUI.PrefixLabel(position, ErrorLabel(), unityStyle);

                TranslationManager.CheckAndDrawDefaultTranslateHelper();
                return;
            }

            d.untracked |= d.origKey.IsNullOrEmpty();
            d.untracked &= !TranslationManager.DefaultTranslate.HasKey(d.origKey);

            bool untrackedAndNotNull = d.untracked && !d.origKey.IsNullOrEmpty();



            /*
            if(untracked != d.untracked;)

            */
            untracked = d.untracked;


            if (d.open)
            {
            GUI.color = new Color(1f,1f,1f, 0.15f);
                GUILayout.BeginVertical(GUILogiked.Styles.Box_OpaqueWindowWhite);
            GUI.color = Color.white;
                GUILayout.Label("<b><size=11><Color=magenta>Content</color></size></b>", unityStyle);
            }


  
          
            if (!TranslationManager.CheckAndDrawDefaultTranslateHelper())
            {
                GUILayout.Label(ErrorLabel());
                return;
            }

            if (untracked)
                label = UntrackedLabel();
            else if (TranslationManager.DefaultTranslate.HasKey(d.origKey))
                label = GetLabel();
            else
            {
                label = ErrorLabel();
                WarningString(property);
            }
            

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.

            EditorGUI.BeginProperty(position, label, property);



            //Init the translation 
            if (!d.initTranslate)
            {
                d.initTranslate = true;
                UpdateTranslationValue(property);

            }

     
            // Draw label
            position = EditorGUI.PrefixLabel(position, label, unityStyle);


            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var rectIcon = new Rect(position.x + position.width - 20, position.y+2, 20, 20);


 
                GUI.enabled = d.currentEdit == Property_Lstring.EditMode.None && !Application.isPlaying;
            GUILogiked.Panels.GUIDrawEditorIcon(() =>
            {
                d.open = !d.open; 
                GUI.FocusControl(null);
            }, GUILogiked.Panels.EditorIconType.Gear, rectIcon);
            GUI.enabled = true;



            if (!d.open)///Panel closed
            {
                var rect1 = new Rect(position.x, position.y, position.width - 20, 20);

              

                GUI.enabled = false;
                EditorGUI.PropertyField(rect1, content, GUIContent.none);
                GUI.enabled = true;

            }
            else//pannel open
            {
                
                GUI.enabled &= untracked || TranslationManager.DefaultTranslate.HasKey(d.origKey);
                GUI.enabled &= !untrackedAndNotNull;

                d.textModified = EditorGUILayout.TextArea(content.stringValue, GUILayout.MinHeight(35));
                content.stringValue = d.textModified;
                GUI.enabled = true;


                if (d.currentEdit == EditMode.TextExitChanged)
                {

                    if (!untracked)
                    {
                        GUILayout.BeginHorizontal();
                        GUILogiked.Panels.GUIDrawEditorIcon(() =>
                        {

                            if (untracked || SaveValue(d.origKey, d.textModified))
                            {
                                d.textBackup = d.textModified;
                                d.open = false;
                            }
                        }, GUILogiked.Panels.EditorIconType.Save);



                        GUILayout.Label("Save changes");

                        GUILayout.FlexibleSpace();

                        GUILayout.Label("Revert changes");

                        GUILogiked.Panels.GUIDrawEditorIcon(() =>
                        {
                            GUI.FocusControl(null);

                            if (untracked)
                                content.stringValue = d.textBackup;

                            UpdateTranslationValue(property);
                        }, GUILogiked.Panels.EditorIconType.RemoveCross);
                        GUILayout.EndHorizontal();

                    }
                    else//Untracked c'est comme si on enregistrait en non stop
                    {
                        d.textBackup = d.textModified;

                    }
                }


                GUILayout.BeginHorizontal();

                GUILayout.Label("<b><size=11><Color=magenta>Key</color></size></b>", unityStyle);



                Rect iconPos = GUILayoutUtility.GetLastRect();
                iconPos.x = iconPos.xMax + 3;
                iconPos.width = 0;//pas afficher

                if (untrackedAndNotNull)
                {
                    iconPos.width = iconPos.height;
                }

                //Affichage constant pour pas faire de bugs
                    GUILogiked.Panels.GUIDrawEditorIcon(GUILogiked.Panels.EditorIconType.Error, iconPos, "You must create a new key to make changes persistent");
                

                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();



                GUI.enabled = d.currentEdit == EditMode.None;
                modifKey = EditorGUILayout.TextField(d.origKey);
                translateId.stringValue = modifKey;

                GUILayout.BeginHorizontal();

            
                    GUI.enabled = !TranslationManager.DefaultTranslate.HasKey(modifKey);


                    if (GUILayout.Button("Create") && !modifKey.IsNullOrWhiteSpace())
                        SaveValue(modifKey, d.textModified, true);


                    GUI.enabled = !GUI.enabled && !modifKey.IsNullOrEmpty();
                    if (GUILayout.Button("Rename"))
                    {
                        d.isRenamingKey = !d.isRenamingKey;
                        newKeyName = modifKey;
                    }


                    if (GUILayout.Button("Remove"))
                    {
                    removeAction = true;
                     
                    }




                    GUI.enabled = true;
                    if (GUILayout.Button("Set..."))
                    {
                        GUI.FocusControl(null);
                        TranslationManager.DefaultTranslate.GenerateMenu((m) =>
                        {
                            UpdateTranslationValue(property, m);
                        }, true);
                    }


                    GUILayout.EndHorizontal();


                    if (d.isRenamingKey)
                        DrawRenamingMenu();




                    GUI.enabled = true;

                }

            




            // EditorGUI.Popup(rectChoseTag, 0, ss);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();

          

            if (d.open)
            {
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            if (d.origKey != modifKey)
                UpdateTranslationValue(property);


            d.currentEdit = (d.textBackup != d.textModified)?EditMode.TextExitChanged: EditMode.None;

            if (removeAction && UnityEditor.EditorUtility.DisplayDialog("Remove key", "Do you want to remove Key {0} ?".Format(modifKey), "yes", "no") )
            {
                TranslationManager.DefaultTranslate.RemoveKey(modifKey);
                newKeyName = "";
            }

        }


        /// <inheritdoc/>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }



        /// <summary>
        /// Mise à jour de la traduction
        /// </summary>
        void UpdateTranslationValue(SerializedProperty obj)
        {
            UpdateTranslationValue(obj, translateId.stringValue);
        }

            void UpdateTranslationValue(SerializedProperty obj, string newTranslateId)
        {

           // bool wasntTracked = !TranslationManager.DefaultTranslate.HasKey(translateId.stringValue);

            translateId.stringValue = newTranslateId;
           
            if (TranslationManager.DefaultTranslate.HasKey(newTranslateId))
                d.untracked = false;


            if (!newTranslateId.IsNullOrEmpty() && !d.untracked)
            {
                var translatedValue = TranslationManager.GetTranslatedValue(newTranslateId);
                content.stringValue = translatedValue;
            }



            obj.serializedObject.ApplyModifiedProperties();
            d.textBackup = content.stringValue;
            d.textModified = d.textBackup;
        }


        string newKeyName;

        void DrawRenamingMenu()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("New key :", GUILayout.Width(70));

            newKeyName = EditorGUILayout.TextField(newKeyName);
          
            GUILogiked.Panels.GUIDrawEditorIcon(() =>
            {
                if (TranslationManager.DefaultTranslate.HasKey(newKeyName)) { EditorUtility.DisplayDialog("Key already exist", $"Key {newKeyName} already exist!", "Ok"); return; }
                TranslationManager.DefaultTranslate.RemoveKey(d.origKey);
                SaveValue(newKeyName, d.textModified, true);

                UpdateTranslationValue(d.currentProp , newKeyName);

                newKeyName = "";
                d.isRenamingKey = false;
                
            }, GUILogiked.Panels.EditorIconType.Save);

            GUILayout.EndHorizontal();

        }


        bool SaveValue(string key, string value, bool createIfNotExist = false)
        {
           return TranslationManager.DefaultTranslate.SaveValue(key, value, createIfNotExist);
        }


    }
}
#endif