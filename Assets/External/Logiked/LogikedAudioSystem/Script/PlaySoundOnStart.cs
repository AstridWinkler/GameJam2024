using logiked.source.attributes;
using logiked.source.types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.audio
{

    [RequireComponent(typeof(GameAudioSource))]
    public class PlaySoundOnStart : MonoBehaviour
    {
        GameAudioSource source;



        private void Awake()
        {
            source = GetComponent<GameAudioSource>();
        }

        private void OnEnable()
        {
            if(playSoundEvent == PlayWhen.OnEnable)
                source.PlaySound(sound);
        }



        private void Start()
        {
            if (playSoundEvent == PlayWhen.Start)
            {
                source.PlaySound(sound);

                if (automaticRemove && !source.IsLoopingSound)
                {
                    new GameTimer(source.CurrentSoundDuration, () =>
                    {
                        Destroy(this, source.CurrentSoundDuration);
                        Destroy(source, source.CurrentSoundDuration);
                    });
                
                }
            }
        }



        [SerializeField, Tooltip("Le son à jouer.")]
        public GameSound sound;
        /// <summary>
        /// Quand jouer le son ?
        /// </summary>
        private enum PlayWhen { Start=0, OnEnable=1}


        [SerializeField, Tooltip("Quand le son doit-il être joué ?")]
        PlayWhen playSoundEvent = PlayWhen.OnEnable;
        
        
        [ShowIf(nameof(playSoundEvent), ShowIfOperations.Equal, PlayWhen.Start)]
        [SerializeField, Tooltip("Suppression automatique de la source aprés avoir fini de jouer le son. (Si le son est une boucle, n'a aucun effet.)")]
        private bool automaticRemove = true;


    }

}