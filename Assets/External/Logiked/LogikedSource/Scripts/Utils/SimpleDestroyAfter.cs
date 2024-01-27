using logiked.source.extentions;
using logiked.source.types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace logiked.source.utilities
{
    /// <summary>
    /// Classe utilitaire qui suprime l'objet défini aprés un temps.
    /// </summary>
    public class SimpleDestroyAfter : MonoBehaviour
    {

        /// <summary>
        /// Le temps avant la destruction de l'objet
        /// </summary>
        [Tooltip("Le temps avant la destruction de l'objet.")]
        public float time = 1f;
        /// <summary>
        /// L'objet en question. Si null, l'objet choisis sera ce GameObject.
        /// </summary>
        [Tooltip("L'objet en question. Si null, l'objet choisis sera ce GameObject.")]
        public GameObject obj;
       
        /// <summary>
        /// Utiliser la destruction sécurisée ? (Extrait les ParticleSystem, Trails.. en les laissant terminer leur animation pour eviter d'avoir des effets moches à l'écran)
        /// </summary>
        [SerializeField]
        [Tooltip("Le temps avant la destruction de l'objet.")]
        bool safetyDestroy;

        void Start()
        {
            if (obj == null)
                obj = gameObject;
            new GameTimer(time, Die/*,  GameClockState.GamePaused, GameClockState.ActivePhysics*/);
        }


        void Die()
        {
            if (!safetyDestroy)
                Destroy(obj);
            else
                obj.SafetyDestroyWithComponents();

        }

    }
}