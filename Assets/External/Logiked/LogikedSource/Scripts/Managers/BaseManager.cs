using System;
using UnityEngine;


namespace logiked.source.manager
{

    public interface IBaseManager
    {
        public void Initialization();
        public GameObject gameObject { get; }
    }


        /// <summary>
        /// Type de base pour tout vos managers-singletons de votre scène
        /// </summary>
        /// <typeparam name="T">Type de votre Manager à récrire (pour le champ `Instance` du singleton)</typeparam>
        public abstract class BaseManager<T> : MonoBehaviour, IBaseManager where T : MonoBehaviour
    {
        [NonSerialized] private bool alreadyInit = false;
        public bool AlreadyInit => alreadyInit;

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null) instance = GameObject.FindObjectOfType<T>();
                return instance;
            }
        }

        /// <summary>
        /// Méthode d'Initialisation de votre manager (appelée par le GameManager). <br/>
        /// Cela permet d'initialiser tout les Managers dans un explicitement déterminé
        /// contrairement aux fonction event Start(), Awake().. qui s'executent parfois de manière   
        /// aléatoire. Par exemple, c'est assez pénible quand votre jeu démare avant que vos resources soient initialisés.
        /// </summary>
        public void Initialization()
        {
            if (alreadyInit) return;
            alreadyInit = true;
            InitManager();
        }


        ///<inheritdoc cref="Initialization"/>
        protected abstract void InitManager();



    }
}
