#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using logiked.source.extentions;
using logiked.source.editor;
using System;
using Object = UnityEngine.Object;


using UnityEditor;


namespace logiked.editor.navigation
{

    public static class AssetNavigationUtility
    {
                
        private static System.Type projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
        private static EditorWindow projectBrowser => EditorWindow.GetWindow(projectBrowserType);



        /// <summary>
        /// Certaines propriétés des <see cref="Object"/> unity ne doivent surtout pas être récupérés, car elles génèrent des Assets. 
        /// </summary>
        private static readonly Dictionary<Type, string[]> GetAllConnectedObjects_CancelUnityInternalProperties = new Dictionary<Type, string[]>()
        {
            { typeof(Renderer), new string[] { nameof(Renderer.material), nameof(Renderer.materials) } },
            { typeof(MeshFilter), new string[] { nameof(MeshFilter.mesh) } },
            { typeof(Component), new string[] { nameof(Component.gameObject) } },
        };

        /// <summary>
        /// GetAllConnectedObjects: Fonction pour suprimer des types pas interessants dans la methode <see cref="GetAllConnectedObjects"/> (types Systèmes, Transform, etc)
        /// </summary>
        /// <returns>True si le type est à exclure</returns>
        private static bool GACO_ExcludeType(Type t)
        {
            return t.FullName.StartsWith("System.") || t.IsPrimitive || t.IsEnum || t.Is<Transform>() || t.Is(typeof(Matrix4x4));
        }

        /// <summary>
        /// GetAllConnectedObjects: Condition pour continuer d'iterer en profondeur sur les types 
        /// </summary>
        private static bool GACO_TypeIteration(Type t)
        {
            return t != null && t != typeof(UnityEngine.MonoBehaviour) && t != typeof(UnityEngine.Object) && t != typeof(object);            
        }


        //GACO:
        /// <summary>
        /// Retourne l'intégralité des <see cref="UnityEngine.Object"></see> définis en tant que varibale dans cette ressource.
        /// </summary>
        /// <param name="resource">L'objet à analyser</param>
        /// <returns>Les objets connectés</returns>
        public static Object[] GetAllConnectedObjects(this Object resource)
        {
            HashSet<object> done = new HashSet<object>();
           
            done.Add(resource);//Parce qu'on ne veut pas avoir notre resource dans le résultat        

            var result = GetAllConnectedObjects_Rec(resource as System.Object, null, resource.name, done);

            if (!result.Contains(resource)) result.Add(resource); //On Ajoute l'objet de base

            return result.Distinct().ToArray();//Supression des doublons
        }


