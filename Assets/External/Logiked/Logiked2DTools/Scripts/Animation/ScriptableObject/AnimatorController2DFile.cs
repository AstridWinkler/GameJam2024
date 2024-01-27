using logiked.source.extentions;
using logiked.source.graphNode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




namespace logiked.Tool2D.animation
{

    /// <summary>
    /// Animator controller File for 2D animations
    /// </summary>
    /// <seealso cref="logiked.LogikedAssemblySettings" />
    [System.Serializable]
    [CreateAssetMenu(fileName = "animator", menuName = "Logiked/2DTools/2D Animator Controller", order = 2)]
    public sealed class AnimatorController2DFile : ScriptableObject, INodeStorage<Animation2DNode, Animator2DTransition>
    {

        #region FIELDS - SERIALIZED

        /// <summary>
        /// Accessibles depuis l'inspecteur
        /// </summary>
        [SerializeField]
        private List<Animation2DNode> animations = new List<Animation2DNode>();
        internal List<Animation2DNode> Animations
        {
            get { return animations; }
            // set { animations = value; }
        }



        [SerializeField]
        private List<Animator2DVariable> variables = new List<Animator2DVariable>();
        internal List<Animator2DVariable> Variables { get { return variables; } set => variables = value; }


        /// <summary>
        /// Nombre d'animarions dans le fichier
        /// </summary>
        public int AnimationCount { get => animations.Count; }



        /// <summary>
        /// Premiere animation jouée dans l'animator
        /// </summary>
        [SerializeField]
        private int animationEntryPointId = 0;
        internal int AnimationEntryPoint
        {
            get { return animationEntryPointId; }
#if UNITY_EDITOR
            set { animationEntryPointId = value; }
#endif
        }


        #endregion

        #region FIELDS IN GAME - NON SERIALIZED

        [NonSerialized]
        private List<Animation2DNode> anyStateNodes;

        #endregion



        #region METHODS - Interfaces
        public List<Animation2DNode> GetNodes()
        {
            return animations;
        }
        public string GetNodeArrayPropertyPath => nameof(animation);

        public void SetNodes(List<Animation2DNode> nodes)
        {
            animations = new List<Animation2DNode>(nodes);
            RefreshIdDictionary();
        }

        public System.Type NewNodeTypes() => typeof(Animation2DNode);

        #endregion

        #region METHODS - Utils & Accessors

        #region Default Implementation

        [NonSerialized]
        private Dictionary<int, Animation2DNode> animationDic;
        public Animation2DNode GetNodeById(int id)
        {
            if (animationDic == null)
                RefreshIdDictionary();

            var res = animationDic.GetOrDefault(id);

#if UNITY_EDITOR
            //When nodes are edited in Unity, they can change frequenlty.
            if (res == null || (res != null && !animations.Contains(res)))//If not exist or deleted Node
                RefreshIdDictionary();
#endif

            return animationDic.GetOrDefault(id);
        }

        /// <summary>
        /// Refresh le dictionnaire contenant toutes les animtions (utilisé via <see cref="GetNodeById"/>) pour le PlayMode.
        /// </summary>
        internal void RefreshIdDictionary()
        {
            // Debug.Log("Update animation file. Original Dictionnary content : "+animationIds.Count);
            //editorAimationIds.Clear();  
            var dic = new Dictionary<int, Animation2DNode>();
            foreach (var anim in animations)
            {
                dic.Add(anim.UniqueNodeId, anim);
            }

            animationDic = dic;
        }
        #endregion




        /// <summary>
        /// Retourne tous les nodes marqués comme "AnyState" pour les transitions.
        /// </summary>
        /// <returns>Liste des nodes "AnyState"</returns>
        internal List<Animation2DNode> GetAnyStateNodes()
        {
            if (anyStateNodes == null)
                RefreshAnyNodeList();

#if UNITY_EDITOR
            RefreshAnyNodeList();
#endif
            return anyStateNodes;
        }


        private void RefreshAnyNodeList()
        {
            anyStateNodes = animations.Where(m => m?.TransitionsMode == Animation2DNode.AnimationTransitionMode.FromAnyState).ToList();
        }

    



        #endregion






#if TEST_WORKSPACE_MODE
        //EDITOR ONLY
#if UNITY_EDITOR
        [Header("Editor parameters")]

        /// <summary>
        //Est-ce que l'animator utilise seulement les animation du dossier dans lequel il est ?
        /// </summary>
        
        [SerializeField]
        private bool folderOnlyEditoMode = true;
        internal bool FolderOnlyEditorMode
        {
            get { return folderOnlyEditoMode; }
            set { folderOnlyEditoMode = value; }
        }

#endif

#endif


    }
}
