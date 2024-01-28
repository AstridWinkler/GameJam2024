using UnityEngine;
using logiked.source.utilities;
using System.Collections.Generic;
using System;
using logiked.source.types;

#if UNITY_EDITOR
using static logiked.source.graphNode.GraphNodeUtils;
using logiked.source.editor;
using UnityEditor;
using UnityEditor.Presets;
#endif

namespace logiked.Tool2D.settings
{
    /// <summary>
    /// <inheritdoc/>
    /// 
    /// Les propriétés du plugin 2DTools
    /// </summary>
    /// <seealso cref="logiked.LogikedAssemblySettings" />
    [CreateAssetMenu(fileName = "2DToolsAssemblySettings", menuName = "Logiked/2DTools/AssemblySettings", order = 1)]
    [System.Serializable]
    public class LogikedPlugin_2DTools : LogikedPlugin<LogikedPlugin_2DTools>
    {

#if UNITY_EDITOR
        /// <summary>Permet de creer le fichier automatiquement au chargement du script, dans un dossier resource.</summary>
        [UnityEditor.InitializeOnLoad]
        public class LogikedPluginFileLoader
        {
            static LogikedPluginFileLoader()
            {
                CreateSettingsInstance("settings2DTools", "Logiked_2DTools");
            }
        }


        public const string MenuItemName = "Assets/2D Tools/";
#endif


        
        public override Color LogColors => (Color.yellow + Color.white*0.5f)*0.65f;



        #region Editor Params
        [Header("Editor settings")]



#if UNITY_EDITOR
        [SerializeField]
        [EditScriptable]
        private Preset textureMainImportPresset = null;
        public Preset TextureMainImportPresset { get { return textureMainImportPresset; } }

        [SerializeField]
        private Preset[] otherImportPressets = new Preset[0];
        public Preset[] OtherImportPressets { get { return otherImportPressets; } }

        public  Tuple<string[], string[]> ImportSettingPressetGuidList 
        {
            get
            {
                List<string> names = new List<string>();
                List<string> guuid = new List<string>();

                names.Add("none");
                guuid.Add("0");

          
                    names.Add("Default");
                    guuid.Add(TextureMainImportPresset?.GetGUID());

                foreach(var e in otherImportPressets)
                {
                    if (e == null) continue;
                    names.Add(e.name);
                    guuid.Add(e.GetGUID());
                }


                return new Tuple<string[], string[]>(names.ToArray(), guuid.ToArray());

            }
        }



        [SerializeField]
        private NodeLineDesign nodeTransitionDesign = NodeLineDesign.SexyBezier;
        public NodeLineDesign NodeTransitionStyleDesign { get { return nodeTransitionDesign; } set { nodeTransitionDesign = value; } }

#endif


        [SerializeField]
        private int defaultSpriteCutSize = 16;
        public int DefaultSpriteCutSize { get { return defaultSpriteCutSize; } }
        
        /*
        [SerializeField]
        private int defaultPixelPerUnit = 16;
        public int DefaultPixelPerUnit { get { return defaultPixelPerUnit; } }
        */

        [Header("Animations Editor")]
        [SerializeField]
        private bool playAnimationsInSceneView = true;
        public bool PlayAnimationsInSceneView { get { return playAnimationsInSceneView; } }

        [SerializeField]
        private bool playAnimatorRenderersInSceneView = true;
        public bool PlayAnimatorRenderersInSceneView { get { return playAnimatorRenderersInSceneView; } }


        [SerializeField]
           private bool playAnimationsInEditorPreview = true;
           public bool PlayPreviewInEditor { get { return playAnimationsInEditorPreview; } set { playAnimationsInEditorPreview = value; }  }
        //   [SerializeField]
        //   private bool showEditorDeletePopups = true;//Affiche les popups dans l'editeur d'animation du style (Voulez vous désactivez le mode AnimationWorkspace ?)
        //   public bool ShowEditorDeletePopups { get { return showEditorDeletePopups; } set { showEditorDeletePopups = value; }  }
        [SerializeField]
        private bool nodeGridPlacement = true;//Clamp les nodes a la grille ?
        public bool NodeGridPlacement { get { return nodeGridPlacement;} set { nodeGridPlacement = value; } }





        #endregion

    }

}
