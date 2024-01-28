using logiked.source.extentions;
using logiked.Tool2D.settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace logiked.Tool2D.animation
{


    [AddComponentMenu("Logiked/2D Animator Renderer")]
    [ExecuteAlways]
    public class AnimatorRenderer2D : MonoBehaviour, I_Animable2D
    {


        [SerializeField] private AnimatorController2DFile animatorFile;
        [SerializeField] private new SpriteRenderer renderer;
        [SerializeField] private bool playOnEnable = true;
        [NonSerialized] private bool isPlaying = false;


        /// <summary>
        /// Node parcouru par l'animator
        /// </summary>
        [NonSerialized]
        private Animation2DNode currentState;

        /// <summary>
        /// Animation instanciée par ce node
        /// </summary>
        private Animation2DReader currentAnim;



        public AnimatorController2DFile AnimatorFile => animatorFile;

        /// <summary>
        /// Pourcentage d'avancement de l'animation actuellement jouée
        /// </summary>
        public float percentStateFinished => currentAnim == null ? 0 : currentAnim.PercentFinished;


#if UNITY_EDITOR
        /// <summary>
        /// Id du Node de l'animation actuellement jouée
        /// </summary>
        internal int CurrentNodeId => currentState == null ? 0 : currentState.UniqueNodeId;
#endif


        public bool IsPlaying { get => isPlaying; }





        /// <summary>
        /// Id de la variable => Instance de la variable
        /// </summary>
        private Dictionary<int, Animator2DVariable> variableFromIds = new Dictionary<int, Animator2DVariable>();
        /// <summary>
        /// Nom de variable => Première instance de la variable qui porte ce nom
        /// </summary>
        private Dictionary<string, Animator2DVariable> variablesNames = new Dictionary<string, Animator2DVariable>();


        /// <summary>
        /// Variables utilisés par SetStateValue avec le parametre Reset à true.
        /// </summary>
        [SerializeField] private HashSet<int> variablesToReset = new HashSet<int>();


        /// <summary>
        /// Apply the speed to the current animatonReader
        /// </summary>
        /*public float Speed
        {
            get => currentAnim == null ? 0 : currentAnim.SpeedModifier; set { speed = value; if (currentAnim != null) currentAnim.SpeedModifier = speed; }
        }
        [SerializeField]
        private float speed = 1f;*/


        
 
        /// <summary>
        /// Assigne la valeur val à la variable
        /// <param name="useAsTrigger">Utiliser comme un trigger ? (Retour de la variable à 0 aprés avoir été utilisé)
        /// Utile pour activer rapidement & temporairement une branche de nodes d'un animator.</param>
        /// </summary>
        public void SetStateValue(string variableName, int val, bool useAsTrigger = true)
        {
            if (!variablesNames.ContainsKey(variableName))
                Debug.LogError("Variable " + variableName + " not found.");
            else
            {
                var variable = variablesNames[variableName];
                if (useAsTrigger)
                    variablesToReset.Add(variable.UniqueId);
                variable.Value = val;
            }
        }


        /// <summary>
        /// Retourne la liste des varibales actuellement utilisées par l'animator
        /// </summary>
        /// <returns>La liste des variables</returns>
        public Animator2DVariable[] GetVariablesList()
        {
            return variablesNames.Values.Select(m => new Animator2DVariable(m)).ToArray();
        }








        private void Awake()
        {
            isPlaying = false;
            RefreshVariables();
        }


        void SetNode(Animation2DNode node, bool allowSameNode = false)
        {
            if (allowSameNode || currentState != node)
            {
                currentState = node;
                currentAnim = new Animation2DReader(currentState.File, currentState.SpeedModifier);
                currentAnim.Play();
            }
        }


        void RefreshVariables()
        {
            variablesNames.Clear();
            variableFromIds.Clear();
       

            if (animatorFile == null) return;

            var vars = animatorFile.Variables;
            Animator2DVariable inst;

            for (int i = vars.Count-1; i >= 0; i--)
            {
                inst = new Animator2DVariable(vars[i]);
                variablesNames.AddOrUpdate(inst.Name, inst);
                variableFromIds.Add(inst.UniqueId, inst);
            }

        }


        public void Play()
        {
            if (!Application.isPlaying && !LogikedPlugin_2DTools.Instance.PlayAnimatorRenderersInSceneView) return;

            if(animatorFile == null)
            {
                Debug.LogWarning("Animator file is null !", this);
                return;
            }

            if (isPlaying || animatorFile.AnimationCount==0) return;

            RefreshVariables();

            SetNode(animatorFile.GetNodeById(animatorFile.AnimationEntryPoint));
            isPlaying = true;
        }


        public void Stop()
        {
            isPlaying = false;
            currentAnim = null;
            currentState = null;
        }
        public void Pause()
        {
            Debug.LogError("Not implemented");
        }

        public void Restart()
        {
            RefreshVariables();
        }


        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.EditorApplication.update += EditorUpdate;
#endif

            if (playOnEnable)
                Play();
        }



        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.EditorApplication.update -= EditorUpdate;
#endif
            // Pause();
            Stop();

        }


