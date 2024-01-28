using logiked.source.attributes;
using logiked.Tool2D.animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace logiked.Tool2D.animation
{

    [AddComponentMenu("Logiked/2D Animation Renderer")]
    [ExecuteAlways]
    public class SimpleAnimationRenderer2D : MonoBehaviour, I_Animable2D
    {
        public enum AnimableRenderTypeEnum { SpriteRenderer, UiImage }


        [SerializeField]
        Animation2DFile animationFile;
        [SerializeField]
        public Animation2DFile AnimationFile { get => animationFile; set => SetAnim(animationFile); }

        [NonSerialized]
        private Animation2DReader currentAnim;

        [SerializeField]
        private AnimableRenderTypeEnum renderType = AnimableRenderTypeEnum.SpriteRenderer;
        public AnimableRenderTypeEnum RenderType { get => RenderType; set => RenderType = AnimableRenderTypeEnum.SpriteRenderer; }


        [ShowIf(nameof(renderType), "==", AnimableRenderTypeEnum.SpriteRenderer)]
        [SerializeField]
        private SpriteRenderer rendererSprite;


        [ShowIf( ShowIfRepeatMode.NotSame)]
        [SerializeField]
        private Image rendererImage;



        /// <summary>
        /// Apply the speed to the current animatonReader
        /// </summary>
        public float Speed
        {
            get => currentAnim == null ? 0 : currentAnim.SpeedModifier; set { speed = value; if (currentAnim != null) currentAnim.SpeedModifier = speed; }
        }
        [SerializeField]
        private float speed = 1f;

        [SerializeField]
        private bool playOnEnable = true;

        public bool IsPlaying { get => currentAnim == null ? false : currentAnim.IsPlaying; }





        public void SetAnim(Animation2DFile value)
        {
            if (value == null)
            {
                currentAnim = null;
                return;
            }

            if (animationFile == null) return;

            currentAnim = new Animation2DReader(animationFile, speed);
            if (playOnEnable)
                Play();
        }

        public void Play()
        {
            if (currentAnim != null)
                currentAnim.Play();
        }
        public void Stop()
        {
            if (currentAnim != null)
                currentAnim.Stop();
        }
        public void Pause()
        {
            if (currentAnim != null)
                currentAnim.Pause();
        }
        public void Restart()
        {
            Stop();
            Play();
        }


        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.EditorApplication.update += EditorUpdate;
#endif

            if (animationFile == null) return;
            if (currentAnim == null)
                SetAnim(animationFile);


            if (playOnEnable)
                Play();
        }


        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) UnityEditor.EditorApplication.update -= EditorUpdate;
#endif
            Pause();
        }


#if UNITY_EDITOR
        public void EditorUpdate()
        {
 if (currentAnim == null) return;


            if (renderType == AnimableRenderTypeEnum.SpriteRenderer)
            {
                if (rendererSprite == null)
                    rendererSprite = GetComponentInChildren<SpriteRenderer>();
            }
            else
            {
                if (rendererImage == null)
                    rendererImage = GetComponentInChildren<Image> ();
            }



            if (currentAnim.SpeedModifier != speed)
                currentAnim.SpeedModifier = speed;

            Update();
        }
#endif


        void Update()
        {
#if UNITY_EDITOR
            if (!logiked.Tool2D.settings.LogikedPlugin_2DTools.Instance.PlayAnimationsInSceneView && !Application.isPlaying) return;
#endif


            if (currentAnim == null) return;

            if (renderType == AnimableRenderTypeEnum.SpriteRenderer)
            {
                if (rendererSprite != null)
                    rendererSprite.sprite = currentAnim.GetCurrentSprite();
            }
            else
            {
                if (rendererImage != null)
                    rendererImage.sprite = currentAnim.GetCurrentSprite();
            }

        }


    }
}