        //GACO:
        /// <summary>
        /// Parcour en profondeur des Fields définis dans la resource jusqu'a trouver des <see cref="Object"/>
        /// </summary>
        /// <param name="alreadyDone">Liste des champs déja parcourus</param>
        private static List<Object> GetAllConnectedObjects_Rec(object resource, MemberInfo actInfo, string path, HashSet<object> alreadyDone)
        {
            List<Object> end = new List<Object>();


            //Condition d'arret

            if (resource == null) return end;

            string code = actInfo?.MetadataToken.ToString() + resource?.GetHashCode();

            if (alreadyDone.Contains(code))
                return end;

            alreadyDone.Add(code);


            //Recup du type du Membre

            var propertyType = actInfo?.GetTypePropertyOrField() ?? resource?.GetType();
            var allFieldsList = new List<MemberInfo>();


            if (propertyType.IsGenericArray())   //Si la ressoource est un array 
            {

                if (GACO_ExcludeType(propertyType.GetGenericArrayElementType()))//On check si le contenu de l'array à un interet
                    return end;

                IList tabs = resource as IList;//On cast l'array et on itère sur ses élèments
                object element;

                for (int i = 0; i < tabs.Count; i++)
                {
                    element = tabs[i];
                    end.AddRange(GetAllConnectedObjects_Rec(element, null, $"{path}[{i}]", alreadyDone));
                }
                return end;
            }
            else if (propertyType.Is<GameObject>()) // Si c'est un gameObject on récupere tout ses Components
            {
                GameObject obj = resource as GameObject;
                var comps = obj.GetComponentsInChildren<Component>(true);


                for (int i = 0; i < comps.Length; i++) //Itération sur les composants
                {
                    if (comps[i] == null || comps[i].GetType().Is<Transform>()) continue;
                    end.AddRange(GetAllConnectedObjects_Rec(comps[i], null, $"{path}[{i}]", alreadyDone));
                }
                return end;

            }
            else //Si le type de la resource on fait ubne étude approfondie de ses Fields l'objet
            {


                //On s'autorise à lire des propriétés sur l'objet seulement si elles sont définis chez UnityEngine.
                //En fait, il existe certain Assets qui sont référencés stoqués en C++ dans le code source de l'éditeur.
                //Par exemple, Renderer.sharedMaterial ou SpriteRenderer.sprite n'ont pas de champs associés en C#. 
                //Ce sont pourtant des valeurs interessantes, on récupère ces données ici :
                IEnumerable<PropertyInfo> propertyReadingSecure;

                var currentType = propertyType;

                //La fonction GetProperties fonctionne sur les classes parents. Ainsi il faut qu'elle ne s'execute que 1 seule fois.
                bool alreadyCollectProperties = false;

                do
                {

                    if (!alreadyCollectProperties && currentType.Assembly.FullName.StartsWith("UnityEngine."))
                    {
                        propertyReadingSecure = currentType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                        propertyReadingSecure = propertyReadingSecure.Where(m => m.PropertyType.Is<Object>());//Si c'est un unityObject

                        propertyReadingSecure = propertyReadingSecure.Where(m => !(GetAllConnectedObjects_CancelUnityInternalProperties.GetOrDefault(m.DeclaringType)?.Contains(m.Name) ?? false));//Si la propriété n'est pas toxique et ne va pas génerer de la merde

                        propertyReadingSecure = propertyReadingSecure.Where(m => !m.IsDefined(typeof(System.ObsoleteAttribute), false)); //On suprime les propriétés obsoletes

                        allFieldsList.AddRange(propertyReadingSecure);

                        alreadyCollectProperties = true;
                    }

                    //Récupération de tout les champs
                    allFieldsList.AddRange(currentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));

                } while (GACO_TypeIteration(currentType = currentType.BaseType));


            }


            var allFields = allFieldsList.Where(m => !GACO_ExcludeType(m.GetTypePropertyOrField()));//Quand le champ est d'un type correct

            //Recuperation des Objects
            var epurated = allFields.Where(m => m.GetTypePropertyOrField().Is<UnityEngine.Object>());

            //Recuperation des array d'Objets
            var epurated_Array = allFields.Where(m => m.GetTypePropertyOrField().IsGenericArray() && m.GetTypePropertyOrField().GetGenericArrayElementType().Is<UnityEngine.Object>());


            Object cast = null;

            foreach (var obj in epurated)
            {
                try
                {
                    cast = obj.GetValuePropertyOrField(resource) as Object;

                    if (cast != null)//On prends pas les objets nulls
                        end.Add(cast);
                }
                catch (Exception e)
                {
                    Debug.Log("Skipped item...");
                    Debug.LogException(e);
                }
            }


            //Iteration sur les arrays

            foreach (var item in epurated_Array)
            {
                IList castArrays = (IList)item.GetValuePropertyOrField(resource);

                if (castArrays != null)
                    foreach (Object endObject in castArrays)
                        if (endObject != null)
                            end.Add(endObject);//On prends pas les objets nulls
            }


            //Récursion sur tout les autres champs récupérés qui sont surement classes, des structs, des listes de classes... etc.

            var iterationArray = allFields.Except(epurated.Union(epurated_Array));

            object val;

            foreach (var item in iterationArray)
            {
                try
                {
                    val = item.GetValuePropertyOrField(resource);
                    // if (item.Name == actInfo?.Name && val?.ToString() == resource?.ToString()) continue; //Ciclic value access

                    end.AddRange(GetAllConnectedObjects_Rec(val, item, $"{path}.{item.Name}", alreadyDone));
                }
                catch (Exception e)
                {
                    Debug.Log("Handled error");
                    Debug.LogException(e);
                }
            }

            return end;
        }

    }
}

#endif


