using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace logiked.source.extentions
{



    /// <summary>
    /// Extentions pour les classes qui font de la reflection
    /// </summary>
    public static class ReflectExtention
    {


        /// <summary>
        /// [WIP, peut-être un peu con]
        /// [TODO]: utiliser des members au lieu de field/Property etc
        /// Conteneur dynamique pour une valeur obtenue par reflection. Peut être issu d'un champ ou d'une propiété. Appeler <see cref="Value"/> pour récuperer la valeure actuelle du pointeur.
        /// </summary>
        public class ReflectedObject
        {
            private readonly PropertyInfo prop;
            private readonly FieldInfo field;
            private object obj;
            private Type type;

            private bool isArrayValue;//Valeur d'un element dans un array

            public Type Type => type;


            public bool ValueNotFound => Type == null && Value == null;

            public void SetValue(object value)
            {



                if (isArrayValue)// || ( value != null && value is ICollection))
                {
                    Debug.LogError("Array element editing currently not suported.");
                }

                if (field != null)
                {
                    field.SetValue(obj, value);
                }
                else if (prop != null)
                    prop.SetValue(obj, value);

            }

            /*
            internal ReflectedObject(object obj, Type type, bool isArrayValue) : this(obj, null)
            {
                this.isArrayValue = isArrayValue;
                this.type = type;
            }*/

            internal ReflectedObject(object obj, object propertyOrField)
            {


                prop = propertyOrField as PropertyInfo;
                field = propertyOrField as FieldInfo;

                type = prop?.PropertyType ?? field?.FieldType;

                this.obj = obj;
            }


            public object Value
            {
                get
                {
                    if (obj == null) return null;
                    if (isArrayValue) return obj;
                    if (prop != null) return prop.GetValue(obj);
                    if (field != null) return field.GetValue(obj);
                    return null;
                }
            }
        }




        private static readonly Regex ArrayRegex = new Regex(@".*(\[([0-9]+)\])");

        static MethodInfo arrayGetElementMethod;

        static MethodInfo ArrayGetElementMethod
        {
            get
            {

                if (arrayGetElementMethod == null)
                {
                    Type t = typeof(Array);

                    arrayGetElementMethod = t.GetMethod("GetValue",
                             BindingFlags.Public |
                             BindingFlags.NonPublic |
                             BindingFlags.Default |
                             BindingFlags.Static |
                             BindingFlags.FlattenHierarchy |
                             BindingFlags.Instance);
                }


                return arrayGetElementMethod;
            }
        }




        /// <summary>
        /// Retourne par reflection le champ/propiété et sa valeur stockés dans l'objet.
        /// </summary>
        /// <param name="obj">L'objet sur lequel est appelé la fonction</param>
        /// <param name="path">Le chemin relatif vers la valeur depuis l'objet. Exemple : "gameObject.transform.position" pour un Obj de type GameObject.</param>
        /// <param name="canBeSingletonClass">Autoriser l'acces à un singleton/ autre classe statiques.</param>
        /// <returns>Wrapper de la valeur obtenue. (peut etre null)</returns>
        public static ReflectedObject GetReflectedValueAtPath(this object obj, string path, bool canBeSingletonClass = true)
        {
            if (obj == null || path.IsNullOrEmpty()) return null;



            var paths = path.Split(new[] { '.' }, 2);
            var currentSearchdName = paths.First();
            var reg = ArrayRegex.Match(currentSearchdName);
            int arrayIndex = reg.Success ? int.Parse(reg.Groups[2].Value) : -1;



            if (arrayIndex >= 0 || reg.Success)
                currentSearchdName = currentSearchdName.Replace(reg.Groups[1].Value, "");


            Type type;
            FieldInfo inf;
            PropertyInfo prop;
            object end = null;
            object selectedPropType = null;







            /////FIND


            type = obj.GetType();

            if (canBeSingletonClass)//Singleton
            {
                var singletonName = GetTypeBySimpleName(currentSearchdName);
                if (singletonName != null && paths.Length > 1)
                {
                    type = singletonName;
                    obj = null;
                    paths = paths[1].Split(new[] { '.' }, 2);
                    currentSearchdName = paths.First();

                }
            }


            //            Debug.LogError(type.Name);
            //           Debug.LogError(currentSearchdName);


            ///Field test

            Type saveType = type;

            while ((inf = type.GetField(currentSearchdName,
                     BindingFlags.Public |
                     BindingFlags.NonPublic |
                     BindingFlags.Default |
                     BindingFlags.Static |
                     BindingFlags.FlattenHierarchy |
                     BindingFlags.Instance)) == null && type.BaseType != null)
            {
                type = type.BaseType;
            }

            if (inf != null)
            {
                end = inf.GetValue(obj);
                selectedPropType = inf;
                goto end;
            }


            ////Poperty test


            type = saveType;

            while ((prop = type.GetProperty(currentSearchdName,
                     BindingFlags.Public |
                     BindingFlags.NonPublic |
                     BindingFlags.Default |
                     BindingFlags.Static |
                     BindingFlags.FlattenHierarchy |
                     BindingFlags.Instance)) == null && type.BaseType != null)
            {
                type = type.BaseType;
            }

            if (prop != null)
            {
                end = prop.GetValue(obj, new object[0]);
                selectedPropType = prop;

                goto end;
            }





        ///USE :
        end:





            // Debug.LogErrorFormat("{0} == \n {1} \n get {2}", obj, end, currentSearchdName);

            if (end != null)
            {

                // && end.GetType().IsArray)
                if (arrayIndex >= 0)
                {


                    //Cette crétinerie aurait pu etre évitée si Unity avait accés au C# 6.0, avec Linq.ElementAt<value>
                    // end.GetType().DebugClassContentReflection();

                    IEnumerable enumerable = end as IEnumerable;
                    int c = 0;
                    end = null;

                    if (enumerable != null)
                    {
                        foreach (var item in enumerable)
                        {
                            if (c == arrayIndex)
                            {
                                end = item;
                                break;
                            }
                            c++;
                        }

                        if (paths.Length == 1)
                            return new ReflectedObject(end, true);
                    }

                    ///////////////////////////////////////////////////////////////////////////////////////////


                }


                if (paths.Length > 1)
                    return end.GetReflectedValueAtPath(paths[1], false);


            }



            return new ReflectedObject(obj, selectedPropType);
        }






        /// <summary>
        /// [WIP]Set la valeur d'un champ de l'objet spécifié
        /// </summary>
        /// <typeparam name="T">le type</typeparam>
        /// <param name="obj">L'objet</param>
        /// <param name="name">Le nom du champ</param>
        /// <param name="val">la nouvelle valeur</param>
        public static void SetFieldValue<T>(object obj, string name, T val)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(obj, val);
        }


        /// <summary>
        /// [WIP]Get la valeur d'un champ de l'objet spécifié
        /// </summary>
        /// <typeparam name="T">le type</typeparam>
        /// <param name="obj">L'objet</param>
        /// <param name="name">Le nom du champ</param>
        /// <returns>La valeur obtenue (null si rien n'a été trouvé)</returns>
        public static T GetFieldValue<T>(object obj, string name)
        {
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field?.GetValue(obj);
        }

        /// <summary>
        /// Montre tout les membres (statiques/private ou non) d'une classe dans la console unity.
        /// </summary>
        /// <param name="t">Le type sur lequel est appelé la fonction</param>
        public static void DebugClassContentReflection(this Type t)
        {
#if !UNITY_EDITOR
return;
#endif


            List<string> methods = new List<string>();
            List<string> properties = new List<string>();
            List<string> members = new List<string>();

            StringBuilder final = new StringBuilder();


            void PrintMethodHeader(MethodInfo m, StringBuilder sel)
            {
                if (m.IsPublic)
                    sel.Append("public");
                else if (m.IsPrivate)
                    sel.Append("private");
                else
                    sel.Append("internal");
                sel.Append(" ");


                if (m.IsStatic)
                {
                    sel.Append("static");
                    sel.Append(" ");
                }
                else if (m.IsVirtual)
                {
                    sel.Append("virtual");
                    sel.Append(" ");
                }

                sel.Append(m.ReturnType.Name);
                sel.Append(" ");
                sel.Append(m.Name);
            }


            void PrintMethod(MethodInfo m)
            {
                StringBuilder sel = new StringBuilder();
                PrintMethodHeader(m, sel);
                sel.Append("(");
                var parms = m.GetParameters();
                for (int i = 0; i < parms.Length; i++)
                    sel.Append(parms[i].ParameterType.Name + " " + parms[i].Name + (i == parms.Length - 1 ? "" : ", "));

                sel.Append(");");
                methods.Add(sel.ToString());
            }


            void PrintProperty(PropertyInfo m)
            {
                StringBuilder sel = new StringBuilder();
                sel.Append(m.GetMethod.ReturnType.Name);
                sel.Append(" ");
                sel.Append(m.Name);
                sel.Append(";");
                properties.Add(sel.ToString());
            }

            void PrintField(FieldInfo m)
            {
                StringBuilder sel = new StringBuilder();
                if (m.IsPublic)
                    sel.Append("public");
                else if (m.IsPrivate)
                    sel.Append("private");
                else
                    sel.Append("internal");
                sel.Append(" ");

                if (m.IsStatic)
                {
                    sel.Append("static");
                    sel.Append(" ");
                }

                sel.Append(m.FieldType.Name);
                sel.Append(" ");
                sel.Append(m.Name);
                sel.Append(";");
                properties.Add(sel.ToString());
            }




            var met = t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            {
                foreach (var m in met)
                    PrintMethod(m);
            }




            var pro = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            {
                foreach (var m in pro)
                    PrintProperty(m);
            }

            var memb = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            {
                foreach (var m in memb)
                    PrintField(m);
            }

            methods.Sort();
            properties.Sort();
            members.Sort();

            final.Append(string.Join("\n", methods));
            final.Append("\n\n");
            final.Append(string.Join("\n", properties));

            final.Append("\n\n");
            final.Append(string.Join("\n", members));
            final.Append("\n\n");

            Debug.Log(final.ToString());
        }


        private static Dictionary<string, Type> allTypeList = null;

        /// <summary>
        /// Obtenir un type par son nom, de manière simplifié (pas le nom de package complet). Ne gère pas plusieurs classes portant le même nom.
        /// </summary>
        /// <param name="name">Le nom de la classe à chercher</param>
        /// <returns>Le type (ou null)</returns>
        public static Type GetTypeBySimpleName(string name)
        {
            int i;

            if (allTypeList == null)
            {
                allTypeList = new Dictionary<string, Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
                {
                    var t = assembly.GetTypes();

                    //var tt = assembly.GetType(name);

                    for (i = 0; i < t.Length; i++)
                    {

                        if (allTypeList.ContainsKey(t[i].Name))
                        {

                            //  Debug.LogWarning($"Deux classes portent le même nom : {t[i].Name}");
                        }
                        else
                            allTypeList.Add(t[i].Name, t[i]);
                    }
                }
            }


            if (allTypeList.ContainsKey(name))
            {
                return allTypeList[name];
            }

            return null;
        }


        /// <summary>
        /// [WIP] Obtenir un type par son nom, (Fullname ou assembly qualified name).
        /// </summary>
        /// <param name="name">Le nom du type à chercher</param>
        /// <returns>Le type (ou null)</returns>
        public static Type ByName(string name)
        {
            var t = Type.GetType(name);
            if (t != null) return t;



            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }


        /// <summary>
        /// Verifie si un type est herité d'un autre. Raccourci pour IsAssignableFrom
        /// </summary>
        /// <param name="type">Le type à verifier</param>
        /// <param name="motherClass">La classe mère de ce type</param>
        /// <returns>Le type herite-t-il de la classe mère ?</returns>
        public static bool Is(this Type type, Type motherClass)
        {
            return motherClass.IsAssignableFrom(type);
        }
        /// <summary>
        /// Verifie si un type est herité d'un autre. Raccourci pour IsAssignableFrom
        /// </summary>
        /// <param name="type">Le type à verifier</param>
        /// <typeparam name="T">La classe mère de ce type</param>
        /// <returns>Le type herite-t-il de la classe mère ?</returns>
        public static bool Is<T>(this Type type) where T : class
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <summary>
        /// Verifie si un type de l'objet est herité d'un autre. Raccourci pour GetType().IsAssignableFrom()
        /// </summary>
        /// <param name="obj">L'objet avec le type à verifier</param>
        /// <typeparam name="T">La classe mère de ce type</param>
        /// <returns>Le type herite-t-il de la classe mère ?</returns>
        public static bool IsTypeOf<T>(this object obj) where T : class
        {
            if (obj == null) return false;

            if(obj is Type)
            {
                return typeof(T).IsAssignableFrom((Type)obj);
            }

            var t = obj.GetType();
            return typeof(T).IsAssignableFrom(t);
        }



        /// <summary>
        /// Verifier si un type est un array ou une liste
        /// <returns>Le type (ou null)</returns>
        public static bool IsGenericArray(this Type type)
        {
            return type.Is<IList>() && type.Is<IEnumerable>();
        }

        /// <summary>
        /// Retourne le type des éléments de cette classe, si elle est un Array ou une liste.
        /// <returns>Le type</returns>
        public static Type GetGenericArrayElementType(this Type type)
        {
            if (type.Is(typeof(Array)))
                return type.GetElementType();
            return type.GenericTypeArguments[0];


        }

        /// <summary>
        /// Convertis dynamiquement la liste en array
        /// <param name="list">La liste à convertir</param>
        /// <returns>L'array converti</returns>
        public static Array CastListToArray(this IList list)
        {
            if (list == null) return null;

            if (list.GetType().Is<System.Array>()) return (System.Array)list;

            var internalArrayType = list.GetType().GetGenericArrayElementType();
            Array destinationArray = Array.CreateInstance(internalArrayType, list.Count);
            var arrayCast = Enumerable.Range(0, list.Count).Select(i => list[i]).ToArray();
            Array.Copy(arrayCast, destinationArray, destinationArray.Length);
            return destinationArray;
        }


    
        /// <summary>
        /// Retourne la valeur du MemberInfo, si c'est un Field ou une Property
        /// </summary>
        /// <param name="member">Le membre</param>
        /// <param name="instance">L'instance de la classe concernée</param>
        /// <returns>La valeur du membre sur la classe</returns>
        public static object GetValuePropertyOrField(this MemberInfo member, object instance )
        {
            Assert.IsNotNull(member, "Member cannot be null !");
            
            if (member.MemberType == MemberTypes.Field)
            {
                var prop = (FieldInfo)member;
                return prop.GetValue(instance);
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                var prop = (PropertyInfo)member;
                var res = prop.GetValue(instance);
                return res;
            }
            
            throw new ArgumentException($"Member must be a Field or a Property. Current member {member.Name} is a {member.MemberType}", nameof(member) );
        }




        /// <summary>
        /// Retourne le type du MemberInfo, si c'est un Field ou une Property
        /// </summary>
        /// <param name="member">Le membre</param>
        /// <returns>Le type du membre</returns>
        public static Type GetTypePropertyOrField(this MemberInfo member )
        {
            Assert.IsNotNull(member, "Member cannot be null !");

            if (member.MemberType == MemberTypes.Field)
            {
                return ((FieldInfo)member).FieldType; ;
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                return ((PropertyInfo)member).PropertyType;
            }

            throw new ArgumentException($"Member must be a Field or a Property. Current member {member.Name} is a {member.MemberType}", nameof(member));
        }






    }

}

