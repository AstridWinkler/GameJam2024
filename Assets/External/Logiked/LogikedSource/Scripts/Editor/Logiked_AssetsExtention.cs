#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System;
using logiked.source.extentions;

using Object = UnityEngine.Object;

namespace logiked.source.editor
{

    [System.Serializable]
    public static class Logiked_AssetsExtention
    {




        /// <summary>
        /// Retoure tout les assets du Type T contenu dans le projet
        /// </summary>
        /// <param name="searchFolder">Le dossier de recherche.</param>
        public static List<T> FindAssetsByType<T>(params string[] searchFolder) where T : UnityEngine.Object
        {


            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchFolder);

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }



            return assets;
        }

        /// <summary>
        /// Retroune le chemin du fichier, relatif au dossier Assets (utile pour travailler avec Assetdabase)
        /// </summary>
        public static string GetFilePath(this ScriptableObject obj)
        {
            return (AssetDatabase.GetAssetPath(obj));
        }

        /// <summary>
        /// Retroune le chemin du dossier du fichier, relatif au dossier Assets (utile pour travailler avec Assetdabase)
        /// </summary>
        public static string GetFolderPath(this ScriptableObject obj)
        {
            var p = obj.GetFilePath();
            return p.IsNullOrEmpty() ? null : Path.GetDirectoryName(p);
        }



        /// <summary>
        /// Ajoute un racourci sur la dernière entrée d'un mennuItem
        /// </summary>
        /// <param name="menu">Le menu sur lequel le raccourci doit-être rajouté</param>
        /// <param name="key">La touche du raccourci</param>
        /// <param name="modifiers">Les modifiers de la touche (Control, Alt, Shift..) Peuvent-être cummulés.</param>
        /// <remarks>Les raccourcis sont effectifs seulement là où le GenericMenu est redéfini à chaque frames.</remarks>
        /// <returns></returns>
        public static bool AddShortcut(this GenericMenu menu, KeyCode key, EventModifiers modifiers = EventModifiers.None)
        {
            //Le code clavier qui s'affiche propement à l'écran la racourcis. A mettre dans un GUIContent (élément du menu)
            StringBuilder shortcutCode = new StringBuilder(" _");
            if (modifiers.HasFlag(EventModifiers.Control)) shortcutCode.Append("%");
            if (modifiers.HasFlag(EventModifiers.Alt)) shortcutCode.Append("&");
            if (modifiers.HasFlag(EventModifiers.Shift)) shortcutCode.Append("#");
            shortcutCode.Append(key.ToString());

            var assembly = typeof(UnityEditor.GenericMenu).Assembly.GetType("UnityEditor.GenericMenu");
             //assembly.DebugClassContentReflection();
            //Ce qui nous interesse
            //private ArrayList menuItems;

           var menuItemsProp = assembly.GetProperty("menuItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
       
            IList value = (IList)menuItemsProp.GetValue(menu);
            var lastItem = value[value.Count - 1];

            //lastItem.GetType().DebugClassContentReflection();
            //Type retourné : UnityEditor.GenericMenu.MenuItem item;
            //Ce qui nous interesse :
            //public MenuFunction func;
            //public MenuFunction2 func2;
            //public GUIContent content; 

            //Obtenir le GuiContent à modifier. Ajout des racourcis dans le texte
            var lastItemType = lastItem.GetType();
            var type = lastItemType.GetField("content", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            GUIContent guiText = (GUIContent)type.GetValue(lastItem);
            guiText.text += shortcutCode.ToString();

            //Obtenir la fonction callback
            type = lastItemType.GetField("func", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            GenericMenu.MenuFunction func = (GenericMenu.MenuFunction)type.GetValue(lastItem);

            //Si tout les modifier sont préssés (ctrl/Alt/Shif)
            if (!Event.current.modifiers.HasFlag(modifiers)) return false;
            //Si l'event est un keydown
            if (Event.current.type != EventType.KeyDown) return false;
            //Si la touche préssée est la bonne
            if (Event.current.keyCode != key) return false;

            //Alors activation de la fonction
            if (func != null)
            {
                Event.current.Use();
                func.Invoke();
                return true;
            }

         

            type = lastItemType.GetField("func2", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var datasField = lastItemType.GetField("userData", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            GenericMenu.MenuFunction2 func2 = (GenericMenu.MenuFunction2)type.GetValue(lastItem);
            System.Object userDatas = datasField.GetValue(lastItem);

            if (func2 != null)
            {
                Event.current.Use();
                func2.Invoke(userDatas);
                return true;
            }

            //User is using function on disabled Menu
            return false;
        }


        /// <summary>
        /// Verrifie si l'asset est immutable et ne peut donc pas être modifiée. (Si il fait parti d'un package, etc).
        /// </summary>
        /// <returns>Etat de l'asset</returns>
        public static bool IsImmutableAsset(this UnityEngine.Object obj)
        {
            if (obj == null) return true;//Si l'objet est nul ou si il n'a pas de PATH on estime qu'il n'esgt pas modifiable
            var path = AssetDatabase.GetAssetPath(obj);
            if (path.IsNullOrEmpty()) return true;
            return null != UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path);
        }

        /// <summary>
        /// Vérifie si l'asset dispose d'un certain label
        /// </summary>
        /// <param name="obj">L'asset comportant le label</param>
        /// <param name="label">Le label à trouver</param>
        /// <returns>True si le label est sur l'objet</returns>
        public static bool HasLabel(this UnityEngine.Object obj, string label)
        {
            if (obj == null) return false;
            var lst = AssetDatabase.GetLabels(obj).ToList();
            return lst.Contains(label);
        }



        /// <summary>
        /// Ajoute les labels à l'asset 
        /// <param name="obj">L'asset à modifier</param>
        /// <param name="labels">Les labels à ajouter sur l'objet</param>
        /// </summary>
        public static void AddLabel(this UnityEngine.Object obj, params string[] labels)
        {
            if (obj == null) return;


            var lst = AssetDatabase.GetLabels(obj).ToList();
            foreach (var label in labels)
            {
                if (lst.Contains(label)) return;
                lst.Add(label);
            }

            AssetDatabase.SetLabels(obj, lst.ToArray());

            obj.SetDirtyNow();
        }
        /// <summary>
        /// Suprime le label sur l'asset
        /// </summary>
        /// <param name="obj">l'asset à modifier</param>
        /// <param name="label">Le label à supprimer. Entrer '*' pour tous les supprimer.</param>
        /// <param name="searchAndRemove">Suprimer tout les labels contenant la chaine "label".</param>
        public static void RemoveLabel(this UnityEngine.Object obj, string label, bool searchAndRemove = false)
        {
            if (obj == null) return;
            bool removeAll = (label == "*");
            var lst = AssetDatabase.GetLabels(obj).ToList();
            if (!searchAndRemove)
            {
                if (!lst.Contains(label)) return;
                lst.Remove(label);
            }
            else if (searchAndRemove)
            {
                List<string> toRem = new List<string>();
                foreach (var e in lst) if (e.Contains(label) || removeAll) toRem.Add(e);
                if (toRem.Count == 0) return;
                foreach (var e in toRem) lst.Remove(e);
            }

            AssetDatabase.SetLabels(obj, lst.ToArray());


            obj.SetDirtyNow();

        }

        /// <summary>
        /// Suprime tous les labels de l'asset
        /// </summary>
        /// <param name="obj">l'asset à modifier</param>
        public static void RemoveLabel(this UnityEngine.Object obj)
        {
            obj.RemoveLabel("*", true);
        }


        /// <summary>
        /// Suprime tout les label contenant la chaine "search", et les updates avec la chaine "label".<br></br>
        /// <example> Utile quand on modifie le nom d'un asset, comme une texture, et qu'on veut mettre à jour toute ses animations.<br></br>
        /// <code>
        /// UpdateLabel("tex_nouveauID", "tex");// => a appliquer sur tout les assets concernés.
        /// </code></example>
        /// </summary>
        public static void UpdateLabel(this UnityEngine.Object obj, string label, string search)
        {
            if (obj == null) return;
            if (HasLabel(obj, label)) return;

            obj.RemoveLabel(search, false);
            obj.AddLabel(label);
        }

        /// <summary>
        /// Raccourci vers la fonction <see cref="EditorUtility.SetDirty(UnityEngine.Object)"/>
        /// </summary>
        /// <param name="obj">Objet à enregistrer</param>
        public static void SetDirtyNow(this UnityEngine.Object obj)
        {
            if (obj == null) return;
            EditorUtility.SetDirty(obj);

        }

        /// <summary>
        /// Enregistre et recharge l'asset 
        /// </summary>
        /// <param name="obj">Asset à enregistrer</param>
        public static void SaveAndReload(this UnityEngine.Object obj)
        {
            if (obj == null) return;

            EditorUtility.SetDirty(obj);


            if (!AssetDatabase.IsAssetImportWorkerProcess())
            {
                AssetDatabase.SaveAssets();

                var path = AssetDatabase.GetAssetPath(obj);
                if (!path.IsNullOrEmpty())
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
            else
            {
                Debug.LogError("Aborted, loading..");
            }
        }

        /// <summary>
        /// Format du chemin
        /// </summary>
        public enum PathFormat {
            /// <summary>
            /// Chemin relatif au dossier Assets.
            /// </summary>
            AssetRelative,
            /// <summary>
            /// Chemin Absolu, depuis la racine du disque.
            /// </summary>
            AbsolutePath }


        /// <summary>
        /// Obtenir le Path de cet Asset. Attention : L'objet doit être un asset (prefab, scriptableObject.. etc)
        /// </summary>
        /// <param name="target">L'objet en question</param>
        /// <param name="format">Format du Path retourné.</param>
        /// <param name="withExtention">Supprimer l'extension du fichier dans le résultat ?</param>
        /// <returns>Le chemin de l'objet</returns>
        public static string GetAssetPath(this UnityEngine.Object target, PathFormat format = PathFormat.AbsolutePath, bool withExtention = true)
        {
            var path = GetAssetPath(target, format);
            if (!withExtention)
                path = path.Substring(0, path.Length - Path.GetExtension(path).Length);
            return path;
        }

        /// <summary>
        /// Obtenir le Path de cet Asset. Attention : L'objet doit être un asset (prefab, scriptableObject.. etc)
        /// </summary>
        /// <param name="target">L'objet en question</param>
        /// <param name="format">Format du Path retourné.</param>
        /// <param name="withExtention">Supprimer l'extension du fichier dans le résultat ?</param>
        /// <returns>Le chemin de l'objet</returns>
        public static string GetAssetPath(UnityEngine.Object target, PathFormat format = PathFormat.AbsolutePath)
        {
            var path = AssetDatabase.GetAssetPath(target);
            if (format == PathFormat.AssetRelative)
                return path;
            return GetAbsolutePath(path);
        }

        /// <summary>
        /// Converti un Path relatif aux assets en chemin absolu
        /// </summary>
        /// <param name="path">Chemin relatif au dossier 'Assets'</param>
        /// <returns>Chemin de type C:/../../.</returns>
        public static string GetAbsolutePath(string path) => path.IsNullOrEmpty() ? "" : Application.dataPath.Replace("Assets", path);



        /// <summary>
        /// Converti un chemin absolu en chemin relatif aux assets. Utile pour travailler avec la classe <see cref="AssetDatabase"/> 
        /// </summary>
        /// <param name="path">Chemin absolu</param>
        /// <returns>Chemin de format Assets/../../.</returns>
        public static string GetRelativePath(string path) => path.Replace(Path.DirectorySeparatorChar, '/').Replace(Application.dataPath, "Assets");




    
    }
}

#endif
