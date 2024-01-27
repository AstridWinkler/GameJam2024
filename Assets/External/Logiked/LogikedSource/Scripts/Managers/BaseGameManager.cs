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

        [SerializeField]
        private List<ScriptableObject> logikedPluginSettings = new List<ScriptableObject>();


        public ScriptableObject GetPluginSettings(Type setting)
        {
            return logikedPluginSettings.FirstOrDefault(m => m.GetType() == setting);
        }


        private void Update()
        {
            UpdateGameManager();
        }


        protected virtual void UpdateGameManager() { }

        /*

#if UNITY_EDITOR
        void UpdatePlugins(UnityEditor.PlayModeStateChange change)
        {
            switch (change)
            {
                case UnityEditor.PlayModeStateChange.ExitingEditMode:

                    break;


            }
        }
#endif

        void UpdatePlugins()
        {

        }
        */

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

