#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using logiked.source.editor;
using logiked.source.utilities;
using System.Text;
using System.Linq;
using System.IO;
using logiked.source.database;
using logiked.source.extentions;

namespace logiked.source.database.editor
{

    /// <summary>
    /// Inspector de base pour les LogikedDatabase, à override pour vos propres base de données.
    /// </summary>
    [CustomEditor(typeof(LogikedDatabase))]
    [CanEditMultipleObjects]
    public abstract class Inspector_LogikedDatabase : Editor
    {

        protected SerializedProperty isSingletonDatabase;

        protected SerializedProperty itemList;
        protected SerializedProperty categories;
        protected SerializedProperty generatedItemScriptName;
        protected SerializedProperty itemTags;
        protected SerializedProperty createFolderForEachElements;
        protected SerializedProperty databaseElementsLabel
;

        LogikedDatabase thisDatabase;

        Dictionary<Object, Editor> itemEditorList = new Dictionary<Object, Editor>();


        //  List<bool> editItem = new List<bool>();


        Color boxCol = new Color(0.5f, 0.5f, 0.5f);

        bool editCategories = false;

        bool cancelOperation = false;

        bool newCategoryMenuOpen;


        string newCategoryName;

        protected bool newItemMenuOpen;

        DatabaseCategory newCategory = new DatabaseCategory();









        public virtual void OnEnable()
        {

            thisDatabase = target as LogikedDatabase;

            /* if (LogikedPlugin_ItemSystem.Instance.DefaultItemDatabase != (ItemDatabase)target)
             {
                 Debug.LogError("Default item database SET!");
                 LogikedPlugin_ItemSystem.Instance.DefaultItemDatabase = (ItemDatabase)target;
             }*/





            itemList = serializedObject.FindProperty("itemList");
            categories = serializedObject.FindProperty("categories");
            itemTags = serializedObject.FindProperty("itemTags");
            generatedItemScriptName = serializedObject.FindProperty("generatedItemScriptName");
            createFolderForEachElements = serializedObject.FindProperty("createFolderForEachElements");
            isSingletonDatabase = serializedObject.FindProperty("isSingletonDatabase");
            databaseElementsLabel = serializedObject.FindProperty("databaseElementsLabel");


            if (!databaseElementsLabel.stringValue.IsNullOrEmpty() && !target.HasLabel(databaseElementsLabel.stringValue))
            {
                target.RemoveLabel();
                target.AddLabel(databaseElementsLabel.stringValue);
            }


            RefreshItemEditorList();
        }


        public void DrawSectionText(string text) => DrawSectionText(text, () => { });

        private bool lastSectOpen;

        public void DrawSectionText(string text, System.Action drawIconAction)
        {

            EditorGUI.indentLevel = 0;





            GUILayout.BeginHorizontal();
            GUILayout.Label(text, GUILogiked.Styles.Text_Big);
            GUILayout.FlexibleSpace();
            drawIconAction();
            GUILayout.EndHorizontal();


            EditorGUI.indentLevel = 1;



        }



        //protected virtual void DrawSettingsPanel() { }

        public void OnItemRemoved_(DatabaseAbstractElement element, string assetPath)
        {
            element.OnRemoveScriptableObject();
            OnItemRemoved(element, assetPath);
        }

        /// <summary>
        /// Appelé à la suppression d'un item
        /// </summary>
        public virtual void OnItemRemoved(DatabaseAbstractElement element, string assetPath) { }


        /// <summary>
        /// Dessine des informations / champs assignables ocmplémentaires pour la création de nouveaux élements dans la bdd.
        /// </summary>
        /// <returns>Si retourne vrai, autorise la création de l'élément</returns>
        public virtual bool DrawNewElementPanel()
        {
            return true;
        }




