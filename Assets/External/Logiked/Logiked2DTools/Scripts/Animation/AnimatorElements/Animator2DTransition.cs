using logiked.source.extentions;
using logiked.source.graphNode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.Tool2D.animation
{

    [System.Serializable]
    public class Animator2DTransition : NodeTransition
    {
   


        public Animator2DTransition(int nextNodeId) : base(nextNodeId)
        {

        }




        public enum AnimationRuleEnd { Immediately, AnimationEnded }

        [SerializeField] private List<Animator2DRule> rules = new List<Animator2DRule>();
        [SerializeField] private AnimationRuleEnd endRuleInteraction = AnimationRuleEnd.AnimationEnded;



        public AnimationRuleEnd EndRuleInteraction
        {
            get { return endRuleInteraction; }
#if UNITY_EDITOR
            set { endRuleInteraction = value; }
#endif
        }

#if UNITY_EDITOR
        [NonSerialized]
        public bool IsPointerOver;

#endif

        public List<Animator2DRule> Rules
        {            
            get { return rules; }
        }





    }
}
