using UnityEngine;
using logiked.source.attributes;
using logiked.source.extentions;
using UnityEngine.Audio;
using logiked.source.types;
#if UNITY_EDITOR
using logiked.source.editor;
using UnityEditor;
#endif


namespace logiked.audio
{
    /// <summary>
    /// [GROS WIP] Classe qui permet de pouvoir jouer des sons;
    /// </summary>
    [System.Serializable]
    public class GameAudioSource : MonoBehaviour
    {


        #region Fields - AudioSource

        /// <summary>
        /// Type d'AudioSource à utiliser
        /// </summary>
        public enum SoundSourceType { Presset = 0, Generated = 1, ClassicAudioSource = 2 }


        [FieldSectionBegin("Audio Source")]
        [SerializeField, Tooltip("Quel AudioSource utiliser ?")]
        private SoundSourceType audioSourceType = SoundSourceType.Presset;
        public SoundSourceType AudioSourceType { get => audioSourceType; set => audioSourceType = value; }


        //Type : Assign
        [ShowIf(nameof(audioSourceType), ShowIfOperations.Equal, SoundSourceType.ClassicAudioSource)]
        [SerializeField]
        private AudioSource source;



        //Type : generated
        [ShowIf(nameof(audioSourceType), ShowIfOperations.Equal, SoundSourceType.Generated)]
        [SerializeField, Tooltip("Distance max pour l'émission du son")]
        public float maxDistance = 15f;

        [ShowIf(ShowIfRepeatMode.Same)]
        [SerializeField, Tooltip("Le coef de volume. (WIP mettre une courbe un de ces 4)")]
        public float volumeCoeficient = 1f;


