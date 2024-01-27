#if UNITY_EDITOR
using logiked.source.extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace logiked.source.editor
{
    /// <summary>
    /// Finds custom property drawer for a given type.
    /// From https://forum.unity.com/threads/solved-custompropertydrawer-not-being-using-in-editorgui-propertyfield.534968/
    /// </summary>
    public static class PropertyDrawerFinder
    {
        struct TypeAndFieldInfo
        {
            internal Type type;
            internal FieldInfo fi;
        }

        /// <summary>
        /// Dessine la propriété correctement (avec son drawer custom si elle en possède un)
        /// </summary>
        /// <param name="prop">La propiété</param>
        /// <param name="position">Position alloué à son affichage</param>
        /// <param name="label">Le label associé</param>
        public static void DrawPropertyField(this SerializedProperty prop, Rect position, GUIContent label)
        {
            PropertyFieldWithoutAttributes(position, prop, label);
        }

        /// <summary>
        /// Dessine la propriété correctement (avec son drawer custom si elle en possède un). Attention, système d'affichage différent si c'est Unity qui s'occupe d'affichage.
        /// </summary>
        /// <param name="prop">La propiété</param>
        /// <param name="label">Le label associé</param>
        public static void DrawPropertyField(this SerializedProperty prop, GUIContent label)
        {
            PropertyFieldWithoutAttributes(prop, label);
        }


        // Rev 3, be more evil with more cache!
        private static readonly Dictionary<int, TypeAndFieldInfo> s_PathHashVsType = new Dictionary<int, TypeAndFieldInfo>();
        private static readonly Dictionary<Type, PropertyDrawer> s_TypeVsDrawerCache = new Dictionary<Type, PropertyDrawer>();



        public static object GetParent(this SerializedProperty prop)
        {

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }


        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }



        private static MethodInfo _propertyFieldNotAttributesChilds;
        private static MethodInfo PropertyFieldNotAttributesChilds
        {
            get
            {
                if (_propertyFieldNotAttributesChilds == null)
                {
                    //typeof(EditorGUI).DebugClassContentReflection();
                    _propertyFieldNotAttributesChilds = typeof(EditorGUI).GetMethod("PropertyFieldInternal", BindingFlags.NonPublic | BindingFlags.Static);
                    //PropertyFieldInternal
                    //DefaultPropertyField
                }
                return _propertyFieldNotAttributesChilds;
            }
        }



        private static MethodInfo _propertyFieldNotAttributesNoChilds;
        private static MethodInfo PropertyFieldNotAttributesNoChilds
        {
            get
            {
                if (_propertyFieldNotAttributesNoChilds == null)
                {
                    // typeof(EditorGUI).DebugClassContentReflection();
                    _propertyFieldNotAttributesNoChilds = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static);
                    //PropertyFieldInternal
                    //DefaultPropertyField
                }
                return _propertyFieldNotAttributesNoChilds;
            }
        }



        private static MethodInfo _propertyHeightNotAttributes;
        private static MethodInfo PropertyHeightNotAttributes
        {
            get
            {
                if (_propertyHeightNotAttributes == null)
                {
                    _propertyHeightNotAttributes = typeof(EditorGUI).GetMethod("GetPropertyHeightInternal", BindingFlags.NonPublic | BindingFlags.Static);
                    //PropertyFieldInternal
                    //DefaultPropertyField
                }
                return _propertyHeightNotAttributes;
            }
        }



        private static MethodInfo _propertyHeightNoChilds;
        private static MethodInfo PropertyHeightNoChilds
        {
            get
            {
                if (_propertyHeightNoChilds == null)
                {
                    _propertyHeightNoChilds = typeof(EditorGUI).GetMethod("GetSinglePropertyHeight", BindingFlags.NonPublic | BindingFlags.Static);
                }
                return _propertyHeightNoChilds;
            }
        }




        /// <summary>
        /// Draw property with associated Property Drawer if exist
        /// </summary>

        public static void PropertyFieldWithoutAttributes(SerializedProperty property, GUIContent label)
        {
            var prop = FindDrawerForProperty(property);

            // typeof(EditorGUILayout).DebugClassContentReflection();


            Rect position;


            if (prop == null)
                position = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(property, true));//tester EditorGUI.GetPropertyHeight(property, true)
            else
                position = EditorGUILayout.GetControlRect(true, prop.GetPropertyHeight(property, label));




            property.DrawPropertyField(position, label);
        }

        /*

        if (prop != null)
        {
            var position = EditorGUILayout.GetControlRect();//tester EditorGUI.GetPropertyHeight(property, true)
            prop.OnGUI(position, property, new GUIContent(property.displayName, label.image, label.tooltip));
        }
        else
        {
          //  PropertyFieldlayoutNotAttributes.Invoke(null, new object[] { property, new GUIContent(property.displayName, label.image, label.tooltip) });


        }
    */

        /// <summary>
        /// Draw property with associated Property Drawer if exist
        /// </summary>

        public static void PropertyFieldWithoutAttributes(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = FindDrawerForProperty(property);

            label = new GUIContent(property.displayName, label.image, label.tooltip);

            if (prop != null)
            {
                prop.OnGUI(position, property, label);
            }
            else
            {


                PropertyFieldNotAttributesChilds.Invoke(null, new object[] { position, property, label, true });

                /*
                if (property.isExpanded)
                    PropertyFieldNotAttributesChilds.Invoke(null, new object[] { position, property, label, true });
                //Fait des loop holes de Drawer incroyable ! TU peux mettre 4 drawer, ils vont tous agir plusieur fois cest un bordels.
                /C'est pouquoi jai été oblgé de dev des bout de code "anti recusrion" dans BetterDrawer

                else
                    PropertyFieldNotAttributesNoChilds.Invoke(null, new object[] { position, property, label });
                */
                //Corrige le problème de dupplication des Drawers
                //Empeche les classes de s'afficher correctement, et toutes les scructures qui ont besoin d'etre deployed

            }
        }



        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int a = 0;
            return GetDefaultPropertyHeight(property, label, ref a);
        }

        public static float GetDefaultPropertyHeight(SerializedProperty property, GUIContent label, ref int iterations)
        {
            var prop = FindDrawerForProperty(property);

            // return 20;

            label = new GUIContent(property.displayName, label.image, label.tooltip);

            //  return  EditorGUI.GetPropertyHeight(property, label);


            if (prop != null)
            {
                return prop.GetPropertyHeight(property, label);
            }
            else
            {


                if (property.isExpanded)
                {

                    //  return (float)PropertyHeightNotAttributes.Invoke(null, new object[] { property, label, true });
                    //  return (float)PropertyHeightNotAttributes.Invoke(null, new object[] { property, label, true });             
                    //Retourne de la merde, ducoup je joue la récusrion

                    SerializedProperty iterator = property.Copy();

                    float sum = (float)PropertyHeightNoChilds.Invoke(null, new object[] { iterator, label });
                    int ind = 0;

                    var enumerator = iterator.GetEnumerator();
                    StringBuilder blt = new StringBuilder();

                    var baseDepth = iterator.depth;

                    while (enumerator.MoveNext())
                    {
                        var propy = enumerator.Current as SerializedProperty;
                        if (propy == null) continue;


                        if (baseDepth + 1 != propy.depth && baseDepth != propy.depth)
                        {
                            //         Debug.Log($"{propy.propertyPath}:{baseDepth} vs {propy.depth}");
                            continue;
                        }
                        else
                        {
                            //     Debug.Log($"{propy.propertyPath}");

                        }

                        /*
                        blt.AppendLine(propy.displayName);

                       var parent = propy.GetParent() as SerializedProperty;
                        if (parent != property && propy != property)
                        {
                        //    Debug.Log("Canceled parent : " + parent.displayName);
                            continue;
                        }*/

                        int ret = 0;

                        sum += GetDefaultPropertyHeight(propy, new GUIContent(iterator.displayName), ref ret);

                        //  if(ret > 0)
                        // Debug.Log("analy;"+ ret);

                        ind++;
                        iterations = ind;
                    }

                    //  Debug.Log(blt.ToString());


                    /*
                    while (iterator.Next(true))
                        {
                        ind++;
                        sum += GetDefaultPropertyHeight(iterator, new GUIContent(iterator.displayName));
                        }*/

                    //    Debug.Log(ind);
                    return sum + 20;

                }
                else
                    return (float)PropertyHeightNotAttributes.Invoke(null, new object[] { property, label, false });

                // return (float)PropertyHeightNoChilds.Invoke(null, new object[] { property, label });


            }
        }






        //private readonly static Dictionary<int, Action<IList>> arrayModificationAction = new Dictionary<int, Action<IList>>();
        private readonly static Dictionary<int, IList> arrayModificationAction = new Dictionary<int, IList>();



        /// <summary>
        /// Cette fonction sert à afficher le champ associé à une valeur, peu importe si elle est serializable ou non
        /// </summary>
        /// <param name="property">La valeur a afficher</param>
        /// <param name="content">Le texte à aficher</param>
        /// <returns></returns>
        public static T DrawPropertyOject<T>(T property, GUIContent content, params GUILayoutOption[] options)
        {
            return (T)DrawPropertyOject((object)property, content, typeof(T), options);
        }




        /// <summary>
        /// Cette fonction sert à afficher le champ associé à une valeur, peu importe si elle est serializable ou non
        /// </summary>
        /// <param name="property">La valeur a afficher</param>
        /// <param name="content">Le texte à aficher</param>
        /// <param name="datatype">Le type d ela valeur (histoire que meme un type null puisse avoirun affichage)</param>
        /// <returns></returns>
        public static object DrawPropertyOject(object property, GUIContent content, Type datatype = null, params GUILayoutOption[] options)
        {


            if (property == null && datatype == null) return null;

            if (property != null && datatype == null)
                datatype = property.GetType();

            if (property == null && datatype != null)
            {
                if (datatype.IsValueType)                
                    property =  Activator.CreateInstance(datatype);
                
            }


            if (datatype.Is(typeof(UnityEngine.Object))) return EditorGUILayout.ObjectField(content, property as UnityEngine.Object, datatype, false, options);
            else if (datatype.Is(typeof(Enum))) return EditorGUILayout.EnumPopup(content, (Enum)property, options);
            else if (datatype == typeof(bool)) return EditorGUILayout.Toggle(content, (bool)property, options);
            else if (datatype == typeof(int)) return EditorGUILayout.IntField(content, (int)property, options);
            else if (datatype == typeof(float)) return EditorGUILayout.FloatField(content, (float)property, options);
            else if (datatype == typeof(string)) return EditorGUILayout.TextField(content, (string)property, options);
            else if (datatype == typeof(Vector2)) return EditorGUILayout.Vector2Field(content, (Vector2)property, options);
            else if (datatype == typeof(Vector2Int)) return EditorGUILayout.Vector2IntField(content, (Vector2Int)property, options);
            else if (datatype == typeof(Vector3)) return EditorGUILayout.Vector2Field(content, (Vector3)property, options);
            else if (datatype == typeof(Vector3Int)) return EditorGUILayout.Vector3IntField(content, (Vector3Int)property, options);

            else if (datatype.Is(typeof(IEnumerable)))
            {


                int id = (content?.text?.GetHashCode() ?? 1) * (datatype?.GetHashCode() ?? 1);



                #region Array Edition


                var castedList = (IList)property;



                Type internalArrayType = datatype.GetGenericArrayElementType();


                Type listType = typeof(List<>).MakeGenericType(new[] { internalArrayType });
                IList list = (IList)Activator.CreateInstance(listType);




                EditorGUILayout.LabelField(content.text);
                EditorGUI.indentLevel++;
                var s = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 60;



                void ModifyArray(IList newList)
                {
                    arrayModificationAction.AddOrUpdate(id, newList);
                }


                if (arrayModificationAction.ContainsKey(id))
                {
                    if (Event.current.type == EventType.Layout)
                    {
                        list = arrayModificationAction[id];
                        arrayModificationAction.Remove(id);
                        GUI.changed = true;
                    }
                }
                else foreach (var e in (IEnumerable)property)
                        list.Add(e);



                for (int i = 0; i < list.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    list[i] = DrawPropertyOject(list[i], new GUIContent($"[{i}]"), internalArrayType, options);


                    GUILogiked.Panels.DrawArrayElementContextButton(list, i, ModifyArray);

                    EditorGUILayout.EndHorizontal();

                }




                #endregion





                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILogiked.Panels.GUIDrawEditorIcon(() =>
                {
                    if (list.Count == 0) list.Add(internalArrayType.IsValueType ? Activator.CreateInstance(internalArrayType) : null); else list.Add(list[list.Count - 1]);

                }, GUILogiked.Panels.EditorIconType.AddItem);
                EditorGUILayout.EndHorizontal();


                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = s;





                //On convertis dynamiquement notre ILIST vers le type qu'il faut

                if (datatype.Is<Array>())
                {
                    return list.CastListToArray();
                }
                //  else  Debug.Log("Is LIST !");                



                return list;

            }
            else
                EditorGUILayout.LabelField("not implemented");


            return null;
        }




        /// <summary>
        /// Searches for custom property drawer for given property, or returns null if no custom property drawer was found.
        /// </summary>
        public static PropertyDrawer FindDrawerForProperty(SerializedProperty property)
        {
            PropertyDrawer drawer;
            TypeAndFieldInfo tfi;





            int pathHash = _GetUniquePropertyPathHash(property);

            if (!s_PathHashVsType.TryGetValue(pathHash, out tfi))
            {
                tfi.type = _GetPropertyType(property, out tfi.fi);
                s_PathHashVsType[pathHash] = tfi;
            }

            if (tfi.type == null)
                return null;


            if (!s_TypeVsDrawerCache.TryGetValue(tfi.type, out drawer))
            {
                drawer = FindDrawerForType(tfi.type);
                s_TypeVsDrawerCache.Add(tfi.type, drawer);
            }

            if (drawer != null)
            {
                // Drawer created by custom way like this will not have "fieldInfo" field installed
                // It is an optional, but some user code in advanced drawer might use it.
                // To install it, we must use reflection again, the backing field name is "internal FieldInfo m_FieldInfo"
                // See ref file in UnityCsReference (2019) project. Note that name could changed in future update.
                // unitycsreference\Editor\Mono\ScriptAttributeGUI\PropertyDrawer.cs
                var fieldInfoBacking = typeof(PropertyDrawer).GetField("m_FieldInfo", BindingFlags.NonPublic | BindingFlags.Instance);
                if (fieldInfoBacking != null)
                    fieldInfoBacking.SetValue(drawer, tfi.fi);
            }

            return drawer;
        }

        /// <summary>
        /// Gets type of a serialized property.
        /// </summary>
        private static Type _GetPropertyType(SerializedProperty property, out FieldInfo fi)
        {

            // To see real property type, must dig into object that hosts it.
            fi = GetPropertyFieldInfo(property);
            return fi?.FieldType;
        }

        /// <summary>
        /// For caching.
        /// </summary>
        private static int _GetUniquePropertyPathHash(SerializedProperty property)
        {
            int hash = property.serializedObject.targetObject.GetType().GetHashCode();
            hash += property.propertyPath.GetHashCode();
            return hash;
        }


        public static FieldInfo GetPropertyFieldInfo(SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            string[] fullPath = property.propertyPath.Split('.');
            var bind = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Default;
            FieldInfo fi = parentType.GetField(fullPath[0], bind); // NonPublic to support [SerializeField] vars



            //Ce code sert à creuser dans les sous objets afin d'atteindre la propriété
            for (int i = 1; i < fullPath.Length; i++)
            {

                if (fi == null) return null;//Si il n'arrive pas a creuser, on cancel.

                Type fieldType = null;
                //like `tiles.Array.data[0].tilemodId`


                if (fullPath[i] == "Array")
                {

                    try
                    {

                        //Marche seulement avec les listes/collections
                        var arg = fi.FieldType.GetGenericArguments();
                        if (arg.Length > 0)
                            fieldType = arg.Single();

                        if (fieldType == null)
                            //Marche seulement avec les array
                            fieldType = fi.FieldType.GetElementType();


                    }
                    catch (Exception e)
                    {
                        fieldType = null;
                        Debug.LogException(e);
                    }

                    i += 2;

                    if (fieldType == null)
                    {
                        Debug.LogError($"Element type of array { fi.FieldType.ToString()} not found ! {fullPath.ToStringArray(".")}");
                        return null;
                    }


                    if (i == fullPath.Length) i--;


                }
                else
                {
                    fieldType = fi.FieldType;
                }

                fi = fieldType.GetField(fullPath[i], bind);
            }

            return fi;
        }

        static bool _IsArrayPropertyPath(string[] fullPath, int i)
        {
            // Also search for array pattern, thanks user https://gist.github.com/kkolyan
            // like `tiles.Array.data[0].tilemodId`
            // This is just a quick check, actual check in Unity uses RegEx
            if (fullPath[i] == "Array" && i + 1 < fullPath.Length && fullPath[i + 1].StartsWith("data"))
                return true;
            return false;
        }

        /// <summary>
        /// Stolen from unitycsreference\Editor\Mono\ScriptAttributeGUI\ScriptAttributeUtility.cs
        /// </summary>
        static bool _IsListType(Type t, out Type containedType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
            {
                containedType = t.GetGenericArguments()[0];
                return true;
            }

            containedType = null;
            return false;
        }

        /// <summary>
        /// Returns custom property drawer for type if one could be found, or null if
        /// no custom property drawer could be found. Does not use cached values, so it's resource intensive.
        /// </summary>
        public static PropertyDrawer FindDrawerForType(Type propertyType)
        {



            var cpdType = typeof(CustomPropertyDrawer);
            FieldInfo typeField = cpdType.GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo childField = cpdType.GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

            // Optimization note:
            // For benchmark (on DungeonLooter 0.8.4)
            // - Original, search all assemblies and classes: 250 msec
            // - Wappen optimized, search only specific name assembly and classes: 5 msec

            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Wappen optimization: filter only "*Editor" assembly
                //    if (!assem.FullName.Contains("Editor"))
                //        continue;

                foreach (Type candidate in assem.GetTypes())
                {
                    // Wappen optimization: filter only "*Drawer" class name, like "SomeTypeDrawer"

                    // if (candidate.Name.Contains("Lstring"))
                    //     Debug.LogError("ou");


                    if (!typeof(PropertyDrawer).IsAssignableFrom(candidate))//Name.Contains("Drawer"))
                        continue;



                    // See if this is a class that has [CustomPropertyDrawer( typeof( T ) )]
                    foreach (Attribute a in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                    {
                        if (a.GetType().IsSubclassOf(typeof(CustomPropertyDrawer)) || a.GetType() == typeof(CustomPropertyDrawer))
                        {

                            CustomPropertyDrawer drawerAttribute = (CustomPropertyDrawer)a;
                            Type drawerType = (Type)typeField.GetValue(drawerAttribute);

                            if (drawerType == propertyType ||
                                (bool)childField.GetValue(drawerAttribute) && propertyType.IsSubclassOf(drawerType) ||
                                (bool)childField.GetValue(drawerAttribute) && IsGenericSubclass(drawerType, propertyType))
                            {
                                if (candidate.IsSubclassOf(typeof(PropertyDrawer)))
                                {
                                    // Technical note: PropertyDrawer.fieldInfo will not available via this drawer
                                    // It has to be manually setup by caller.
                                    var drawer = (PropertyDrawer)Activator.CreateInstance(candidate);
                                    return drawer;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns true if the parent type is generic and the child type implements it.
        /// </summary>
        private static bool IsGenericSubclass(Type parent, Type child)
        {
            if (!parent.IsGenericType)
            {
                return false;
            }

            Type currentType = child;
            bool isAccessor = false;
            while (!isAccessor && currentType != null)
            {
                if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == parent.GetGenericTypeDefinition())
                {
                    isAccessor = true;
                    break;
                }
                currentType = currentType.BaseType;
            }
            return isAccessor;
        }

    }
}
#endif