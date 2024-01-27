
using logiked.source.utilities;
using System;
using UnityEditor;
using UnityEngine;
using logiked.source.attributes.root;
using logiked.source.extentions;
using static logiked.source.extentions.ReflectExtention;
using System.Linq;

#if UNITY_EDITOR
using logiked.source.editor;
#endif

namespace logiked.source.attributes
{
    /// <summary>
    /// Les différents type de comparaisons pour l'attribut ShowIfAttribute
    /// </summary>
    public enum ShowIfOperations { Equal, NotEqual, Lesser, Greater }

    /// <summary>
    /// Parametre de répetitions pour l'attribut ShowIfAttribute. Mettez ShowIfRepeatMode.Same dans le constructeur de l'attribut pour qu'il execute la même comparaison que le précédent attribut.
    /// </summary>
    public enum ShowIfRepeatMode { Same = 1, NotSame = 2 }

    /// <summary>
    /// Attribut qui affiche(ou non) les champs sur lequel il est disposé.<br></br>
    /// Todo : faire un Contexte parent pour que les attributs qui utilisent du SAME ou NOTSAME fonctionne avec des classes imbriqués
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : FutureFieldAttribute
    {

        public enum ComputeMode { OR = 1, AND = 2 }

        /// <summary>
        /// Permet de repeter un showIf sans avoir besoin de resaisir plusieurs fois l'opération
        /// </summary>
        private ShowIfRepeatMode repeatmode = 0;
        private ShowIfOperations operation;

        private static bool last_result;
        private static ShowCalculation last_opt;

        /// <summary>
        /// Liste des Calculs à effectuer
        /// </summary>
        private ShowCalculation[] calcList;
        ComputeMode currentMode = ComputeMode.OR;


        ReflectedObject getValue;



        /// <param name="fieldName">Le nom du champ/propriété qui sert à la comparaison, depuis l'objet conteneur du champ.</param>
        /// <param name="operation">Type d'opération de comparaison à appliquer</param>
        /// <param name="result">Résultat attendu</param>
        public ShowIfAttribute(string fieldName, ShowIfOperations operation, object result) : this()
        {
            currentMode = ComputeMode.OR;
            calcList = new[] { new ShowCalculation(fieldName, operation, result) };
            repeatmode = 0;
        }


        /// <param name="fieldName">Le nom du champ/propriété qui sert à la comparaison, depuis l'objet conteneur du champ.</param>
        /// <param name="operation">Type d'opération de comparaison à appliquer. Peut etre "=", "==", "!=", "<", ">"</param>
        /// <param name="result">Résultat attendu</param>
        public ShowIfAttribute(string fieldName, string operation, object result) : this()
        {
            currentMode = ComputeMode.OR;
            ShowIfOperations opp = ShowIfOperations.Equal;
            switch (operation)
            {

                case "=":
                case "==":
                    opp = ShowIfOperations.Equal;
                    break;
                case "!=":
                    opp = ShowIfOperations.NotEqual;
                    break;
                case "<":
                    opp = ShowIfOperations.Lesser;
                    break;
                case ">":
                    opp = ShowIfOperations.Greater;
                    break;
                default:
                    throw new System.ArgumentException($"\"{operation}\" is an invalid operator ! Valid operator are : \n \"=\", \"==\", \"!=\", \"<\",\">\".", nameof(operation));
            }
            calcList = new[] { new ShowCalculation(fieldName, opp, result) };
            repeatmode = 0;
        }




        //Ne fonctionne pas du tout ! les attributs ont besoin de paramètres constants
        /*
         public ShowIfAttribute(ComputeMode mode, params object[] CompareList)
                {
                    currentMode = mode;
                    calcList = Array.ConvertAll(CompareList, m => (ShowCalculation)m); 
                    repeatmode = 0;
                }
        */



        /// <summary>
        /// Relance une comparaison pour l'affichage en copiant le dernier attribut ShowIfAttribute utilisé
        /// </summary>
        /// <param name="mode">Le mode de répétition : Même affichage / affichage inverse(else)</param>
        public ShowIfAttribute(ShowIfRepeatMode mode) : this()
        {
            repeatmode = mode;
        }

        public ShowIfAttribute()
        {
            order = -100;
        }



        /// <summary>
        /// Relance une comparaison pour l'affichage en copiant le dernier rérulstat + un OR ou AND un autre résultat.
        /// </summary>
        /// <param name="mode">Le mode de répétition : Même affichage / affichage inverse(else)</param>
        public ShowIfAttribute(ComputeMode mode, string secondaryFieldName, ShowIfOperations secondaryOperation, object secondaryResult) : this()
        {
            currentMode = mode;
            calcList = new[] { last_opt, new ShowCalculation(secondaryFieldName, secondaryOperation, secondaryResult) };
            repeatmode = 0;

        }





#if UNITY_EDITOR


        bool IsRenderCanceled(SerializedProperty prop)
        {
            return (bool)GetContext(prop).datas.GetOrDefault(nameof(IsRenderCanceled), false);
        }




        protected override void OnGUIRecursive(Rect position, SerializedProperty property, GUIContent label, AttributeContext Context)
        {
            bool BrokenRender = Context.GetData<bool>(nameof(IsRenderCanceled));



            switch ((int)repeatmode)
            {

                case 0:



                    object o = property.GetParent();

                    bool endResult = (currentMode == ComputeMode.AND);

                    foreach (var e in calcList)
                    {

                        if (currentMode == ComputeMode.OR)
                            endResult |= e.Evaluate(o);

                        if (currentMode == ComputeMode.AND)
                            endResult &= e.Evaluate(o);

                    }


                    //  Type type = GetValue(o, fieldName);
                    // if (type == null) Debug.LogErrorFormat("Attribute [ShowIf] incorrect field name {0} on {1}.{2}", fieldName, property.serializedObject.targetObject.name, property.name);
                    //  Debug.LogError("snip");


                    BrokenRender = !endResult;
                    last_opt = calcList[0];

                    last_result = BrokenRender;
                    break;


                case (int)ShowIfRepeatMode.Same:
                    BrokenRender = last_result;
                    break;

                case (int)ShowIfRepeatMode.NotSame:
                    BrokenRender = !last_result;
                    break;
            }


            Context.SetData(nameof(IsRenderCanceled), BrokenRender);

            if (BrokenRender)
            {
                return;
            }

            CallNextAttribute(position, property, label);
        }



        public override float GetPropertyHeightRecursive(SerializedProperty property, GUIContent label)
        {
            //Eviter d'avoir des array tout moches si rien n'est affiché dedans
            return IsRenderCanceled(property) ? (property.IsInArray(PropertyExtensioin.IsInArrayMode.PropertyIsAnArrayElement) ? SIZE_LINE : 0) : base.GetPropertyHeightRecursive(property, label);
        }







#endif



        /// <summary>
        /// Représentation d'un calcul pour l'attribut ShowIf
        /// </summary>
        [Serializable]
        public class ShowCalculation
        {

            private string fieldName;
            private ShowIfOperations operation;
            private object exceptedResult;
            private object currentValue;

            //Autre calcul à faire un AND avec



            public ShowCalculation(string fieldName, ShowIfOperations operation, object exceptedResult)
            {
                this.fieldName = fieldName;
                this.operation = operation;
                this.exceptedResult = exceptedResult;
            }




            public bool Evaluate(object obj)
            {
                var reflect = obj.GetReflectedValueAtPath(fieldName);

                if (reflect != null)
                {
                    currentValue = reflect.Value;
                }


                var res = Compute();

                /*
                if (AND_Calc != null)
                    res = res && AND_Calc.Evaluate(property);
                */

                return res;
            }

            /*
            //UN peu crétin mais bon
            public ShowCalculation And(string fieldName, ShowIfOperations operation, object exceptedResult)
            {
                AND_Calc = new ShowCalculation(fieldName, operation, exceptedResult);
                return AND_Calc;
            }
            */

            bool Compute()
            {
                if (exceptedResult == null)
                {
                    switch (operation)
                    {
                        case ShowIfOperations.Equal: return currentValue == null;
                        case ShowIfOperations.NotEqual: return currentValue != null;
                        default: return false;
                    }
                }

                if (exceptedResult is int)
                {
                    switch (operation)
                    {
                        case ShowIfOperations.Equal: return (int)currentValue == (int)exceptedResult;
                        case ShowIfOperations.NotEqual: return (int)currentValue != (int)exceptedResult;
                        case ShowIfOperations.Greater: return (int)currentValue > (int)exceptedResult;
                        case ShowIfOperations.Lesser: return (int)currentValue < (int)exceptedResult;
                    }
                }

                else

                if (exceptedResult is float)
                {
                    switch (operation)
                    {
                        case ShowIfOperations.Equal: return (float)currentValue == (float)exceptedResult;
                        case ShowIfOperations.NotEqual: return (float)currentValue != (float)exceptedResult;
                        case ShowIfOperations.Greater: return (float)currentValue > (float)exceptedResult;
                        case ShowIfOperations.Lesser: return (float)currentValue < (float)exceptedResult;
                    }
                }
                else
                {
                    switch (operation)
                    {
                        case ShowIfOperations.Equal: return exceptedResult.Equals(currentValue);
                        case ShowIfOperations.NotEqual: return !exceptedResult.Equals(currentValue);
                        case ShowIfOperations.Greater: return System.Convert.ToInt32(currentValue) > System.Convert.ToInt32(exceptedResult);
                        case ShowIfOperations.Lesser: return System.Convert.ToInt32(currentValue) < System.Convert.ToInt32(exceptedResult);
                    }
                }
                return false;

            }


        }


    }

    /// <summary>
    /// Attribut qui répète le comportement du dernier ShowIF utilisé.<br></br>
    /// Racourci du constructeur <see cref="ShowIfAttribute"></see> avec le paramètre <see cref="ShowIfRepeatMode.Same"/>. <br></br>
    /// Todo : faire un Contexte parent pour que les attributs qui utilisent du SAME ou NOTSAME fonctionne avec des classes imbriqués.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfSameAttribute : ShowIfAttribute
    {
        public ShowIfSameAttribute() : base(ShowIfRepeatMode.Same) { }

    }

    


}
