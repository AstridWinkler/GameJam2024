using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace logiked.source.types
{

    /// <summary>
    /// Timer de jeu pour faire <para/>
    /// - Un délai d'attente  <para/>
    /// - Une interpolation/animation d'une valeur (Lerp/Slerp...)
    /// - [WIP] Mettre en pause selon des parametres du jeu (menu de pause, cinématique, etc..)
    /// Voir <see cref="extentions.TimerExtention"/> pour d'autres fonctions.
    /// </summary>
    public class GameTimer
    {

        public delegate bool EachFrameAction(GameTimer t);


        private static TimerCoroutineContainer timerManager;

        private float totalWaitTime;
        private float currentWaitTime;

        private bool isRunning=false;

        private Action endAction;
        private Coroutine currentCor = null;
      
        /// <summary>
        /// Action qui est appelée chaque frame du timer avec 1 paramettre. Utile pour faire des interpolation.
        /// Le paramettre est un float qui varie de 0 à 1 en fonction de l'état d'avancement du timer.
        /// Cette Action doit retourner un booléen qui indique si le timer doit stopper ou non (False = arret du timer).
        /// </summary>
        private EachFrameAction eachFrame = (t) => true;
        public event EachFrameAction EachFrame { add { eachFrame += value; } remove { eachFrame -= value; } }

        /// <summary>
        /// Le timer est t-il en train de tourner?
        /// </summary>
        public bool IsRunning => isRunning;
        /// <summary>
        /// Le timer à t-il fini de tourner ?
        /// </summary>
        public bool IsDone => !isRunning;

        /// <summary>
        /// Le pourcentage de temps restant (compris entre 0 et 1)
        /// </summary>
        public float Percent => currentWaitTime / totalWaitTime;
        /// <summary>
        /// Le temps restant en secondes
        /// </summary>
        public float RemainTime => totalWaitTime - currentWaitTime;


        /// <param name="time">Temps à attendre en secondes</param>
        /// <param name="endCallback">La foncion appelée une fois le timer écoulé </param>
        public GameTimer(float time, Action endCallback = null) : this(time, endCallback, null) { }

        /// <param name="time">Temps à attendre en secondes</param>
        /// <param name="eachFrame">La fonction appelée a chaque frame du timer (retourner boolean, parametre : GameTimer)</param>
        public GameTimer(float time, EachFrameAction eachFrame) : this(time, null , eachFrame) { } 


        /// <param name="time">Temps à attendre en secondes</param>
        /// <param name="endCallback">La foncion appelée une fois le timer écoulé </param>
        /// <param name="eachFrame">La fonction appelée a chaque frame du timer (retourner boolean, parametre : GameTimer)</param>
        public GameTimer(float time, Action endCallback, EachFrameAction eachFrame)
        {


            endAction = endCallback;
            isRunning = true;
            totalWaitTime = time;
            EachFrame += eachFrame;

            if (time <= 0)
            {
                Finish();
                return;
            }

            if (timerManager == null)
                timerManager = new GameObject("TimeManager").AddComponent<TimerCoroutineContainer>();
            currentCor = timerManager.StartCor(CorLoop());
        }


        /// <summary>
        /// Arrete le timer sans réaliser le callback
        /// </summary>
        public void Stop()
        {
            if (currentCor != null)
                timerManager.StopCor(currentCor);
            eachFrame = (t) => true;
            isRunning = false;
        }

        /// <summary>
        /// Réalise le callback du timer et l'arrete
        /// </summary>
        public void Done()
        {
            Stop();
            Finish();
        }


        IEnumerator CorLoop()
        {

            currentWaitTime = 0;

            bool eachFrameCondition = true;

            while (currentWaitTime < totalWaitTime && (eachFrameCondition = eachFrame(this)) )
            {
                currentWaitTime += Time.deltaTime;
                yield return null;
            }

            if (!eachFrameCondition)
            {
                Stop();
                yield break;
            }


            currentWaitTime = totalWaitTime;

            Finish();
        }


        void Finish()
        {

            isRunning = false;
            currentWaitTime = 1;
            totalWaitTime = 1;
            eachFrame(this);

            if (endAction != null)
                endAction();
        }
    }



}