#if UNITY_EDITOR
        void EditorUpdate()
        {
            if (renderer == null) renderer = GetComponentInChildren<SpriteRenderer>();
            if (animatorFile == null || renderer == null) return;

            Update();

        }
#endif



        void Update()
        {

            if (!Application.isPlaying && !LogikedPlugin_2DTools.Instance.PlayAnimatorRenderersInSceneView)
            {
#if UNITY_EDITOR
                if (renderer == null) renderer = GetComponentInChildren<SpriteRenderer>();
                if (renderer != null && animatorFile != null && animatorFile.AnimationCount > 0)
                {
                    renderer.sprite = animatorFile.GetNodeById(animatorFile.AnimationEntryPoint)?.File?.Sprites[0];
                }
                return;
#endif

                }


                bool hasChanged = false;

            if (animatorFile == null) return;

            //Test transitions from the current node
            if (currentState != null)            
                hasChanged = CheckAndApplyNodeConditions(currentState);
            


            //If nothing foud, Test transitions for all "Any State" nodes
            if (!hasChanged )
            {
                foreach (var item in animatorFile.GetAnyStateNodes())
                {
                    if (CheckAndApplyNodeConditions(item)) break;
                }
            }



            if (currentAnim == null ) return;

            if (currentState != null)
                currentAnim.Variation = variableFromIds.GetOrDefault(currentState.VariationVariableId)?.Value ?? 0;
            renderer.sprite = currentAnim.GetCurrentSprite();
        }


        /// <summary>
        ///Check les conditions du node actuellement joué pour progresser dans le graph de l'animation. 
        ///alreadySkip stocke les elements déja parcouru, pour eviter une boucle infinie de parcour recursif entre les nodes.
        /// </summary>
        /// <param name="alreadyChecked">The already checked nodes.</param>
        /// <returns>True si il y a eu une modification d'état</returns>
        bool CheckAndApplyNodeConditions(Animation2DNode currentNode, HashSet<Animation2DNode> alreadyChecked = null)
        {
            int i, j;
            Animation2DNode next;
            Animator2DTransition trans;
            Animator2DRule rule;
            bool ruleEval = true;

            var transitionsList = currentNode.AllTransitions;


            for (i = 0; i < transitionsList.Length; i++)
            {


                ///init
                trans = transitionsList[i];
                ruleEval = true;


                ///Endrule
                switch (trans.EndRuleInteraction)
                {
                    case Animator2DTransition.AnimationRuleEnd.AnimationEnded:
                        if (currentState?.File != null)
                        {
                            if (currentAnim.FrameDeltaCumulated.Abs() < currentAnim.file.Duration) continue;
                        }
                        else
                            continue;
                        break;
                }


                //Check all rules
                for (j = 0; j < trans.Rules.Count; j++)
                {
                    rule = trans.Rules[j];

    
                    if (!rule.Evaluate( variableFromIds.GetOrDefault(rule.VariableId)?.Value ?? variableFromIds.GetOrDefault(0).Value ) )
                    {
                        ruleEval = false;// canel
                        break;
                    }
                }

                if (!ruleEval) continue;

                next = animatorFile.GetNodeById(trans.NextNodeId);

                if (alreadyChecked == null) alreadyChecked = new HashSet<Animation2DNode>();

                if (alreadyChecked.Contains(next)) continue;//Si l'animator veut retourner sur un node déja parcouru


                ///Prochain node trouvé
              
                alreadyChecked.Add(currentNode);
                SetNode(next);//Apply now node
                CheckAndApplyNodeConditions(next, alreadyChecked);//Check if is other transitions than can be satisfied


                ///Si des Variables on été utilisés pour le calcul, alors on reset les variables temporaires qui ont effectué ce déplacement.

                if (trans.Rules.Count > 0)
                {
                    foreach (var v in variablesToReset)
                        variableFromIds[v].Value = 0;
                }

                return true;
            }
            return false;
        }
    }
}
