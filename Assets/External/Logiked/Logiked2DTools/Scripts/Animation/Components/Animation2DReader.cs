using logiked.source.extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace logiked.Tool2D.animation
{

    public class Animation2DReader
    {

        #region Properties
        public readonly Animation2DFile file;
        private Sprite[] spriteList;

        private bool isPlaying = false;
        private bool isPaused = false;

        private long startTime = 0;
        private long savedFrameTime = 0;
        private float frameDeltaCumulated = 0;
        private float percentFinished = 0;

        private int variation = 0;
        public int Variation
        {
            get => variation;
            set
            {
                if(value != variation)
                {
                    variation = value;
                    RefreshSprites();
                }
            }
        }



        //private long pauseOffset = 0;
        // private long pauseTime = 0;

        private static long CurrentSystemTime { get => (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); }
        private long ElapsedSinceStart { get => CurrentSystemTime - startTime; }


        public float FrameDeltaCumulated { get => frameDeltaCumulated; }
        public float SpeedModifier { get; set; }
        public float PercentFinished { get => percentFinished; }


        public bool IsPlaying
        {
            get { return isPlaying; }
            set { isPlaying = value; }
        }
        #endregion

        #region Generation
        public Animation2DReader(Animation2DFile from, float speedModifier = 1f)
        {
            this.file = from;
            this.SpeedModifier = speedModifier;

            if (file == null) return;
            RefreshSprites();

            InitCounterValues();
        }

        void InitCounterValues()
        {
            startTime = CurrentSystemTime;
            savedFrameTime = ElapsedSinceStart;
            frameDeltaCumulated = 0;
        }

        void RefreshSprites()
        {
            spriteList = GenerateSpriteList();
        }

        private Sprite[] GenerateSpriteList()
        {
            List<Sprite> sprites = new List<Sprite>();

            var origSprites = file.GetSprites(variation) ?? new Sprite[0];

            switch (file.LoopMode)
            {
                case Animation2DFile.LoopProcess.PingPong:
                    sprites.AddRange(origSprites);
                    for (int i = origSprites.Length - 2; i > 0; i--)
                        sprites.Add(origSprites[i]);
                    break;

                default:
                    sprites.AddRange(origSprites);

                    break;
            }
            return sprites.ToArray();
        }
        #endregion

        #region Public Methods
        public void Play()
        {
            if (isPaused)
            {
                savedFrameTime = ElapsedSinceStart;
                isPaused = false;
            }


            if (isPlaying) return;
            isPlaying = true;
            InitCounterValues();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Stop()
        {
            isPlaying = false;
            isPaused = false;
        }

        public void SetPlayState(bool on)
        {
            if (on) Play();
            else Stop();
        }


        public void Restart()
        {
            Stop();
            Play();
        }


        public Sprite GetCurrentSprite()
        {
            if (file == null) return null;

#if UNITY_EDITOR
            RefreshSprites();
#endif


            float duration = file.Duration;
            if (Mathf.Abs(duration) < 0.001f) duration = Mathf.Sign(duration) * 0.001f;



            long elapsedSinceStart = (isPlaying ) ? ElapsedSinceStart  :  (long)(1000 * duration.Abs() + 1);
            //Soit on joue et on incrément le temps,
            //Soit on ne joue plus, et on met le curseur à la fin du temps
        

            //  if (isPaused)
            //       pauseOffset += elapsedSinceStart - savedFrameTime; 

            if (!isPaused)
                frameDeltaCumulated += (float)((double)( (elapsedSinceStart - savedFrameTime))/1000.0) * Time.timeScale * SpeedModifier;
  
            
            savedFrameTime = elapsedSinceStart;

            //chiant de re check a chaque fois
            if (frameDeltaCumulated.Abs() > duration.Abs() && (file.LoopMode == Animation2DFile.LoopProcess.Once || file.LoopMode == Animation2DFile.LoopProcess.OnceDisapear))
            {
                Stop();

                if (file.LoopMode == Animation2DFile.LoopProcess.Once)
                    return spriteList[ (SpeedModifier*duration.Sign() > 0)? spriteList.Length - 1 : 0];
                return null;
            }


            var cycleFrameTime = frameDeltaCumulated.Cycle(duration);
            percentFinished = cycleFrameTime / duration;
            int spriteIndex = (int)(percentFinished * spriteList.Length);

            if (spriteList.Length == 0) return null;

            return spriteList[spriteIndex];
        }


        #endregion

    }
}
