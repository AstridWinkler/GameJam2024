using logiked.Tool2D.animation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.Tool2D.animation
{

    [AddComponentMenu("Logiked/2D Animation Renderer")]
    [ExecuteAlways]
    public class SimpleAnimationRenderer2D : MonoBehaviour, I_Animable2D
    {
        [SerializeField]
        Animation2DFile animationFile;
        [SerializeField]
        public Animation2DFile AnimationFile { get => animationFile; set => SetAnim(animationFile); }

        [NonSerialized]
        private Animation2DReader currentAnim;

        [SerializeField]
        private SpriteRenderer render;
        

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
            if (render == null) render = GetComponentInChildren<SpriteRenderer>();
            if (currentAnim == null || render == null) return;
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
            render.sprite = currentAnim.GetCurrentSprite();
        }


    }
}