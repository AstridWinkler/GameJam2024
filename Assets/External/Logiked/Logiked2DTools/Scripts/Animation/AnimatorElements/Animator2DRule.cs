using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;


namespace logiked.Tool2D.animation
{

    /// <summary>
    /// Rules for Animator2DTransitions between animators variables 
    /// </summary>
    [System.Serializable]
    public class Animator2DRule
    {


        public enum AnimationRuleComparaison { Equal = '=', NotEqual = '≠' }

        [SerializeField] private AnimationRuleComparaison comparator = AnimationRuleComparaison.Equal;
        [SerializeField] private int result;
        [SerializeField] private int variableId = 0;

        /*
        [SerializeField] private int variableID = 0;

        public int VariableID
        {
            get { return variableID; }
#if UNITY_EDITOR
            set { variableID = value; }
#endif
        }*/




#if UNITY_EDITOR
        public int Result { get => result; internal set { result = value; } }
        public int VariableId { get => variableId; internal set { variableId = value; }}
        public AnimationRuleComparaison Comparator { get => comparator; internal set { comparator = value; } }
#else
        public int Result { get => result;  }
        public int VariableId { get => variableId; }
        public AnimationRuleComparaison Comparator { get => comparator; }

#endif




        public bool Evaluate(int val)
        {
            switch (comparator)
            {
                case AnimationRuleComparaison.Equal: return val == result;
                default: return val != result;
            }
        }

        public string ToString(string variableName)
        {
            StringBuilder str = new StringBuilder();
            str.Append(variableName);
            str.Append(" ");
            str.Append((char)Comparator);
            str.Append(" ");
            str.Append(Result);
            str.Append("\n");
            return str.ToString();
        }
    }


}
