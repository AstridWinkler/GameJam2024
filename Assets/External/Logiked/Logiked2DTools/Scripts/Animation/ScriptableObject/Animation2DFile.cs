
using logiked.source.utilities;
using UnityEditor;
using UnityEngine;


    namespace logiked.Tool2D.animation
    {
    /// <summary>
    /// Un fichier d'animation
    /// </summary>
    /// <seealso cref="logiked.LogikedAssemblySettings" />
    [CreateAssetMenu(fileName = "anim", menuName = "Logiked/2DTools/2D Animation", order = 2)]


    [System.Serializable]
    public class Animation2DFile : ScriptableObject
    {

        public enum LoopProcess { Loop, Once, PingPong , OnceDisapear }


        /// <summary>
        /// Stocker une frameRate au lieu d'une durée permet de détacher la valeur de la vitesse du nombre de frames.
        /// Mais on va quand meme stoquer une durée parce que c'est vraiment plus utile pour le cerveau humain et pour les cas de figures commun.
        /// Exemple : "L'animation d'attaque doit durer 0.6 secondes" Bref on s'en fout de la frame rate.
        /// </summary>
        [SerializeField]
        private float duration = 1f;
        public float Duration
        {
            get { return duration; }
#if UNITY_EDITOR
            set { duration = value; }
#endif
        }

        /// <summary>
        /// Interaction à la fin de l'animation
        /// Loop : boucle
        /// Once : Lecture une seule fois
        /// PingPong : Boucle avec aller/retour de l'animation
        /// </summary>        
        [SerializeField]
        private LoopProcess loopMode;
        public LoopProcess LoopMode
        {
            get { return loopMode; }
#if UNITY_EDITOR
            set { loopMode = value; }
#endif
        }


        [ SerializeField]
        private Sprite[] sprites;

        public virtual Sprite[] Sprites { get { return sprites; }
#if UNITY_EDITOR
            set {  sprites = value; }
#endif
        }

        /// <summary>
        /// Récupère des sprites spécifiques
        /// </summary>
        /// <param name="variationId">L'Id de la variation. N'a un effet que sur une <see cref="Animation2DFileVariations"/></param>
        /// <returns>Les sprites</returns>
        public virtual Sprite[] GetSprites(int variationId)
        {
            return Sprites;
        }





    }
}
