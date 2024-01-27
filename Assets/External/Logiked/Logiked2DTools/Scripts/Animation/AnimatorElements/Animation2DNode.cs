using logiked.source.utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using logiked.source.extentions;
using logiked.Tool2D.settings;
using System.Text;
using logiked.source.graphNode;
using System.Linq;

#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif

namespace logiked.Tool2D.animation
{







    /// <summary>
    /// Animation wrapper for 2D Animator
    /// </summary>
    [System.Serializable]
    public class Animation2DNode : GraphicNode<Animator2DTransition>
    {
        public enum AnimationTransitionMode { FromThisState = 0, FromAnyState = 1 }

        [SerializeField] private AnimationTransitionMode transitionMode = AnimationTransitionMode.FromThisState;

        [SerializeField] private Animation2DFile file;
        [SerializeField] private float speedModifier = 1;
        [SerializeField] private int variationVariableId = 0;
        


        //[SerializeField] private List<Animator2DTransition> Transitions = new List<Animator2DTransition>();



        #region Transitions

        [SerializeField] private NodeTransitionPin<Animator2DTransition> transitionPack = new NodeTransitionPin<Animator2DTransition>();

        //public override Animator2DTransition[] AllTransitions { get => transitionPack.Transitions; }
        public override NodeTransitionPin<Animator2DTransition>[] TransitionPins
        {
            get => new NodeTransitionPin<Animator2DTransition>[] { transitionPack } ;
        //    set => transitionPack = value.LastOrDefault();
        }


        public NodeTransitionPin<Animator2DTransition> AnimationTransition { get => transitionPack; }

        #endregion


        public string ShortAnimationName
        {
            get
            {
                string res = "Null anim";

                if (File != null)
                {
                    res = File.name.Substring(File.name.LastIndexOf('_') + 1);
                    if (text.IsNullOrEmpty()) res = File.name;
                    else
                        res = res.Substring(0, 1).ToUpper() + res.Substring(1);
                }
                return res;
            }
        }


        public Animation2DNode() : base(new Vector2(150, 75))
        {
            transitionPack = new NodeTransitionPin<Animator2DTransition>();
        }



        #region Properties

        public int VariationVariableId { get => variationVariableId; set => variationVariableId = value; }

        public AnimationTransitionMode TransitionsMode
        {
            get { return transitionMode; }
#if UNITY_EDITOR
            set { transitionMode = value; }
#endif
        }
            
        public Animation2DFile File
        {
            get { return file; }
#if UNITY_EDITOR
            set { file = value; }
#endif
        }







        public float SpeedModifier
        {
            get { return speedModifier; }
#if UNITY_EDITOR
            set { speedModifier = value; }
#endif
        }
        #endregion

        #region OnlyEditor       

#if UNITY_EDITOR


        [NonSerialized] public bool isRealtimePlaying;
        [NonSerialized] public float percentPlaying;
 



        private Animation2DReader readerPreview;
        public Animation2DReader ReaderPreview
        {
            get { return readerPreview; }
            set { readerPreview = value; }
        }


        public override void DrawNode(Vector3 newScreenPosition, float zoomSize, SerializedProperty serialized)
        {
            UpdateLastScreenCoordinates(newScreenPosition, zoomSize);

            StringBuilder buildText = new StringBuilder("\n");


            if (file is Animation2DFileVariations)
            {
                var dir = file as Animation2DFileVariations;
                buildText.Append($"<{dir.VariationCount} variations>\n");
              
                var rectEffect = LastScreenRect;
                GUI.color = Color.white * 0.5f + new Color(0, 0, 0, 1);
                rectEffect.position -= Vector2.one * zoomSize * 20f;
                GUI.Box(rectEffect, "", NodeStyle);

                GUI.color = Color.white * 0.75f + new Color(0, 0, 0, 1);
                rectEffect.position += Vector2.one * zoomSize * 10f;
                GUI.Box(rectEffect, "", NodeStyle);

                GUI.color = Color.white;

            }

            if (transitionMode == AnimationTransitionMode.FromAnyState)
            {
                buildText.AppendLine("<Any state>");
                if(File != null)
                    buildText.AppendLine(ShortAnimationName);

            }
            else
            {
                buildText.AppendLine(ShortAnimationName);
            }

     


                   text = buildText.ToString();

            Rect drawRect = LastScreenRect;

            base.DrawNode(newScreenPosition, zoomSize, serialized);




            if (isRealtimePlaying)
            {
                Rect loadingBox = drawRect;
                loadingBox.height = Math.Max(15f * zoomSize, 12f);  
                loadingBox.width = drawRect.width * 0.75f;

                loadingBox.center = new Vector2(drawRect.center.x, drawRect.center.y - drawRect.height / 1.75f);

                GUI.color = Color.white;
                GUI.Box(loadingBox, "", GUILogiked.Styles.Box_OpaqueWindowWhite);

                GUI.color = Color.green;
                loadingBox.width = loadingBox.width * percentPlaying;
                GUI.Box(loadingBox, "", GUILogiked.Styles.Box_OpaqueWindowWhite);
                GUI.color = Color.white;


                // buildText.AppendLine("[" + new string('█', (percentPlaying * 10f).Rnd()) + new string('░', ((1f - percentPlaying) * 10f).Rnd()) + "]");
            }

            if (File != null)
            {
                if (ReaderPreview == null || ReaderPreview.file != File || ReaderPreview.SpeedModifier != speedModifier)
                {
                    ReaderPreview = new Animation2DReader(File, SpeedModifier);
                    ReaderPreview.Play();
                }

                if(file is Animation2DFileVariations)
                {
                    ReaderPreview.Variation = (CurrentEditor as AnimatronWindow2D)?.CurrentFile?.Variables.FirstOrDefault(m => m?.UniqueId == VariationVariableId)?.Value ?? 0;
                }


                if (ReaderPreview.IsPlaying != LogikedPlugin_2DTools.Instance.PlayPreviewInEditor)
                    ReaderPreview.SetPlayState(LogikedPlugin_2DTools.Instance.PlayPreviewInEditor);

                if (SpeedModifier != ReaderPreview.SpeedModifier)
                    ReaderPreview.SpeedModifier = SpeedModifier;


                //Get sprite and align
                Sprite sp = ReaderPreview.GetCurrentSprite();

                if (sp != null)
                {
                    //Place the sprite on the bottom left of the node
                    var spriteRect = drawRect;
                    float spriteRatio = (sp.rect.width / sp.rect.height);
                    spriteRect.height = drawRect.height * 1.5f;
                    spriteRect.width = spriteRect.height * spriteRatio;
                    spriteRect.center = new Vector2(drawRect.xMin, drawRect.yMax - drawRect.height / 4f);

                    GUILogiked.Panels.DrawSprite(sp, spriteRect);
                }
            }

            if (transitionPack == null)
            {
                transitionPack = new NodeTransitionPin<Animator2DTransition>();
            }


            Vector2 positionIcon = new Vector2(drawRect.xMax - zoomSize*12, drawRect.yMax - zoomSize*12);


                transitionPack.DrawConnexionPin(zoomSize, positionIcon, GUILogiked.Panels.EditorIconType.NodeConnectBlendTree, 15f);


            


        }





#endif
        #endregion



    }
}