        protected string SaveCreatedElementFolder(int category, string fileName, DatabaseAbstractElement newElement)
        {


            var categBase = GetCategory(category);
            string itemFold = categBase.CategoryName;


            if (createFolderForEachElements.boolValue)
            {
                itemFold += "/" + fileName;
                itemFold = CreateFolderFromDatabase(itemFold);
            }
            else
            {
                itemFold = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)) + "/" + itemFold;
            }




            var path = itemFold + "/" + fileName;

            if (!Directory.Exists(Logiked_AssetsExtention.GetAbsolutePath(itemFold)))
                Directory.CreateDirectory(Logiked_AssetsExtention.GetAbsolutePath(itemFold));

            AssetDatabase.CreateAsset(newElement as ScriptableObject, path + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ///Ajouté d'un l'abel pour sort les items dans l'inspector [WIP]
            if (!databaseElementsLabel.stringValue.IsNullOrEmpty())
            {
                var lab = databaseElementsLabel.stringValue + "_" + AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newElement));
                newElement.AddLabel(databaseElementsLabel.stringValue, lab);
            }

            return itemFold + "/";
        }


        /// <summary>
        /// CRéation d'un nouvel élement dans la bdd.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual DatabaseAbstractElement CreateNewElement(int category, string fileName)
        {


            var categBase = GetCategory(category);

            if (categBase == null)
            {
                Debug.LogError("La catégorie de l'élement que vou sessayez de créer n'existe pas. Annulation.");
            }


            System.Type type = System.Type.GetType(categBase.DesciptorClass);

            if (type == null)
            {
                Debug.LogWarningFormat("Le type spécifié de Descriptor dans la catégorie <color=red>{0}</color> de la bdd <color=red>{1}</color> est nul ! Assignement automatique du type par défaut de la bdd dans la catgorie.",
                   categBase?.CategoryName, target.name);

                categBase.DesciptorClass = GetDesciptorBaseType().AssemblyQualifiedName;
            }

            DatabaseAbstractElement newItem = (DatabaseAbstractElement)CreateInstance(type);
            newItem.name = fileName;

            SaveCreatedElementFolder(category, fileName, newItem);


            return newItem;
        }



        public System.Type GetDesciptorBaseType()
        {
            return (target as LogikedDatabase).ItemListBase.GetType().GetElementType();
        }

        public virtual void OnCustomInspectorGui() { }

        public sealed override void OnInspectorGUI()
        {

            //  DrawDefaultInspector();
            //  return;

            cancelOperation = false;

            serializedObject.Update();






            //    if (!TranslationManager.CheckAndDrawDefaultTranslateHelper())
            //        return;


            DrawSettings();

            GUILayout.Space(10);


            OnCustomInspectorGui();

            DrawCategories();

            GUILayout.Space(10);
            DrawTags();



            /*
            GUI.enabled = false;
             EditorGUILayout.PropertyField(categories);
            GUI.enabled = true;*/

            // EditorGUILayout.PropertyField(itemList);
            GUILayout.Space(10);


            DrawItemSet();

            serializedObject.ApplyModifiedProperties();
        }


        void DrawSettings()
        {
            DrawSectionText("Setting", () =>
            {
                GUILogiked.Panels.GUIDrawEditorIcon(() => EditorGUIUtility.PingObject(target), GUILogiked.Panels.EditorIconType.FolderWhite);
            });




            EditorGUILayout.PropertyField(isSingletonDatabase);
            EditorGUILayout.PropertyField(databaseElementsLabel);
            EditorGUILayout.PropertyField(createFolderForEachElements);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(generatedItemScriptName);



            if (false && GUILayout.Button("Generate/Update {0}.cs item script references".Format(generatedItemScriptName.stringValue)))
            {
                GenerateProjectItemScript();
            }



        }



        void DrawTags()
        {
            DrawSectionText("Tags");
            EditorGUILayout.PropertyField(itemTags);
        }

        void DrawCategories()
        {
            DrawSectionText("Categories", () => GUILogiked.Panels.GUIDrawEditorIcon(() => editCategories = !editCategories, GUILogiked.Panels.EditorIconType.Gear));



            for (int i = 0; i < categories.arraySize; i++)
            {
                if (!DrawCatergory(categories.GetArrayElementAtIndex(i), editCategories))
                {
                    categories.DeleteArrayElementAtIndex(i);
                    break;
                }
            }



            if (editCategories)
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("New"))
                {


                    newCategoryMenuOpen = !newCategoryMenuOpen;
                }

                GUILayout.EndHorizontal();

                if (newCategoryMenuOpen)
                {
                    GUI.color = boxCol;
                    GUILayout.BeginVertical("box");
                    GUI.color = Color.white;
                    newCategoryName = EditorGUILayout.TextField(new GUIContent("Category name", "Des categories pour trier les elements de la bdd"), newCategoryName, GUILayout.MinWidth(100));


                    /*
                     newCategoryDescriptorScript = (MonoScript)EditorGUILayout.ObjectField(new GUIContent("Base descriptor class", "The default descriptor class for items of this category"), newCategoryDescriptorScript, typeof(MonoScript), false);


                     newCategoryScript = (MonoScript)EditorGUILayout.PropertyField(new GUIContent("Base behaviour class", "The default behaviour class for items of this category"), newCategoryScript, typeof(MonoScript), false);
                     if (newCategoryScript != null && !typeof(ItemBehaviour).IsAssignableFrom(newCategoryScript.GetClass()))
                         newCategoryScript = null;
                     */



                    GUILayout.FlexibleSpace();

                    GUI.enabled = !(newCategoryName.IsNullOrEmpty() || newCategoryName.Contains(" ")); //|| newCategoryScript == null || newCategoryDescriptorScript == null);

                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                    {

                        categories.InsertArrayElementAtIndex(categories.arraySize);
                        var prop = categories.GetArrayElementAtIndex(categories.arraySize - 1);
                        prop.FindPropertyRelative("categoryName").stringValue = newCategoryName;
                        prop.FindPropertyRelative("desciptorClass").stringValue = GetDesciptorBaseType().AssemblyQualifiedName;


                        // prop.FindPropertyRelative("BaseBehaviourClass").stringValue = newCategoryScript.GetClass().Name;
                        // prop.FindPropertyRelative("DesciptorClass").stringValue = newCategoryDescriptorScript.GetClass().Name;

                        CreateFolderFromDatabase(newCategoryName);

                        newCategoryMenuOpen = false;
                        newCategoryName = "";
                        editCategories = true;
                        // newCategoryScript = null;

                    }
                    GUILayout.EndVertical();
                    GUI.enabled = true;
                }
            }
        }



        List<DatabaseAbstractElement> sortedItemList = new List<DatabaseAbstractElement>();

        protected string newItemFileName;
        protected int editItemCategory;

        void DrawItemSet()
        {
            GUILayout.Label("Items set ", GUILogiked.Styles.Text_Big);
            cancelOperation = false;

            DatabaseAbstractElement desc;

            //Draw each objects
            for (int i = 0; i < itemList.arraySize; i++)
            {

                desc = itemList.GetArrayElementAtIndex(i).objectReferenceValue as DatabaseAbstractElement;
                if (!sortedItemList.Contains(desc) && desc != null)
                {
                    sortedItemList.Add(desc);
                }
            }

            if (sortedItemList.Contains(null))
                sortedItemList = sortedItemList.Where(m => m != null).ToList();

            //Draw sort tools
            DrawSortToolbar();



            //Draw each objects
            for (int i = 0; i < sortedItemList.Count; i++)
            {
                DrawItem(sortedItemList[i]);

            }




            if (GUILayout.Button("New element", GUILayout.Width(75)))
            {
                newItemMenuOpen = !newItemMenuOpen;
            }


            if (newItemMenuOpen)
            {

                GUI.color = boxCol;
                GUILayout.BeginVertical("box");
                GUI.color = Color.white;

                newItemFileName = EditorGUILayout.TextField(new GUIContent("File name"), newItemFileName);
                editItemCategory = EditorGUILayout.Popup("Item type ", editItemCategory, ((LogikedDatabase)target).ItemCategories);


                var readyToCreate = DrawNewElementPanel();

                GUILayout.FlexibleSpace();

                GUI.enabled = readyToCreate && !(newItemFileName.IsNullOrEmpty() || newItemFileName.Contains(" "));

                if (GUILayout.Button("Add", GUILayout.Width(50)))
                {

                    var newItem = CreateNewElement(editItemCategory, newItemFileName);
                    itemList.InsertArrayElementAtIndex(itemList.arraySize);
                    var prop = itemList.GetArrayElementAtIndex(itemList.arraySize - 1);
                    prop.objectReferenceValue = newItem;
                    prop.serializedObject.ApplyModifiedProperties();
                    RefreshItemEditorList();
                    EditorGUIUtility.PingObject(newItem);

                    newItemFileName = "";
                    newItemMenuOpen = false;
                    editItemCategory = 0;
                    newItemFileName = "";
                }


                GUILayout.EndVertical();
                GUI.enabled = true;
            }
        }


        /// <summary>
        /// Retourne la catégorie de l'id category
        /// </summary>
        public DatabaseCategory GetCategory(int category)
        {
            return ((LogikedDatabase)target).GetCategory(category);
        }



        /// <summary>
        /// Desine la barre de recherche d'items
        /// </summary>
        void DrawSortToolbar()
        {

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh folders"))
            {
                sortedItemList = Logiked_AssetsExtention.FindAssetsByType<DatabaseAbstractElement>(thisDatabase.GetFolderPath());
                sortedItemList = sortedItemList.Where((m) => m != null).ToList();
                thisDatabase.SetItemList(sortedItemList.ConvertAll(m => m as DatabaseAbstractElement));
                RefreshItemEditorList();
            }



            if (GUILayout.Button("Sort by category") && sortedItemList.Count > 0)
            {
                var newList = sortedItemList.OrderBy(m => m.CategoryId).ToList();
                if (newList[0] == sortedItemList[0])
                    sortedItemList = sortedItemList.OrderBy(m => -m.CategoryId).ToList();
                else
                    sortedItemList = newList;
            }

            if (GUILayout.Button("Open all") && sortedItemList.Count > 0)
            {
                foreach (var item in sortedItemList)
                    item.IsEditedByDatabase = true;

            }

            if (GUILayout.Button("Close all") && sortedItemList.Count > 0)
            {
                foreach (var item in sortedItemList)
                    item.IsEditedByDatabase = false;
            }


            GUILayout.EndHorizontal();
        }




        /// <summary>
        /// Creer un dossier relatif au chemin de cette database
        /// </summary>
        protected string CreateFolderFromDatabase(string fold)
        {
            string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)) + "/" + fold;

            if (!AssetDatabase.IsValidFolder(path))
                AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            /*  catch (System.Exception e)
              {
                  Debug.LogException(e);
                  Debug.LogError("Folder:"+path);
              }*/
            return path;
        }







        void GenerateProjectItemScript()
        {
            /*
            int i, j;
            SerializedProperty propCateg;
            SerializedProperty propItem;
            DatabaseAbstractElement objItem;

            var p = System.IO.Path.GetDirectoryName(Logiked_AssetsExtention.GetAbsolutePath(target)) + "/{0}.cs".Format(generatedItemScriptName.stringValue);
            StringBuilder finalFile = new StringBuilder();
            string codeEnum = @"enum {0}Type {{ {1} }};";
            string categoryName;

            StringBuilder enumGenerated = new StringBuilder();
            StringBuilder itemGenerated = new StringBuilder();



            for (i = 0; i < categories.arraySize; i++)
            {
                propCateg = categories.GetArrayElementAtIndex(i);
                categoryName = propCateg.FindPropertyRelative("categoryName").stringValue;
                enumGenerated.Append(categoryName);
                enumGenerated.Append("=");
                enumGenerated.Append(i);

                if (i != categories.arraySize - 1)
                    enumGenerated.Append(",");

                itemGenerated.Append("\npublic static class ");
                itemGenerated.Append(categoryName);
                itemGenerated.Append("{\n");



                for (j = 0; j < itemList.arraySize; j++)
                {
                    propItem = itemList.GetArrayElementAtIndex(j);
                    objItem = propItem.objectReferenceValue as ItemDescriptor;
                    if (objItem == null) continue;
                    if (objItem.ItemCategoryId == i) {
                        itemGenerated.Append("public static readonly ItemID ");
                        itemGenerated.Append(objItem.FileNameShort);
                        itemGenerated.Append(" = \"");
                        itemGenerated.Append(objItem.GetItemId());
                        itemGenerated.Append("\";\n");
                    }
                }

                itemGenerated.Append("}\n");



            



            }

            codeEnum = codeEnum.Format(generatedItemScriptName.stringValue, enumGenerated.ToString());




            string codeItems = @"public static class {0} {{ 
{1}
}};";
            codeItems = codeItems.Format(generatedItemScriptName.stringValue, itemGenerated.ToString());

            finalFile.Append("using logiked.items;\n\n");
            finalFile.Append(codeEnum);
            finalFile.Append("\n\n\n");
            finalFile.Append(codeItems);



            Debug.Log("Creating file:" + p);
            System.IO.File.WriteAllText(p, finalFile.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            */

        }


        /// <summary>
        /// Desine la catégorie Prop, retourne False si elle est suprimée
        /// </summary>
        bool DrawCatergory(SerializedProperty prop, bool edit)
        {

            if (!edit)
            {
                EditorGUILayout.LabelField("· " + prop.FindPropertyRelative("categoryName").stringValue, GUILogiked.Styles.Text_BigBold);
                return true;
            }



            GUI.color = boxCol;
            GUILayout.BeginVertical("box");
            GUI.color = Color.white;


            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(prop.FindPropertyRelative("categoryName").stringValue, GUILogiked.Styles.Text_BigBold);


            GUILayout.FlexibleSpace();

            GUILogiked.Panels.GUIDrawEditorIcon(() => { cancelOperation = true; }, GUILogiked.Panels.EditorIconType.RemoveCross);
            GUILayout.EndHorizontal();

            if (cancelOperation) return false;


            EditorGUILayout.PropertyField(prop);


            GUILayout.EndVertical();
            return true;
        }





        void DrawItem(DatabaseAbstractElement prop)
        {

            if (prop == null) return;

            DatabaseAbstractElement desc = prop;



            if (!itemEditorList.ContainsKey(desc) || itemEditorList[desc] == null)
                RefreshItemEditorList();

            Editor editor = itemEditorList[desc];

            //Editor.CreateCachedEditorWithContext(prop, prop, null, ref editor);


            //  if (editor != null) editor.editEnabled = prop.IsEditedByDatabase;


            GUI.color = boxCol;
            GUILayout.BeginVertical("box");
            GUI.color = Color.white;

            GUILayout.BeginHorizontal();

            GUILogiked.Panels.GUIDrawEditorIcon(() => prop.IsEditedByDatabase = !prop.IsEditedByDatabase, GUILogiked.Panels.EditorIconType.Gear);
            GUILogiked.Panels.GUIDrawEditorIcon(() => EditorGUIUtility.PingObject(desc), GUILogiked.Panels.EditorIconType.FolderWhite);



            //  editor.DrawHeaderCategoryText();
            DrawElementHeader(prop);

            if (prop.IsEditedByDatabase)
            {

                GUILayout.FlexibleSpace();
                GUILogiked.Panels.GUIDrawEditorIcon(() =>
                {
                    var result = EditorUtility.DisplayDialog("Delete", "Voulez vous suprimer les fichiers de l'item ?", "yes", "no");

                    switch (result)
                    {
                        case true:

                            var path = AssetDatabase.GetAssetPath(desc);

                            // Si le fichier n'est pas dans un sous-dossier, on suprime juste le fichier

                            if (Path.GetDirectoryName(Path.GetDirectoryName(path)) != Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)))
                            {
                                //Sinon on suprime tout le dossier associé à l'élément
                                path = Path.GetDirectoryName(path);
                            }


                            OnItemRemoved_(desc, path);
                            AssetDatabase.DeleteAsset(path);
                            goto NO;

                        case false:

                        NO:
                            if (thisDatabase != null)
                            {
                                itemList.DeleteArrayElementAtIndex(thisDatabase.ItemListBase.ToList().IndexOf(prop));
                                RefreshItemEditorList();
                            }
                            else
                            {
                                Debug.LogError("error when removing item!!");
                            }
                            break;

                    }




                }, GUILogiked.Panels.EditorIconType.RemoveCross);
            }

            GUILayout.EndHorizontal();

            if (prop.IsEditedByDatabase && editor != null) editor.OnInspectorGUI();


            GUILayout.EndVertical();
            return;
        }


        public void RefreshItemEditorList()
        {


            if (thisDatabase != null)
                EditorUtility.SetDirty(thisDatabase);


            var lst2 = new Dictionary<Object, Editor>();
            SerializedProperty prop;
            //Inspector_DatabaseAbstractElement curEdit;
            Editor curEdit;
            Object selectedObj;


            Debug.Log("Refreshing item database..");

            ///Recuperation et construction des editors de la liste d'objs

            for (int i = 0; i < itemList.arraySize; i++)
            {
                prop = itemList.GetArrayElementAtIndex(i);
                selectedObj = prop.objectReferenceValue;
                if (selectedObj == null) continue;


                if (itemEditorList.ContainsKey(selectedObj) && itemEditorList[selectedObj] != null)
                {

                    lst2.Add(selectedObj, itemEditorList[selectedObj]);
                    itemEditorList.Remove(selectedObj);
                    continue;
                }
                else
                {
                    curEdit = CreateEditor(prop.objectReferenceValue);// as Inspector_DatabaseAbstractElement;
                    lst2.Add(selectedObj, curEdit);
                }
            }
            ///Supression des editors inutilisés

            foreach (var e in itemEditorList)
                DestroyImmediate(e.Value);


            itemEditorList = lst2;


            //assignement de la database pour chaque item
            foreach (var e in itemEditorList.Keys)
            {
                DatabaseAbstractElement elem = e as DatabaseAbstractElement;
                if (elem.Database != thisDatabase)
                {
                    elem.Database = thisDatabase;
                    EditorUtility.SetDirty(elem);
                }
            }


        }





        public void DrawElementHeader(DatabaseAbstractElement elem)
        {

            GUILayout.BeginHorizontal();


            GUILayout.FlexibleSpace();

            DatabaseCategory categ;


            if ((categ = GetCategory(elem.CategoryId)) != null)
            {

                EditorGUILayout.LabelField($"<color=#{ColorUtility.ToHtmlStringRGB(categ.categoryColor)}>{categ.CategoryName}</color>", GUILogiked.Styles.Text_Big); ;
                GUILayout.Space(10);
                EditorGUILayout.LabelField($"<color=#{ColorUtility.ToHtmlStringRGB(categ.categoryColor)}>{elem.ItemName}</color>", GUILogiked.Styles.Text_Big);
            }

            else
            {
                EditorGUILayout.LabelField(elem.ItemName, GUILogiked.Styles.Text_Big);
                GUILayout.Space(10);
                EditorGUILayout.LabelField("[type:" + elem.CategoryId + "]", GUILogiked.Styles.Text_Big);
            }


            GUILayout.FlexibleSpace();

            if (!elem.IsEditedByDatabase && elem.Sprite != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(40, 40);
                GUILogiked.Panels.DrawSprite(elem.Sprite, previewRect);
            }


            GUILayout.EndHorizontal();

        }


    }
}

#endif