        [ShowIf(ShowIfRepeatMode.Same)]
        [SerializeField, Tooltip("Répartition de la stéréo en fonction de la distance par rapport à la source.\n Absice: distance [0;1]\n Ordonnée: 0 = Stéréo, 0.5 = Mono, 1 = Stéréo inversé")]
        public AnimationCurve stereoByDistance = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0.5f), new Keyframe(.5f, 0f) });

        [ShowIf(ShowIfRepeatMode.Same)]
        [SerializeField, Tooltip("Dépendance à la spatialisation 3D (distance, effet doppler). Grosso modo : Si cette courbe est à 0 le son sera entenu partout dans la scene.")]
        public AnimationCurve ambiantSound = new AnimationCurve(new Keyframe[] { new Keyframe(0, .75f), new Keyframe(.5f, 1f) });

        [ShowIf(ShowIfRepeatMode.Same)]
        [SerializeField, Tooltip("Quantité d'effet doppler. [0.1 c'est bien]")]
        public float dopplerEffect = 0.1f;
        [ShowIf(ShowIfRepeatMode.Same)]
        [AudioMixer, Tooltip("AudioMixer à utiliser sur l'AudioSource")]
        public int audioMixer;


        //Type : Presset
        [ShowIf(nameof(audioSourceType), ShowIfOperations.Equal, SoundSourceType.Presset)]
        [SerializeField]
        [AudioSourcePresset, Tooltip("Le presset de l'AudioSource à utiliser")]
        public int sourcePresset;


        [ShowIf(nameof(audioSourceType), ShowIfOperations.NotEqual, SoundSourceType.ClassicAudioSource)]
        [SerializeField, Tooltip("Active l'option 'loop' sur l'AudioSource.")]
        private bool loopSound = false;


        [AudioMixerAttribute]
        [FieldSectionEnd]
        public int mixerType;
        public AudioSource Source => source;
        [SerializeField]
        AudioMixerGroup overrideMixer;


        #endregion

        #region Fields - Values

        /// <summary>
        /// 
        /// </summary>
        public float Volume { get => Volume_; set { Volume_ = value; fadeVolTimer.AutoStop(); } }
        public float Pitch { get => Pitch; set { source.pitch = calculatedPitch * value * basePitch; userSetPitch = value; } }

        public float Volume_ { get => userSetVolume; set { source.volume = calulatedVolume * value * baseVolume; userSetVolume = value; } }



        public float CurrentUsedPitch => source.pitch;

        private float userSetVolume = 1;
        private float userSetPitch = 1;

        private float calulatedVolume = 1;
        private float calculatedPitch = 1;

        private float baseVolume = 1;
        private float basePitch = 1;



        bool isInit = false;


        /// <summary>
        /// Le son actuellement joué
        /// </summary>
        public AudioClip CurrentlyPlaying { get; private set; }

        /// <summary>
        /// Durée totale du son actuellement joué.
        /// </summary>
        public float CurrentSoundDuration { get => CurrentlyPlaying ? CurrentlyPlaying.length : 0; }

        /// <summary>
        /// Le son actuellement joué
        /// </summary>
        public bool IsLoopingSound { get => source.loop; set => source.loop = value; }
   

        
        #endregion



        #region Methods - Event


        private void OnDestroy()
        {
            if (source != null)
                Destroy(source);
        }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            if (audioSourceType == SoundSourceType.Generated)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, maxDistance);
            }
        }

        private void Awake()
        {
            Init();
        }


        private void Init()
        {
            if (isInit) return;
            isInit = true;


            switch (audioSourceType)
            {

                case SoundSourceType.Generated:

                    source = gameObject.AddComponent<AudioSource>();

                    source.dopplerLevel = dopplerEffect;
                    source.maxDistance = maxDistance;

                    source.volume = volumeCoeficient;
                    source.SetCustomCurve(AudioSourceCurveType.Spread, stereoByDistance);
                    source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, ambiantSound);

                    if(overrideMixer != null)
                    source.outputAudioMixerGroup = overrideMixer;

                    
                    var curve = source.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
                    curve.keys = new[] { new Keyframe(0, 1f), new Keyframe(1, 0), };
                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
                    

                    source.rolloffMode = AudioRolloffMode.Custom;
                    source.playOnAwake = false;
                    IsLoopingSound= loopSound;

                    source.outputAudioMixerGroup = LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.GetAudioMixerPresset(audioMixer);

                    break;


                case SoundSourceType.Presset:

                    if (sourcePresset == -1)
                    {
                        Debug.LogError("Presset is null");
                        return;
                    }

                    var p = LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase.GetAudioSourcePresset(sourcePresset);
                    source = p.CopyComponent(gameObject);
                    IsLoopingSound = loopSound;

                    source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, p.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
                    source.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, p.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
                    source.SetCustomCurve(AudioSourceCurveType.SpatialBlend, p.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
                    source.SetCustomCurve(AudioSourceCurveType.Spread, p.GetCustomCurve(AudioSourceCurveType.Spread));
                  
                    if (overrideMixer != null)
                        source.outputAudioMixerGroup = overrideMixer;

                    break;

            }

            baseVolume = source.volume;
            basePitch = source.pitch;
            source.playOnAwake = false;

        }


        #endregion

        #region Methods - Audio

        public void StopSound()
        {
            if (source != null)
                source.Stop();
        }

        GameTimer fadeVolTimer;
        float curVol;
        float targetVol;

        public void VolumeFade(float newVolume, float time)
        {
            if (targetVol == newVolume) return;
            fadeVolTimer.AutoStop();

            targetVol = newVolume;
            curVol = Volume_;
        
            fadeVolTimer = new GameTimer(0.15f, (t) =>
            {
                if (this == null) return false;
                Volume_ = Mathf.Lerp(curVol, newVolume , t.Percent);  
                return true;
            });
        }

        public void PlaySound(GameSoundFile sound)//Pour que les UnityEvents (genre buttons) puissent appeler cette fonction
        {
            PlaySound(sound.Sound);
        }

        public void PlaySoundClip(AudioClip sound)//Pour que les UnityEvents (genre buttons) puissent appeler cette fonction
        {
            PlaySound(new GameSound(sound) );
        }


        public void PlaySound(GameSound sound)//, bool loop = false)
        {
            if (!isInit) Init();

            if (sound == null) return;

            var clip = sound.GetAClip;

            if (clip == null) return;

            calulatedVolume = sound.VolumeAuto;
            calculatedPitch = sound.PitchAuto;

            if (IsLoopingSound && source.clip == clip && source.isPlaying) return;//Ne pas rejouer la musique actuelle si elle joue déja

           // source.loop = loop;

                      
            source.volume = baseVolume * calulatedVolume * userSetVolume;
            source.pitch = basePitch * calculatedPitch * userSetPitch;
            CurrentlyPlaying = clip;
            source.clip = (CurrentlyPlaying);

            if (IsLoopingSound)
            {
                if (source.isPlaying)
                    source.Stop();

                source.Play();
            }
            else
            {

                source.PlayOneShot(CurrentlyPlaying);
            }
        }

        #endregion
    
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(GameAudioSource))]
    internal class Inspector_GameAudioSource : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Show database ");
            GUILogiked.Panels.GUIDrawEditorIcon(() => ProjectBrowserReflection.SelectAssetInProjectWindow(LogikedPlugin_AudioSystem.Instance.DefaultAudioDatabase), GUILogiked.Panels.EditorIconType.FolderWhite);
            GUILayout.EndHorizontal();
        }
    }

#endif




}