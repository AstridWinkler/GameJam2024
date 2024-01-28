

using logiked.source;
using logiked.source.types;
using UnityEngine;


namespace logiked.audio
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [CreateAssetMenu(fileName = "AudioAssemblySettings", menuName = CreateAssetMenuName+"/AssemblySettings", order = 1)]
    [System.Serializable]
    public class LogikedPlugin_AudioSystem : LogikedPlugin<LogikedPlugin_AudioSystem>
    {
        
        public const string CreateAssetMenuName = "Logiked/Audio/";

#if UNITY_EDITOR
        /// <summary>Permet de creer le fichier automatiquement au chargement du script, dans un dossier resource.
        /// </summary>
        [UnityEditor.InitializeOnLoad] class InitPlugin {
            static InitPlugin()
            {
                LogikedPlugin<LogikedPlugin_AudioSystem>.CreateSettingsInstance("settingsAudioSystem", "Logiked_AudioSystem");
            }
        }
#endif

        [SerializeField]
        private AudioDatabase defaultAudioDatabase;


#if UNITY_EDITOR
        public AudioDatabase DefaultAudioDatabase { get => defaultAudioDatabase; set => defaultAudioDatabase = value; }
#else
        public AudioDatabase DefaultAudioDatabase { get => defaultAudioDatabase; }

#endif

        [SerializeField] private float defaultGameSoundVolumeVariation = 0.05f;
        [SerializeField] private float defaultGameSoundPitchVariation = 0.05f;


        public float DefaultGameSoundVolumeVariation { get => defaultGameSoundVolumeVariation; }
        public float DefaultGameSoundPitchVariation { get => defaultGameSoundPitchVariation; }

    }

}
