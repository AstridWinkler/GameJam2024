using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using logiked.source.utilities;
using logiked.source.extentions;
using System.Reflection;



#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.source.attributes.root
{




    /// <summary>
    /// Attribut gérénique permettant de simplifier l'implémentation de plusieurs Drawers sur un meme champ. Peut etre override sur vos custom drawers
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FutureFieldAttribute : PropertyAttribute
    {


        /// <summary>
        /// [WIP TEST] :Défini comment cet attribut se comporte quand il est disposé sur un array
        /// </summary>
        public enum AttributeArrayInteraction { ApplyOnEachElement = 0, ApplyOnFirstElement = 1, ApplyOnLastElement = 2
        }

        /// <summary>
        ///  [WIP TEST] : Défini comment cet attribut se comporte quand il est disposé sur un array
        /// </summary>
        public virtual AttributeArrayInteraction ArrayInteractionMode => AttributeArrayInteraction.ApplyOnEachElement;



        /// <summary>
        /// Liste de tout les attributs stockés
        /// </summary>
        public List<object> stored = new List<object>();

        public FutureFieldAttribute tempNextDrawer;

        public bool propertyAlreadyDrawn;

        public int propertyIndex=0;//Temp property index

        int baseOrder;



        public class AttributeContext
        {
            public float AttributeHeight = 0;
            public Dictionary<string, object> datas = new Dictionary<string, object>();
          


            public T GetData<T>(string name) 
            {
                if (!datas.ContainsKey(name))
                    datas.Add(name, default(T));
                return (T)datas[name];
            }
            public void SetData<T>(string name, T value)
            {
                    datas.AddOrUpdate(name, value);
            }


        }



        public FutureFieldAttribute()
        {
            baseOrder = order;
        }




#if UNITY_EDITOR

        /// <summary>
        /// Event Editeur Unity pour tracer la propriété à l'écran
        /// </summary>
        /// <param name="position">Espace alloué à la propriété</param>
        /// <param name="property">La propritété</param>
        /// <param name="label">Le label affiché devant la propriété</param>
        protected abstract void OnGUIRecursive(Rect position, UnityEditor.SerializedProperty property, GUIContent label, AttributeContext Context);

       static  Dictionary<string, bool> recSauce = new Dictionary<string, bool>();

        public string Code(SerializedProperty property ) {
           return $"{property.propertyPath} {GetType().Name} {propertyIndex}";
        }



        private readonly bool logInformations = false;


        public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var Context = GetContext(property);

            var code = Code(property);//Id de cet attribut + propriété

            /*
            switch(ArrayInteractionMode)
            {
                case AttributeArrayInteraction.ApplyOnFirstElement:
                    if (property.IsInArray(PropertyExtensioin.IsInArrayMode.PropertyIsAnArrayElement))
                    {
                        if (property.propertyPath.Substring(property.propertyPath.Length - 3) != "[0]")
                        {
                            CallNextAttribute(position, property, label);//Skip attribute
                            return;
                        }
                    }
                    break;


                case AttributeArrayInteraction.ApplyOnLastElement:

                    break;




            }*/


            //Eviter l'abrutie de recursion que unity fait quand plusieurs attributs sont présents sur le même champ
            var rec = recSauce.GetValueOrDefault(code);





            if (rec)
            {

                if (logInformations)
                    Debug.LogWarning("rec:" + code + " " + code.GetHashCode());

                // Debug.Log(code.GetHashCode());




                //Debug.Log("rec");
                //position.height =  32;
                try
                {

                    //  property.DrawPropertyField(position, label);
                    CallNextAttribute(position, property, label);

                }
                catch (Exception e)
                {
                    if (logInformations)

                        Debug.Log("Exeption: " + code + "\n" + e);

                }
                //property.DrawPropertyField(position, label);
                // propertyAlreadyDrawn = true;


                recSauce.AddOrUpdate(code, false);
                //test, danger de mort
                //Je ne comprends pas comment ca peut marcher, si le truc doit faire 4 recusrions et que à la deuxieme iteration la sécurité recursion est levée,
                ////ca devrait causer encre 2 merdes. Mais visiblement c'est OK

                return;
            }









            recSauce.AddOrUpdate(code, true);

            if (logInformations)
                Debug.Log("Process " + code);

            try
            {
                OnGUIRecursive(position, property, label, Context);
            }
            catch (Exception gui)
            {
                Debug.LogWarning("Gui Refresh Exception:\n" + gui.Message);
            }

            if (logInformations)
                Debug.Log("Closing " + code);

            recSauce.AddOrUpdate(code, false);


        






        }


        public void ResetDynamicValues()
        {
        //    currentFieldHeight = 0;
            propertyAlreadyDrawn = false;
        }

        /// <summary>
        /// Voilà la démarche : le but serait de se passer de la redéfinition 
        /// d'un GetHeight() systématique, en créant une variable qui servierais à ca. Cette variable
        /// serait alors incrémentable par l'utilisateur dans le OnGUI.<br></br><br></br>
        /// 
        /// Problème 1) :<br></br>
        /// unity sert du GetHeight pour définir l'espace que va prendre la property avant d'appeler le OnGUI.
        /// Il faut donc que 1 frame s'écoule avant d'avoir la taille qui correspond. 
        /// (Frame 1 : la taille est à 0, le OnGui l'initialise)
        /// (Frame 2 : c'est la bonne taille)<br></br><br></br>
        /// 
        /// Problème 2) :<br></br>
        /// Si il y a plusieurs fois l'attribut dans l'inspecteur, cela fait de la merde, car les variables sont partagés
        /// (Field 1 : la taille est à 0 et s'init correctement)
        /// (Field 2 : unity utilise la taille du field 1, car il a le meme attribut, et ensuite le modifie)<br></br><br></br>
        /// 
        /// Solutions : créer un dictionnaire qui stocke le contexte pour faire la correspondance SerializedPorperty/Tailles et autres datas
        /// </summary>
        Dictionary<string, AttributeContext> ContextDict = new Dictionary<string, AttributeContext>();
        
        protected AttributeContext GetContext(SerializedProperty property)
        {
            string path = property.propertyPath;
            if (!ContextDict.ContainsKey(path))
            {
                ContextDict.Add(path, new AttributeContext());
            }
            return ContextDict[path];
        }



    

        public const float SIZE_LINE = 18f;


        /// <summary>
        /// Trouver la taille d'un champ, sans ses property drawers
        /// </summary>
        public float GetFieldHeight(SerializedProperty property, GUIContent label)
        {
            return PropertyDrawerFinder.GetDefaultPropertyHeight(property, label);

            // return EditorGUI.GetPropertyHeight(property, true);

        }



            public float NextDrawerHeight(UnityEditor.SerializedProperty property, GUIContent label, bool propertyAlreadyDrawn)
        {
            return (tempNextDrawer?.GetPropertyHeightRecursive(property, label)) ?? (propertyAlreadyDrawn ? 0 : GetFieldHeight(property, label) );// EditorGUI.GetPropertyHeight(property, label, true));
        }

        public virtual float GetPropertyHeightRecursive(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return GetPropertyLocalHeight(property, label)  + NextDrawerHeight(property, label, propertyAlreadyDrawn);
        }
        public virtual float GetPropertyLocalHeight(UnityEditor.SerializedProperty property, GUIContent label)
        {
            return GetContext(property).AttributeHeight;
        }


        public enum IncrementRectMode { 
            /// <summary>
            /// Incrémenter la position Y du Rect, pour que le prochain drawer se dessine en dessous
            /// </summary>
            BeforeDrawing=0,
            /// <summary>
            /// Dessiner tout les Drawer récursif avant d'incrémenter la position Y du Rect et de le renvoyer.
            /// Permet de dessiner le Drawer actuel en dessous de tout les autres.
            /// </summary>
            Afterdrawing=1,
        }

        /// <summary>
        /// Dessine les drawer d'aprés, ou drawer par défaut
        /// </summary>
        /// <param name="increment">Incrémenter la postion Y du prochain champ ?</param>
        /// <returns></returns>
        public Rect CallNextAttribute(Rect nextPosition,  UnityEditor.SerializedProperty property, GUIContent label, IncrementRectMode increment = IncrementRectMode.BeforeDrawing)
        {
            //On enregistre les modifications de la taille de l'attribut, qui ont eu lieues dans le OnGui. 
            //On appelle le locaHieght, au cas où les taille sont statiques et redéfinies dans cette fonc
            var  Context = GetContext(property);
            Context.AttributeHeight = GetPropertyLocalHeight(property, label);

            var newPos = nextPosition;


            //on fait avancer la pos
            if (increment == IncrementRectMode.BeforeDrawing)
                newPos.y += Context.AttributeHeight;

            if (tempNextDrawer != null)
            {
                
                //on set la nouvelle salle du bordel
                newPos.height = tempNextDrawer.GetPropertyLocalHeight(property, label);
                tempNextDrawer.propertyAlreadyDrawn = propertyAlreadyDrawn;
                tempNextDrawer.OnGUI(newPos, property, label);
            }
            else if (!propertyAlreadyDrawn)
            {
    
                newPos.height = NextDrawerHeight(property, label, propertyAlreadyDrawn);
                property.DrawPropertyField(newPos, label);
            }


           if (increment == IncrementRectMode.Afterdrawing)
                 newPos.y += NextDrawerHeight(property, label, propertyAlreadyDrawn) + 2;


            nextPosition.y = newPos.y;
           return nextPosition;
        }



    }

    public static class PropertyExtensioin
    {

        public enum IsInArrayMode
        {
            /// <summary>
            /// Retournera vrai si la SerializedProperty est une valeur d'un array.
            /// <code>           
            /// //Exemple :
            /// int[] test = new int[10];
            /// //la serializedProperty test[3] => retournera true.
            /// </code>
            /// </summary>
            /// 
            PropertyIsAnArrayElement = 0,
            /// <summary>
            /// Retournera vrai si le chemin de la SerializedProperty contient un array.
            /// <code>           
            /// //Exemple :
            /// Classe[] test = new Classe[10];
            /// //la serializedProperty test[3].child.prout.x => retournera true.
            /// </code>
            /// </summary>
            PropertyPathContainArray = 1
        }

        /// <summary>
        /// Retourne true si la SerializedProp est stockée dans un array
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsInArray(this SerializedProperty property, IsInArrayMode fieldAppliedOnArray = IsInArrayMode.PropertyPathContainArray)
        {
         //   Debug.Log(property.propertyPath);
            string[] variableName = property.propertyPath.Split('.');

            if (fieldAppliedOnArray == IsInArrayMode.PropertyIsAnArrayElement)
            {
                return variableName.Last().Contains("data[");//Si a la fin du path on a le numéro d'index, c'est que c'est un array
            }

            SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);//Ca je sais pas comment ca fonctionne
            return p.isArray;

        }

        
    }







        [CustomPropertyDrawer(typeof(FutureFieldAttribute), true)]
    public class FutureFieldDrawer : PropertyDrawer
    {

        FutureFieldAttribute currentAttribute;

        public static class EditorWindowRetrieve
        {
            private static PropertyInfo current;
            private static FieldInfo m_ActualView;

            public static EditorWindow GetCurrentEditorWindow()
            {

                LazyInitializeCurrentEditorWindowMetadata();
            
                if (current == null)
                    return null;

                object guiView = current.GetValue(null, null);

                if (guiView != null)
                    return m_ActualView.GetValue(guiView) as EditorWindow;
                return null;
            }

            private static void LazyInitializeCurrentEditorWindowMetadata()
            {
                Type GUIViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");
                if (GUIViewType != null)
                {
                    current = GUIViewType.GetProperty("current", BindingFlags.Public | BindingFlags.Static);

                    Type HostViewType = typeof(Editor).Assembly.GetType("UnityEditor.HostView");
                    if (HostViewType != null)
                        m_ActualView = HostViewType.GetField("m_ActualView", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (m_ActualView == null)
                        current = null;
                }
            }
        }






        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return currentAttribute?.GetPropertyHeightRecursive(property, label) ?? EditorGUI.GetPropertyHeight(property, label, true);  
        }


        /// <summary>
        /// Unity s'amuse à appeler plusieur fois OnGui en utilisant les Default drawer de l'inspector. On ne veut pas ca.
        /// </summary>
        //static bool recursionPrevent;


       public static Dictionary<string, bool> recCounter = new Dictionary<string, bool>();



        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
          if(!recCounter.ContainsKey(property.propertyPath))
                recCounter.Add(property.propertyPath, false);


            //   Debug.Log(EditorWindowRetrieve.GetCurrentEditorWindow().name);


            var endDrawer = PropertyDrawerFinder.FindDrawerForProperty(property);



            //Promis c'est utile, sinon les Drawers pètes des cables
            //(Genre modifient le texte affiché pour le nom des champs et
            // affichent des valeurs sans aucune raison)
            label = new GUIContent(label.text, label.image, label.tooltip);



            FutureFieldAttribute _Attribute = attribute as FutureFieldAttribute;


            // First get the attribute since it contains the range for the slider

            if (_Attribute.stored == null || _Attribute.stored.Count == 0)
            {
                _Attribute.stored = fieldInfo.GetCustomAttributes(typeof(FutureFieldAttribute), false).OrderBy(s => ((PropertyAttribute)s).order).ToList();
            }

            var OrigColor = GUI.color;

            var Label = label;

            // if(property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null)
            // Undo.RecordObject(property.objectReferenceValue, "Object changes");


            FutureFieldAttribute currentAttrib;

            var first = _Attribute.stored[0] as FutureFieldAttribute;









            var lastAttrib = first;
            first.propertyIndex = 0;


            for (int i = 1; i < _Attribute.stored.Count; i++)
            {             
                currentAttrib = _Attribute.stored[i] as FutureFieldAttribute;
                currentAttrib.propertyIndex = i;

                //  currentAttrib.ResetDynamicValues();

                lastAttrib.tempNextDrawer = currentAttrib;
                lastAttrib = currentAttrib;
            }



            //Quand unity appelle plusieur fois le OnGUI parce que dieu le veut :
            ///On execute juste le dernier drawer, en priant pour qu'il affiche le champ, tranquillement
            ///Pire : il faut trouver un moyen d'autoriser la récursion quand c'est volontaire. Je cherche un moyen d'ajouter un context qui explique
            ///qui apelle quoi, et comment on autorise de la récursion mais pas auto par unity.


            var recursionPrevent = recCounter[property.propertyPath];
                

            if ( false && recursionPrevent)
            {

                // GUIUtility.ExitGUI();

                /*
                StringBuilder list = new StringBuilder();
                list.AppendLine($"Recursion on comp : {property.displayName}");
                foreach (var item in _Attribute.stored)
                {
                    list.AppendLine($"- {   (item as FutureFieldAttribute).GetType().Name}");
                }
                Debug.Log(list.ToString());
                */

                // Debug.Log("Erreur : " + EditorWindowRetrieve.GetCurrentEditorWindow().GetInstanceID());

                recCounter[property.propertyPath] = false;


                position.height = first.GetPropertyLocalHeight(property, label);
                lastAttrib.CallNextAttribute(position, property, label);
                return;
            }

            //Pour eviter la recursion de cette fonction
            recCounter[property.propertyPath] = true;


              currentAttribute = first;









            position.height = first.GetPropertyLocalHeight(property, label);
            //On en sait rien en fait ! puisque que actuellement ca va dépendre de la variable du first.
            first.ResetDynamicValues();
            first.OnGUI(position, property, label) ;

            GUI.color = OrigColor;

            recCounter[property.propertyPath] = false;

        }









#endif
    }


}
