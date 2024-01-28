using logiked;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace logiked.source.manager {




    /// <summary>
    /// Type de base pour votre GameManager. <br/>
    /// Vous devez définir dans cette classe tout les Managers que vous utiliserez et les initialiser avec <see cref="BaseManager{T}.Initialization"/> : <br/>
    ///<inheritdoc cref="BaseManager{T}.Initialization"/>
    /// </summary>
    [ExecuteAlways]
    public abstract class BaseGameManager : BaseManager<BaseGameManager>
    {



        /// <summary>
        /// Parent pour les objets générés dans la scène par <see cref="GameObject.Inst()"/>
        /// </summary>
        private static GameObject tempInstancesParents;

        /// <inheritdoc cref="tempInstancesParents"/>
        public static Transform TempInstParent => tempInstancesParents.transform;




        private void Update()
        {
            UpdateGameManager();
        }


        protected virtual void UpdateGameManager() { }


        void Awake()
        {
#if UNITY_EDITOR
            //         UnityEditor.EditorApplication.playModeStateChanged += UpdatePlugins;
#endif

            if (!Application.isPlaying) return;



            tempInstancesParents = new GameObject("TempInstances");
            DontDestroyOnLoad(tempInstancesParents);

            Initialization();
        }


    }

}

